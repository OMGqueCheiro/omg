var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres-db-omg")
                      .WithImageTag("17-alpine")
                      .WithDataVolume("omg-postgres-data")
                      .WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("database", "OMGdb");

// PgWeb - Interface web para PostgreSQL
var pgweb = builder.AddContainer("pgweb", "sosedoff/pgweb")
                   .WithHttpEndpoint(port: 8081, targetPort: 8081, name: "pgweb")
                   .WithLifetime(ContainerLifetime.Persistent);

var migrations = builder.AddProject<Projects.OMG_MigrationWorker>("omg-migrationworker")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.OMG_Api>("omg-api")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WithExternalHttpEndpoints();

var blazorApp = builder.AddProject<Projects.OMG_BlazorApp>("omg-blazorapp")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
