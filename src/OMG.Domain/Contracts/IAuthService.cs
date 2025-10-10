using OMG.Domain.Request;

namespace OMG.Domain.Contracts;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request);
}
