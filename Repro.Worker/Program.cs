using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repro.Worker;
using Repro.Worker.Model;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var jsonOptions = new System.Text.Json.JsonSerializerOptions()
{
    AllowOutOfOrderMetadataProperties = true,
};

var connectionString = builder.Configuration.GetConnectionString("postgres-database");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString)
    .EnableDynamicJson()
    .ConfigureJsonOptions(jsonOptions);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<PostgresContext>(options =>
{
    options.UseNpgsql(dataSource);
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
});

// Create a lead
var leadId = Guid.NewGuid();
var match = new HistoryMatched() { MatchedProperty = "Test" };
var complete = new HistoryComplete() { DateCompleted = DateTime.UtcNow };
var lead = new Lead()
{
    Id = leadId,
    LastHistory = complete,
    History = [
        match,
        complete
    ]
};

// Assert JSON serialization of a lead
var json = System.Text.Json.JsonSerializer.Serialize(lead);
var deserialized = System.Text.Json.JsonSerializer.Deserialize<Lead>(json);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();

    db.Database.EnsureCreated();

    db.Leads.Add(lead);
    db.SaveChanges();
}

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();
    var readLead = db.Leads.First(p => p.Id == lead.Id);
    // var readLead = db.Leads.Include(h => h.History).First(p => p.Id == lead.Id);
    // var readLead = db.Leads.First();
}

host.Run();
