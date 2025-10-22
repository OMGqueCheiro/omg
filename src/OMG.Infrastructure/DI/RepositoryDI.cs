using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OMG.Core.Base.Contract;
using OMG.Domain.Contracts.Repository;
using OMG.Repository;
using OMG.Repository.Repositories;

namespace OMG.Infrastructure.DI;

/// <summary>
/// Extensões de DI para repositórios e DbContext
/// </summary>
public static class RepositoryDI
{
    /// <summary>
    /// Registra o DbContext e todos os repositórios
    /// Usa Aspire SQL Server integration para service discovery
    /// </summary>
    public static IHostApplicationBuilder AddOMGRepository(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<OMGDbContext>("database", configureDbContextOptions: option =>
        {
            option.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            option.UseLazyLoadingProxies(); // Habilita proxies dinâmicos para lazy loading
        });

        builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
        builder.Services.AddTransient<IEventRepository, EventRepository>();

        // Registro genérico para qualquer entidade
        builder.Services.AddTransient(typeof(IRepositoryEntity<>), typeof(EntityRepository<>));

        return builder;
    }
}
