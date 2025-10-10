using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OMG.Repository;

namespace OMG.MigrationWorker;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OMGDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            await dbContext.SeedData();
            
            // Migrar banco de dados do Identity
            var identityDbContext = scope.ServiceProvider.GetRequiredService<UserIdentity.Context.UserIdentityDBContext>();
            await identityDbContext.Database.MigrateAsync(cancellationToken);
            
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }
    
    // private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    // {
    //     var strategy = dbContext.Database.CreateExecutionStrategy();
    //     await strategy.ExecuteAsync(async () =>
    //     {
    //         // Run migration in a transaction to avoid partial migration if it fails.
    //         await dbContext.Database.MigrateAsync(cancellationToken);
    //     });
    // }
    //
    // private static async Task SeedDataAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    // {
    //     var strategy = dbContext.Database.CreateExecutionStrategy();
    //     await strategy.ExecuteAsync(async () =>
    //     {
    //         // Seed the database
    //         await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    //         if (!await dbContext.Roles.AnyAsync(cancellationToken))
    //         {
    //             await dbContext.Roles.AddAsync(new IdentityRole("Cliente"), cancellationToken);
    //             await dbContext.Roles.AddAsync(new IdentityRole("Autor"), cancellationToken);
    //             await dbContext.Roles.AddAsync(new IdentityRole("Parceiro"), cancellationToken);
    //             await dbContext.Roles.AddAsync(new IdentityRole("Admin"), cancellationToken);
    //         }
    //
    //         await dbContext.SaveChangesAsync(cancellationToken);
    //         await transaction.CommitAsync(cancellationToken);
    //     });
    // }
}