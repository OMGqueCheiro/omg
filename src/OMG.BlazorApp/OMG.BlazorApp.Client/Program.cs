using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Blazored.LocalStorage;
using OMG.BlazorApp.Client;
using OMG.BlazorApp.Client.Handlers;
using OMG.BlazorApp.Client.Authentication;
using OMG.Core.Handler;
using OMG.Core.Entities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Montar o componente raiz App no div#app
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// Blazored.LocalStorage para armazenar o token JWT
builder.Services.AddBlazoredLocalStorage();

// Autenticação
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>(sp => 
    (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// HttpClient configurado para chamar o servidor Blazor, que por sua vez chama a API via YARP
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// HttpClient nomeado para chamadas diretas à API (opcional, se precisar)
builder.Services.AddHttpClient("OMG.Api", client =>
{
    // O servidor Blazor fará proxy para /api via YARP
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Registrar handlers
builder.Services.AddScoped<IAuthHandler, AuthHandler>();
builder.Services.AddScoped<IClienteHandler, ClienteHandler>();
builder.Services.AddScoped<IPedidoHandler, PedidoHandler>();
builder.Services.AddScoped(typeof(IBaseSearchHandler<>), typeof(BaseSearchHandler<>));

// Registrar handlers CRUD genéricos
builder.Services.AddScoped<ICrudHandler<Cliente>>(sp => 
    new CrudHandler<Cliente>(sp.GetRequiredService<HttpClient>(), "Cliente"));
builder.Services.AddScoped<ICrudHandler<Produto>>(sp => 
    new CrudHandler<Produto>(sp.GetRequiredService<HttpClient>(), "Produto"));
builder.Services.AddScoped<ICrudHandler<Formato>>(sp => 
    new CrudHandler<Formato>(sp.GetRequiredService<HttpClient>(), "Formato"));
builder.Services.AddScoped<ICrudHandler<Cor>>(sp => 
    new CrudHandler<Cor>(sp.GetRequiredService<HttpClient>(), "Cor"));
builder.Services.AddScoped<ICrudHandler<Aroma>>(sp => 
    new CrudHandler<Aroma>(sp.GetRequiredService<HttpClient>(), "Aroma"));
builder.Services.AddScoped<ICrudHandler<Embalagem>>(sp => 
    new CrudHandler<Embalagem>(sp.GetRequiredService<HttpClient>(), "Embalagem"));

await builder.Build().RunAsync();