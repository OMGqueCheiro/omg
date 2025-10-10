using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OMG.UserIdentity.Context;

public class UserIdentityDBContextFactory : IDesignTimeDbContextFactory<UserIdentityDBContext>
{
    public UserIdentityDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserIdentityDBContext>();
        
        // Connection string para design-time (migrations)
        // Em runtime, isso será substituído pela connection string do Aspire
        optionsBuilder.UseSqlServer("Server=localhost;Database=OMGdb;User Id=sa;Password=Pass@word;TrustServerCertificate=True");

        return new UserIdentityDBContext(optionsBuilder.Options);
    }
}
