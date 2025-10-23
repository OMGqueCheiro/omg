using MudBlazor.Services;
using OMG.BlazorApp.Client.Pages;
using OMG.BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// YARP Reverse Proxy para encaminhar /api/* para omg-api
// O Aspire injeta automaticamente os endpoints via service discovery
// A URL será resolvida em runtime via configuração services:omg-api:https:0 ou http:0
var apiUrl = builder.Configuration["services:omg-api:https:0"] 
             ?? builder.Configuration["services:omg-api:http:0"]
             ?? "https://localhost:7086"; // Fallback para desenvolvimento sem Aspire

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            new Yarp.ReverseProxy.Configuration.RouteConfig
            {
                RouteId = "api-route",
                ClusterId = "omg-api-cluster",
                Match = new Yarp.ReverseProxy.Configuration.RouteMatch
                {
                    Path = "/api/{**catch-all}"
                }
            }
        },
        new[]
        {
            new Yarp.ReverseProxy.Configuration.ClusterConfig
            {
                ClusterId = "omg-api-cluster",
                Destinations = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>
                {
                    { "destination1", new Yarp.ReverseProxy.Configuration.DestinationConfig { Address = apiUrl } }
                }
            }
        });

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// YARP Reverse Proxy
app.MapReverseProxy();

app.MapStaticAssets();

// Blazor WebAssembly puro - sem prerendering
// O componente raiz é servido como arquivo estático
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(OMG.BlazorApp.Client._Imports).Assembly);

app.Run();
