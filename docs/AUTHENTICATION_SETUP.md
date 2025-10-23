# Sistema de Autenticação - OMG.BlazorApp

## 📋 Visão Geral

Sistema completo de autenticação JWT implementado no OMG.BlazorApp (Blazor WebAssembly), com armazenamento seguro de token no localStorage e injeção automática em requisições HTTP.

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                  Cliente (WASM)                         │
│                                                         │
│  ┌─────────────┐    ┌──────────────────────┐          │
│  │   Páginas   │───▶│  AuthHandler         │          │
│  │ Login/      │    │  - LoginAsync()      │          │
│  │ Register    │    │  - RegisterAsync()   │          │
│  └─────────────┘    │  - LogoutAsync()     │          │
│                     └──────────┬───────────┘          │
│                                │                       │
│                                ▼                       │
│              ┌─────────────────────────────┐          │
│              │ JwtAuthenticationState      │          │
│              │ Provider                    │          │
│              │ - Gerencia token           │          │
│              │ - localStorage             │          │
│              │ - Valida JWT               │          │
│              │ - Injeta em HttpClient     │          │
│              └─────────────┬───────────────┘          │
│                            │                           │
│                            ▼                           │
│                    ┌───────────────┐                  │
│                    │  LocalStorage │                  │
│                    │  authToken    │                  │
│                    └───────────────┘                  │
└─────────────────────────────────────────────────────────┘
                             │
                             │ HTTP + JWT Bearer Token
                             ▼
                    ┌───────────────┐
                    │  Servidor     │
                    │  YARP Proxy   │
                    └───────┬───────┘
                             │
                             ▼
                    ┌───────────────┐
                    │   OMG.Api     │
                    │ /api/auth/*   │
                    └───────────────┘
```

## 🔑 Componentes Principais

### 1. JwtAuthenticationStateProvider

**Localização:** `OMG.BlazorApp.Client/Authentication/JwtAuthenticationStateProvider.cs`

**Responsabilidades:**
- Gerencia o estado de autenticação do usuário
- Salva/recupera token JWT do localStorage
- Valida expiração do token
- Injeta token no header Authorization do HttpClient
- Notifica mudanças no estado de autenticação

**Métodos:**
```csharp
// Obtém estado atual de autenticação
Task<AuthenticationState> GetAuthenticationStateAsync()

// Notifica que usuário fez login (salva token)
Task NotifyUserAuthentication(string token)

// Notifica que usuário fez logout (remove token)
Task NotifyUserLogout()

// Obtém token armazenado
Task<string?> GetTokenAsync()
```

### 2. AuthHandler

**Localização:** `OMG.BlazorApp.Client/Handlers/AuthHandler.cs`

**Responsabilidades:**
- Faz chamadas à API de autenticação
- Processa login e registro
- Armazena token após login bem-sucedido

**Métodos:**
```csharp
// Faz login e salva token
Task<AuthResponse> LoginAsync(LoginRequest request)

// Registra novo usuário
Task<AuthResponse> RegisterAsync(RegisterRequest request)

// Altera senha
Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request)

// Obtém usuário atual
Task<AuthResponse?> GetCurrentUserAsync()

// Faz logout
Task LogoutAsync()

// Obtém token armazenado
Task<string?> GetStoredToken()
```

## 📄 Páginas de Autenticação

### Login (`/login`)

**Recursos:**
- Formulário com validação (email e senha)
- Feedback visual durante carregamento
- Mensagens de erro/sucesso com Snackbar
- Redirecionamento automático após login

**Fluxo:**
1. Usuário preenche email e senha
2. Clica em "Entrar"
3. AuthHandler chama `/api/auth/login`
4. API valida credenciais e retorna JWT
5. Token salvo no localStorage
6. HttpClient configurado com token
7. Redireciona para home

### Register (`/register`)

**Recursos:**
- Formulário completo com validações
- Confirmação de senha
- Nome opcional
- Validação de formato de email
- Validação de força de senha (mínimo 6 caracteres)

**Fluxo:**
1. Usuário preenche dados
2. Clica em "Criar Conta"
3. AuthHandler chama `/api/auth/register`
4. API cria usuário
5. Redireciona para login

## 🔐 Como o Token é Gerenciado

### 1. Armazenamento

```csharp
// Token salvo no localStorage com chave "authToken"
await _localStorage.SetItemAsync("authToken", token);
```

### 2. Validação

```csharp
// Ao carregar, verifica se token expirou
var jwtToken = handler.ReadJwtToken(token);
if (jwtToken.ValidTo < DateTime.UtcNow)
{
    // Token expirado - remove
    await _localStorage.RemoveItemAsync("authToken");
}
```

### 3. Injeção Automática

```csharp
// Token adicionado automaticamente em TODAS as requisições
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);
```

### 4. Logout

```csharp
// Remove token e limpa HttpClient
await _localStorage.RemoveItemAsync("authToken");
_httpClient.DefaultRequestHeaders.Authorization = null;
```

## 🛠️ Configuração (Program.cs)

```csharp
// LocalStorage para salvar token
builder.Services.AddBlazoredLocalStorage();

// Autenticação
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// AuthenticationStateProvider customizado
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>(sp => 
    (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

// Handler de autenticação
builder.Services.AddScoped<IAuthHandler, AuthHandler>();
```

## 📦 Pacotes NuGet Necessários

```xml
<PackageReference Include="Blazored.LocalStorage" Version="4.*"/>
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.*"/>
<PackageReference Include="Microsoft.AspNetCore.Components.Authorization" Version="9.*"/>
```

## 🎯 Como Usar em Componentes

### Verificar se Usuário Está Autenticado

```razor
<AuthorizeView>
    <Authorized>
        <p>Olá, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>Faça login para continuar.</p>
    </NotAuthorized>
</AuthorizeView>
```

### Obter Token Atual

```csharp
@inject IAuthHandler AuthHandler

private async Task MinhaFuncao()
{
    var token = await AuthHandler.GetStoredToken();
    if (!string.IsNullOrEmpty(token))
    {
        // Token disponível
    }
}
```

### Fazer Logout

```csharp
@inject IAuthHandler AuthHandler
@inject NavigationManager Navigation

private async Task Logout()
{
    await AuthHandler.LogoutAsync();
    Navigation.NavigateTo("/", forceLoad: true);
}
```

## 🔒 Protegendo Páginas

### ⚠️ IMPORTANTE: Blazor WebAssembly Puro

Como este é um projeto **Blazor WebAssembly PURO** (não híbrido), você **NÃO pode usar** o atributo `[Authorize]` em páginas, pois isso tentaria processar a autorização no servidor, que não tem serviços de autenticação.

### ❌ NÃO FAÇA ISSO:

```razor
@page "/admin"
@attribute [Authorize]  <!-- ERRO: Servidor não suporta! -->

<h3>Página Protegida</h3>
```

**Erro resultante:**
```
InvalidOperationException: Unable to find the required 'IAuthenticationService' service
```

### ✅ FAÇA ISSO - Opção 1: AuthorizeView

```razor
@page "/admin"

<AuthorizeView>
    <Authorized>
        <h3>Conteúdo Protegido</h3>
        <p>Olá, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <RedirectToLogin/>
    </NotAuthorized>
</AuthorizeView>
```

### ✅ FAÇA ISSO - Opção 2: Verificação Manual

```razor
@page "/admin"
@inject AuthenticationStateProvider AuthStateProvider
@inject NavigationManager Navigation

@if (isAuthenticated)
{
    <h3>Conteúdo Protegido</h3>
}
else
{
    <RedirectToLogin/>
}

@code {
    private bool isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
        
        if (!isAuthenticated)
        {
            Navigation.NavigateTo("/login");
        }
    }
}
```

### 🛡️ Por que não usar [Authorize]?

1. **Servidor não tem autenticação** - É apenas um host estático + YARP proxy
2. **Evita prerendering** - Mantém arquitetura WASM pura
3. **Sem overhead** - Servidor não processa lógica de autorização
4. **Mais eficiente** - Autenticação acontece só no client

### 🔐 Segurança Real

**IMPORTANTE:** A proteção client-side é apenas para UX!

- ✅ **Segurança REAL está na API** - Valida JWT em cada request
- ✅ **API retorna 401** se token inválido/expirado
- ✅ **Client-side apenas esconde UI** - Não protege dados
- ⚠️ **Nunca confie apenas em validação client-side**

Exemplo de proteção na API:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // ← Aqui SIM! API valida JWT
public class ClienteController : ControllerBase
{
    // Endpoints protegidos
}
```

## 🚀 Requests Autenticadas Automáticas

Uma vez que o usuário está logado, **TODAS as requisições HTTP** incluem automaticamente o token JWT:

```csharp
// Exemplo: ClienteHandler
public async Task<Response<Cliente>> Create(Cliente cliente)
{
    // Token JWT é automaticamente incluído no header Authorization!
    var response = await _httpClient.PostAsJsonAsync("api/Cliente", cliente);
    // ...
}
```

## 📝 Fluxo Completo de Autenticação

1. **Usuário acessa `/login`**
2. **Preenche credenciais e submete**
3. **AuthHandler.LoginAsync()** → `/api/auth/login`
4. **API valida** → Retorna JWT + dados do usuário
5. **JwtAuthenticationStateProvider.NotifyUserAuthentication()**
   - Salva token no localStorage
   - Parseia claims do JWT
   - Configura HttpClient com Bearer token
   - Notifica mudança de estado
6. **Aplicação redireciona** para home
7. **Todas as requisições subsequentes** incluem token
8. **Ao recarregar página:**
   - GetAuthenticationStateAsync() lê token do localStorage
   - Valida se não expirou
   - Reconfigura HttpClient com token
   - Restaura estado de autenticação

## 🔍 Depuração

O `JwtAuthenticationStateProvider` pode incluir logs para debug (já implementados mas comentados):

```csharp
Console.WriteLine($"🔑 [AUTH] Token encontrado: {token.Length} caracteres");
Console.WriteLine($"👤 [AUTH] Claims: {claims.Count}");
Console.WriteLine($"✅ [AUTH] Token salvo com sucesso!");
```

## ⚠️ Segurança

- ✅ Token armazenado no localStorage (persiste entre sessões)
- ✅ Validação de expiração automática
- ✅ Token removido em caso de erro de parsing
- ✅ HTTPS obrigatório em produção
- ✅ Token nunca exposto em logs (exceto debug mode)
- ⚠️ LocalStorage é acessível via JavaScript (considere usar HttpOnly cookies para maior segurança em produção)

## 🎨 UI/UX

- ✅ Feedback visual durante carregamento (spinners)
- ✅ Mensagens de erro/sucesso (Snackbar)
- ✅ Validação de formulários em tempo real
- ✅ Redirecionamentos automáticos
- ✅ Menu de navegação com Login/Logout condicional
- ✅ Exibição do nome do usuário quando logado
