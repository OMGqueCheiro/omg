namespace OMG.WebApp.Authentication;

/// <summary>
/// Serviço de estado de autenticação no servidor (Scoped ao circuito Blazor)
/// Não depende de JavaScript, mantém token em memória durante a sessão
/// </summary>
public class AuthenticationStateService
{
    private string? _token;

    public string? Token
    {
        get => _token;
        set => _token = value;
    }

    public bool HasToken => !string.IsNullOrEmpty(_token);

    public void ClearToken() => _token = null;
}
