using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using OMG.Domain;
using OMG.Domain.Handler;
using OMG.WebApp.Authentication;
using OMG.WebApp.Components;
using OMG.WebApp.Handler;
using System.Globalization;

namespace OMG.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add Authentication State Service (scoped to circuit)
        builder.Services.AddScoped<AuthenticationStateService>();
        builder.Services.AddScoped<AuthenticatedHttpClientFactory>();

        // Add Authentication for Blazor Server
        builder.Services.AddAuthentication()
            .AddCookie(options =>
            {
                options.Cookie.Name = "OMG.Auth";
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            });

        // Add Authorization
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<CustomAuthenticationStateProvider>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddTransient<IPedidoHandler, PedidoHandler>();
        builder.Services.AddTransient<IClienteHandler, ClienteHandler>();
        builder.Services.AddTransient<IAuthHandler, AuthHandler>();
        builder.Services.AddTransient(typeof(IBaseSearchHandler<>), typeof(BaseSearchHandler<>));

        // Configure HttpClient with Authentication
        // IMPORTANTE: Não usar AddHttpMessageHandler pois cria o handler fora do escopo do circuito
        // Os handlers vão injetar o HttpClient e manualmente adicionar o header de autorização
        builder.Services.AddHttpClient(Configuracao.HttpClientNameOMGApi, opt =>
        {
            opt.BaseAddress = new Uri("https://omg-api");
        });

        builder.Services.AddLocalization();
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
