using OMG.WebApp.Components;
using OMG.WebApp.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar cultura pt-BR
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add services to the container.
// Blazor WASM puro - O prerendering será desabilitado via @rendermode
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// MudBlazor
builder.Services.AddMudServices();

// Authentication/Authorization mínima para satisfazer middlewares do ASP.NET Core
// IMPORTANTE: Autenticação REAL é 100% no Client (WASM) via PersistentAuthenticationStateProvider
// Este handler "no-op" apenas satisfaz o pipeline sem interferir com WASM
builder.Services.AddAuthentication("NoOp")
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, 
               NoOpAuthenticationHandler>("NoOp", null);

builder.Services.AddAuthorization();

// HttpClient com service discovery do Aspire para acessar omg-api durante SSR
// Aspire resolve automaticamente https+http://omg-api para a URL real
builder.Services.AddHttpClient("omg-api", client =>
{
    client.BaseAddress = new Uri("https+http://omg-api");
});

// Não registramos Handlers no Server - eles são exclusivos do Client WASM
// O Server é apenas um host estático + proxy reverso (YARP)

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

// Middlewares de autenticação (no-op para satisfazer o pipeline ASP.NET Core)
// IMPORTANTE: Não interfere com autenticação WASM pois não há prerendering
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// YARP Reverse Proxy
app.MapReverseProxy();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(OMG.WebApp.Client._Imports).Assembly);

// Importante: Não adicionar Pages do Server para evitar rotas duplicadas
// Todas as páginas vêm do WebApp.Client (WASM)

app.Run();
