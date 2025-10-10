namespace OMG.Domain.Request;

public record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string? Nome = null
);

public record LoginRequest(
    string Email,
    string Password
);

public record ChangePasswordRequest(
    string Email,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);

public record AuthResponse(
    bool Success,
    string? Token = null,
    string? Message = null,
    DateTime? Expiration = null,
    string? Email = null,
    string? Nome = null
);
