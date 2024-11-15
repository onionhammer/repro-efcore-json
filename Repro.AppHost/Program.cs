using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add Postgres
var postgresdb = builder
    .AddPostgres("postgres").WithPgWeb()
    .AddDatabase("postgres-database", "postgres");

// Add worker
builder.AddProject<Repro_Worker>("worker")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.Build().Run();
