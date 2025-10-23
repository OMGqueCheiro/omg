# Claims Mapping - Correção da Autenticação

## Problema Identificado

Os dados do usuário autenticado não estavam sendo salvos corretamente nos claims. Especificamente, `Identity.Name` retornava `null` mesmo com o usuário autenticado.

## Causa Raiz

O JWT token gerado pela API contém claims customizados (`email`, `userId`, `nome`), mas o Blazor espera claims padrões do tipo `ClaimTypes.Name`, `ClaimTypes.Email`, etc.

### JWT Token Gerado pela API
```csharp
// Em AuthService.cs
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim("userId", user.Id.ToString()),
    new Claim("nome", user.Nome)
};
```

## Solução Implementada

### 1. Mapeamento de Claims em `JwtAuthenticationStateProvider.cs`

Criado o método `CreateIdentityFromToken()` que mapeia os claims do JWT para os claims padrões do ASP.NET:

```csharp
private ClaimsIdentity CreateIdentityFromToken(string jwtToken)
{
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(jwtToken);
    
    var claims = new List<Claim>();
    
    // Mapear email para ClaimTypes.Name (usado por Identity.Name)
    var emailClaim = token.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == JwtRegisteredClaimNames.Email);
    if (emailClaim != null)
    {
        claims.Add(new Claim(ClaimTypes.Name, emailClaim.Value));
        claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
    }
    
    // Mapear nome completo
    var nomeClaim = token.Claims.FirstOrDefault(c => c.Type == "nome");
    if (nomeClaim != null)
    {
        claims.Add(new Claim("nome_completo", nomeClaim.Value));
    }
    
    // Mapear userId para ClaimTypes.NameIdentifier
    var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "userId");
    if (userIdClaim != null)
    {
        claims.Add(new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value));
    }
    
    // Adicionar claims restantes
    foreach (var claim in token.Claims)
    {
        if (!claims.Any(c => c.Type == claim.Type))
        {
            claims.Add(claim);
        }
    }
    
    return new ClaimsIdentity(claims, "jwt");
}
```

### 2. Mapeamento de Claims

| Claim Original (JWT) | Claim Padrão ASP.NET | Uso |
|---------------------|---------------------|-----|
| `email` | `ClaimTypes.Name` | `Identity.Name` |
| `email` | `ClaimTypes.Email` | `User.FindFirst(ClaimTypes.Email)` |
| `userId` | `ClaimTypes.NameIdentifier` | ID único do usuário |
| `nome` | `nome_completo` | Nome completo do usuário |

### 3. Componente `UserInfo.razor`

Criado componente helper para exibir as informações do usuário com fallbacks inteligentes:

```razor
<UserInfo ShowAllClaims="true" />
```

**Recursos:**
- Exibe Email, Nome, User ID e Identity.Name
- Busca claims em múltiplas localizações (fallback)
- Modo debug: `ShowAllClaims="true"` mostra todos os claims
- Design responsivo com MudBlazor

### 4. Métodos Helper no `UserInfo.razor`

```csharp
private string GetUserEmail(AuthenticationState context)
{
    return context.User.FindFirst(ClaimTypes.Email)?.Value
        ?? context.User.FindFirst("email")?.Value
        ?? "N/A";
}

private string GetUserName(AuthenticationState context)
{
    return context.User.FindFirst("nome")?.Value
        ?? context.User.FindFirst("nome_completo")?.Value
        ?? context.User.FindFirst(ClaimTypes.Name)?.Value
        ?? "N/A";
}

private string GetUserId(AuthenticationState context)
{
    return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? context.User.FindFirst("userId")?.Value
        ?? context.User.FindFirst("sub")?.Value
        ?? "N/A";
}
```

## Resultado

### ✅ Agora Funciona

1. **NavMenu.razor**: Exibe `@context.User.Identity?.Name` (email do usuário)
2. **Auth.razor**: Usa `<UserInfo ShowAllClaims="true" />` para debug completo
3. **Qualquer componente**: Pode usar `ClaimTypes.Name`, `ClaimTypes.Email`, etc.

### Como Usar nos Componentes

```razor
<AuthorizeView>
    <Authorized>
        <!-- Exibir email -->
        <p>Email: @context.User.Identity?.Name</p>
        
        <!-- Exibir nome completo -->
        <p>Nome: @context.User.FindFirst("nome_completo")?.Value</p>
        
        <!-- Exibir User ID -->
        <p>ID: @context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value</p>
        
        <!-- Ou usar o componente helper -->
        <UserInfo />
    </Authorized>
</AuthorizeView>
```

## Verificação

Para testar se os claims estão corretos:

1. Faça login na aplicação
2. Acesse `/auth`
3. Verifique:
   - ✅ Email exibido corretamente
   - ✅ Nome exibido corretamente
   - ✅ User ID exibido corretamente
   - ✅ Identity.Name retorna o email
   - ✅ Tabela de claims mostra todos os dados

## Arquivos Modificados

1. **JwtAuthenticationStateProvider.cs**
   - Adicionado método `CreateIdentityFromToken()`
   - Implementado mapeamento de claims
   
2. **_Imports.razor**
   - Adicionado `@using System.Security.Claims`
   - Adicionado `@using OMG.BlazorApp.Client.Shared`

3. **UserInfo.razor** (novo)
   - Componente helper para exibir informações do usuário
   - Suporte a modo debug com todos os claims

4. **Auth.razor**
   - Atualizado para usar `<UserInfo ShowAllClaims="true" />`

## Referências

- [ASP.NET Core ClaimTypes](https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes)
- [JWT Claims](https://www.rfc-editor.org/rfc/rfc7519.html#section-4)
- [Blazor Authentication State](https://docs.microsoft.com/en-us/aspnet/core/blazor/security/)
