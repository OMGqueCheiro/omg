using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OMG.UserIdentity.Entities;

namespace OMG.UserIdentity.Context;

public class UserIdentityDBContext(DbContextOptions<UserIdentityDBContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Renomear tabelas do Identity para português
        builder.Entity<ApplicationUser>().ToTable("Usuarios");
    }
}
