using OMG.Domain.Request;

namespace OMG.WebApp.Handler;

public interface IAuthHandler
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request);
    Task<AuthResponse?> GetCurrentUserAsync();
    Task LogoutAsync();
    Task<string?> GetStoredToken();
}
