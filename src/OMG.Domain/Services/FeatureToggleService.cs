using Microsoft.Extensions.Options;
using OMG.Core.Base;
using OMG.Core.Handler;

namespace OMG.Domain.Services;

/// <summary>
/// Serviço para gerenciamento de feature toggles
/// </summary>
public class FeatureToggleService : IFeatureToggleHandler
{
    private readonly FeatureToggles _featureToggles;

    public FeatureToggleService(IOptions<FeatureToggles> featureToggles)
    {
        _featureToggles = featureToggles.Value;
    }

    /// <summary>
    /// Verifica se o cadastro de usuários está habilitado
    /// </summary>
    public bool IsUserRegistrationEnabled => _featureToggles.EnableUserRegistration;
}
