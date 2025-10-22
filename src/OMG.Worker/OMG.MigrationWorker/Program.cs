using OMG.MigrationWorker;
using OMG.Infrastructure.DI;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var connectionString = builder.Configuration.GetConnectionString("database") 
                       ?? throw new InvalidOperationException("Connection string 'database' not found.");

builder.Services.AddUserIdentity(connectionString);

builder.AddOMGRepository();

var host = builder.Build();
host.Run();