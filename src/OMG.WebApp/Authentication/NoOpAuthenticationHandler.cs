using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;

namespace OMG.WebApp.Authentication;

/// <summary>
/// Handler de autenticação "no-op" para satisfazer o pipeline ASP.NET Core.
/// IMPORTANTE: Autenticação REAL é 100% no Client (WASM) via PersistentAuthenticationStateProvider.
/// Este handler apenas evita exceções quando middlewares UseAuthentication/UseAuthorization são chamados.
/// </summary>
public class NoOpAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public NoOpAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Sempre retorna sucesso com identidade anônima
        // O WASM Client gerencia a autenticação real via AuthenticationStateProvider
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // No-op: não redireciona, o WASM gerencia rotas protegidas
        return Task.CompletedTask;
    }
}
