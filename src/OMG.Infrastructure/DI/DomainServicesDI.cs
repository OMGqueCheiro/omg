using Microsoft.Extensions.DependencyInjection;
using OMG.Core.Handler;
using OMG.Domain.Contracts;
using OMG.Domain.Contracts.Service;
using OMG.Domain.Services;

namespace OMG.Infrastructure.DI;

/// <summary>
/// Extensões de DI para serviços de domínio
/// </summary>
public static class DomainServicesDI
{
    /// <summary>
    /// Registra todos os serviços de domínio (business logic)
    /// </summary>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<IPedidoService, PedidoService>();
        services.AddTransient<ICorService, CorService>();
        services.AddTransient<IFormatoService, FormatoService>();
        services.AddTransient<IProdutoService, ProdutoService>();
        services.AddTransient<IAromaService, AromaService>();
        services.AddTransient<IClienteService, ClienteService>();
        services.AddTransient<IEmbalagemService, EmbalagemService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddSingleton<IFeatureToggleHandler, FeatureToggleService>();

        return services;
    }
}
