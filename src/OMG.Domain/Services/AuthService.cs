using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OMG.Domain.Contracts;
using OMG.Domain.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OMG.Domain.Services;

public class AuthService(
    UserManager<OMG.UserIdentity.Entities.ApplicationUser> userManager,
    SignInManager<OMG.UserIdentity.Entities.ApplicationUser> signInManager,
    IConfiguration configuration) : IAuthService
{
    private readonly UserManager<OMG.UserIdentity.Entities.ApplicationUser> _userManager = userManager;
    private readonly SignInManager<OMG.UserIdentity.Entities.ApplicationUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return new AuthResponse(false, Message: "As senhas não coincidem.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse(false, Message: "Email já está em uso.");
        }

        var user = new OMG.UserIdentity.Entities.ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            Nome = request.Nome,
            DataCriacao = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse(false, Message: $"Erro ao criar usuário: {errors}");
        }

        return new AuthResponse(true, Message: "Usuário criado com sucesso!", Email: user.Email, Nome: user.Nome);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse(false, Message: "Email ou senha inválidos.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return new AuthResponse(false, Message: "Conta bloqueada devido a múltiplas tentativas de login. Tente novamente mais tarde.");
        }

        if (!result.Succeeded)
        {
            return new AuthResponse(false, Message: "Email ou senha inválidos.");
        }

        // Atualizar último acesso
        user.UltimoAcesso = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Gerar token JWT
        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddHours(8);

        return new AuthResponse(
            true, 
            Token: token, 
            Message: "Login realizado com sucesso!", 
            Expiration: expiration,
            Email: user.Email,
            Nome: user.Nome
        );
    }

    public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return new AuthResponse(false, Message: "As senhas não coincidem.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse(false, Message: "Usuário não encontrado.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse(false, Message: $"Erro ao alterar senha: {errors}");
        }

        return new AuthResponse(true, Message: "Senha alterada com sucesso!");
    }

    private string GenerateJwtToken(OMG.UserIdentity.Entities.ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey não configurado");
        var issuer = jwtSettings["Issuer"] ?? "OMG.Api";
        var audience = jwtSettings["Audience"] ?? "OMG.WebApp";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id),
            new Claim("nome", user.Nome ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
