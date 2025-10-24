namespace OMG.Core.Handler;

/// <summary>
/// Interface para gerenciamento de feature toggles
/// </summary>
public interface IFeatureToggleHandler
{
    /// <summary>
    /// Verifica se o cadastro de usuários está habilitado
    /// </summary>
    bool IsUserRegistrationEnabled { get; }
}
