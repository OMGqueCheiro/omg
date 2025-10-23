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

var webApp = builder.AddProject<Projects.OMG_WebApp>("omg-webapp")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

var blazorApp = builder.AddProject<Projects.OMG_BlazorApp>("omg-blazorapp")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
