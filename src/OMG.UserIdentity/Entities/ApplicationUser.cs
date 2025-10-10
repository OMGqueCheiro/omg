using Microsoft.AspNetCore.Identity;

namespace OMG.UserIdentity.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Nome { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoAcesso { get; set; }
}
