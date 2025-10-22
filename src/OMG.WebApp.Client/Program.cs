using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using MudBlazor.Services;
using OMG.WebApp.Client.Authentication;
using OMG.Core;
using OMG.Core.Handler;
using OMG.WebApp.Client.Handlers;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configurar cultura pt-BR
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// MudBlazor
builder.Services.AddMudServices();

// Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

// HttpClient configurado para fazer requisições através do Host
// O Host fará proxy via YARP para omg-api com service discovery
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Registrar Handlers (Client-side) - Usados tanto em SSR quanto em WASM
builder.Services.AddScoped<IAuthHandler, AuthHandler>();
builder.Services.AddScoped<IPedidoHandler, PedidoHandler>();
builder.Services.AddScoped<IClienteHandler, ClienteHandler>();
builder.Services.AddScoped(typeof(IBaseSearchHandler<>), typeof(BaseSearchHandler<>));

await builder.Build().RunAsync();
