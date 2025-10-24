using Microsoft.Extensions.Configuration;
using OMG.Core.Handler;

namespace OMG.BlazorApp.Client.Services;

/// <summary>
/// Serviço para gerenciamento de feature toggles no Blazor Client
/// </summary>
public class ClientFeatureToggleService : IFeatureToggleHandler
{
    private readonly IConfiguration _configuration;

    public ClientFeatureToggleService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Verifica se o cadastro de usuários está habilitado
    /// </summary>
    public bool IsUserRegistrationEnabled => 
        _configuration.GetValue<bool>("FeatureToggles:EnableUserRegistration", true);
}
