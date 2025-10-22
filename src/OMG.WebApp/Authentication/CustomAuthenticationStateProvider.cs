using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OMG.WebApp.Authentication;

public class CustomAuthenticationStateProvider(
    HybridAuthenticationService authStateService,
    ILogger<CustomAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    private readonly HybridAuthenticationService _authStateService = authStateService;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger = logger;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _logger.LogInformation("🔍 GetAuthenticationStateAsync - Verificando token");
        
        var token = _authStateService.GetToken();
        
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("⚠️ Sem token, retornando usuário anônimo");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        _logger.LogInformation("✅ Token encontrado (tamanho: {Length})", token.Length);
        
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation("👤 Usuário autenticado: {Email}", user.FindFirst(ClaimTypes.Email)?.Value ?? "N/A");
        
        return Task.FromResult(new AuthenticationState(user));
    }

    public async Task NotifyUserAuthentication(string token)
    {
        _logger.LogInformation("🔔 NotifyUserAuthentication - Token recebido (tamanho: {Length})", token.Length);
        
        await _authStateService.SetTokenAsync(token);
        _logger.LogInformation("💾 Token salvo");
        
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _logger.LogInformation("📢 Notificando mudança de estado de autenticação");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        
        _logger.LogInformation("✅ Autenticação completada");
    }

    public async Task NotifyUserLogout()
    {
        _logger.LogInformation("🚪 Logout iniciado");
        
        await _authStateService.ClearTokenAsync();
        
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        
        _logger.LogInformation("✅ Logout completado");
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
