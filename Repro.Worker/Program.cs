using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Repro.Worker;
using Repro.Worker.Model;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// var jsonOptions = new System.Text.Json.JsonSerializerOptions()
// {
//     AllowOutOfOrderMetadataProperties = true,
// };

var connectionString = builder.Configuration.GetConnectionString("postgres-database");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<PostgresContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.EnrichNpgsqlDbContext<PostgresContext>();

// Create a lead
var lead = new Lead()
{
    HistoryJson = [
        new HistoryMatched() { MatchedProperty = "Matched" },
        new HistoryComplete() { DateCompleted = DateTimeOffset.Now }
    ],
    HistoryJsonB = [
        new HistoryMatched() { MatchedProperty = "Matched" },
        new HistoryComplete() { DateCompleted = DateTimeOffset.Now }
    ]
};

// Assert JSON serialization of a lead
var json = System.Text.Json.JsonSerializer.Serialize(lead);//, jsonOptions);
var deserialized = System.Text.Json.JsonSerializer.Deserialize<Lead>(json);//, jsonOptions);

Debug.Assert(deserialized != null);
Debug.Assert(lead.HistoryJson.Count == deserialized.HistoryJson.Count);
Debug.Assert(lead.HistoryJson[0].DateCreated == deserialized.HistoryJson[0].DateCreated);
Debug.Assert(lead.HistoryJson[0] is HistoryMatched);

var host = builder.Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();

db.Database.EnsureCreated();

db.Leads.Add(lead);
db.SaveChanges();

// // Read the lead with json
// var readLeadJson = db.Leads.Select(p => new { p.Id, p.HistoryJson }).First();

// // Read the lead with jsonb
// var readLeadJsonB = db.Leads.Select(p => new { p.Id, p.HistoryJsonB }).First();

var readLead = db.Leads.First();

host.Run();
