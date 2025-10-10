using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OMG.WebApp.Authentication;

public class CustomAuthenticationStateProvider(
    AuthenticationStateService authStateService,
    ILogger<CustomAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private readonly AuthenticationStateService _authStateService = authStateService;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger = logger;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogInformation("🔍 GetAuthenticationStateAsync - HasToken: {HasToken}", _authStateService.HasToken);
        
        if (!_authStateService.HasToken)
        {
            _logger.LogWarning("⚠️ Sem token, retornando usuário anônimo");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        _logger.LogInformation("✅ Token encontrado (tamanho: {Length})", _authStateService.Token!.Length);
        
        var claims = ParseClaimsFromJwt(_authStateService.Token!);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation("👤 Usuário autenticado: {Email}", user.FindFirst(ClaimTypes.Email)?.Value ?? "N/A");
        
        return Task.FromResult(new AuthenticationState(user));
    }

    public Task NotifyUserAuthentication(string token)
    {
        _logger.LogInformation("🔔 NotifyUserAuthentication - Token recebido (tamanho: {Length})", token.Length);
        
        _authStateService.Token = token;
        _logger.LogInformation("💾 Token salvo no AuthenticationStateService");
        
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation("📢 Notificando mudança de estado de autenticação");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        
        _logger.LogInformation("✅ Autenticação completada");
        
        return Task.CompletedTask;
    }

    public Task NotifyUserLogout()
    {
        _logger.LogInformation("🚪 Logout iniciado");
        
        _authStateService.ClearToken();
        
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        
        _logger.LogInformation("✅ Logout completado");
        
        return Task.CompletedTask;
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        _logger.LogDebug("🔐 Parseando claims do JWT");
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        
        _logger.LogDebug("📋 {ClaimCount} claims encontradas", token.Claims.Count());
        
        return token.Claims;
    }
}
