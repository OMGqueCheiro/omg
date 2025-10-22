using OMG.Core.Request;

namespace OMG.Core.Handler;

public interface IAuthHandler
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request);
    Task<AuthResponse?> GetCurrentUserAsync();
    Task LogoutAsync();
    Task<string?> GetStoredToken();
}
