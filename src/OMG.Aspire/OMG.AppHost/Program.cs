var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql-server-db-omg")
                 .WithImageTag("2022-latest")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database", "OMGdb");

var api = builder.AddProject<Projects.OMG_Api>("omg-api")
    .WithReference(db)
    .WaitFor(db);

var webApp = builder.AddProject<Projects.OMG_WebApp>("omg-webapp")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
