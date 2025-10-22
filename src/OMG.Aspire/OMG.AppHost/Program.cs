var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql-server-db-omg")
                 .WithImageTag("2022-latest")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database", "OMGdb");

var migrations = builder.AddProject<Projects.OMG_MigrationWorker>("omg-migrationworker")
    .WithReference(db)
    .WaitFor(db);

var api = builder.AddProject<Projects.OMG_Api>("omg-api")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WithExternalHttpEndpoints();

// Blazor Auto (SSR + WASM) com YARP proxy reverso
// O Host usa service discovery do Aspire para conectar à API
// O Client usa proxy reverso do Host (/api/* -> omg-api)
var webApp = builder.AddProject<Projects.OMG_WebApp>("omg-webapp")
    .WithReference(api)  // Service discovery funciona!
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
