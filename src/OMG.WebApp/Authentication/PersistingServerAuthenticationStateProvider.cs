using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using System.Security.Claims;

namespace OMG.WebApp.Authentication;

/// <summary>
/// AuthenticationStateProvider para o lado Server (SSR) que persiste o estado para o WASM Client
/// Este provider serializa o estado de autenticação durante a renderização do servidor
/// e o transmite para o cliente durante a hidratação.
/// </summary>
public class PersistingServerAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;
    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingServerAuthenticationStateProvider(PersistentComponentState state)
    {
        _state = state;
        
        // Subscreve ao evento de persistência para serializar o estado
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Retorna o estado de autenticação atual
        return _authenticationStateTask ?? 
               Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask;
        NotifyAuthenticationStateChanged(authenticationStateTask);
    }

    private async Task OnPersistingAsync()
    {
        // Serializa o estado de autenticação para o cliente
        var authenticationState = await GetAuthenticationStateAsync();
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            // Persiste apenas as claims essenciais
            var claims = principal.Claims.Select(c => new SerializableClaim
            {
                Type = c.Type,
                Value = c.Value
            }).ToArray();

            _state.PersistAsJson(nameof(SerializableClaim), claims);
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    /// <summary>
    /// Classe serializável para transferir claims do servidor para o cliente
    /// </summary>
    private class SerializableClaim
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
