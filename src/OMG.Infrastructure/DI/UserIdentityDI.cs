using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OMG.UserIdentity.Context;
using OMG.UserIdentity.Entities;

namespace OMG.Infrastructure.DI;

/// <summary>
/// Extensões de DI para ASP.NET Core Identity
/// </summary>
public static class UserIdentityDI
{
    /// <summary>
    /// Registra o ASP.NET Core Identity com ApplicationUser e IdentityRole
    /// </summary>
    public static IServiceCollection AddUserIdentity(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UserIdentityDBContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configurações de senha
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // Configurações de bloqueio
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // Configurações de usuário
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<UserIdentityDBContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
