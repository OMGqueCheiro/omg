using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OMG.Repository;

public class OMGDbContextFactory : IDesignTimeDbContextFactory<OMGDbContext>
{
    public OMGDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OMGDbContext>();
        
        // Connection string para design-time (migrations)
        // Em runtime, isso será substituído pela connection string do Aspire
        optionsBuilder.UseNpgsql("Host=localhost;Database=OMGdb;Username=postgres;Password=postgres");

        return new OMGDbContext(optionsBuilder.Options);
    }
}
