# Configuração do OMG.BlazorApp

## Arquitetura

O projeto OMG.BlazorApp foi configurado seguindo a arquitetura Blazor WebAssembly **puro** (sem prerendering) com os seguintes componentes:

### 1. OMG.BlazorApp (Servidor)
**Responsabilidades:**
- Hospedar a aplicação WebAssembly (servir arquivos estáticos)
- Funcionar como Reverse Proxy (YARP) para a API
- **NÃO** possui lógica de negócio ou acesso a dados
- **NÃO** faz prerendering (tudo roda no cliente)

**Configurações:**
- Remove completamente Identity e DbContext
- Usa apenas WebAssembly hosting
- YARP encaminha requisições `/api/*` para `omg-api`
- Service Discovery via Aspire para resolver a URL da API
- O HTML do App.razor serve apenas o shell inicial (div#app)

**Fluxo de Requisições:**
```
Client (WASM) -> Servidor Blazor (YARP) -> OMG.Api
```

### 2. OMG.BlazorApp.Client (WebAssembly)
**Responsabilidades:**
- Todas as páginas e componentes da interface
- Lógica de apresentação
- Chamadas HTTP para a API (via servidor YARP)
- Executa 100% no navegador do cliente

**Configurações:**
- Referência ao projeto `OMG.Core` para usar ViewModels e Entities
- HttpClient configurado com BaseAddress do servidor
- Handlers registrados para comunicação com a API
- Componente raiz `App` montado em `div#app`
- `HeadOutlet` montado em `head::after`

## Estrutura de Arquivos

```
OMG.BlazorApp/
├── OMG.BlazorApp/ (Servidor)
│   ├── Components/
│   │   └── App.razor         # Root component (WebAssembly)
│   ├── wwwroot/
│   ├── Program.cs             # YARP + WebAssembly hosting
│   └── OMG.BlazorApp.csproj
│
└── OMG.BlazorApp.Client/ (WebAssembly)
    ├── Pages/                 # Todas as páginas
    │   ├── Home.razor
    │   ├── Counter.razor
    │   ├── Weather.razor
    │   └── Auth.razor
    ├── Layout/                # Layouts
    │   ├── MainLayout.razor
    │   └── NavMenu.razor
    ├── Handlers/              # Handlers para API
    │   └── ClienteHandler.cs
    ├── wwwroot/
    ├── App.razor              # HTML root
    ├── Routes.razor           # Routing
    ├── Program.cs             # DI + HttpClient
    └── OMG.BlazorApp.Client.csproj
```

## Como Adicionar Novos Handlers

1. **Criar interface no OMG.Core** (se não existir):
```csharp
// OMG.Core/Handler/IProdutoHandler.cs
public interface IProdutoHandler
{
    Task<Response<Produto>> Create(Produto produto);
    Task<Response<Produto>> Update(Produto produto);
}
```

2. **Criar handler no Client**:
```csharp
// OMG.BlazorApp.Client/Handlers/ProdutoHandler.cs
using OMG.Core.Base;
using OMG.Core.Entities;
using OMG.Core.Handler;
using System.Net.Http.Json;

namespace OMG.BlazorApp.Client.Handlers;

public class ProdutoHandler : IProdutoHandler
{
    private readonly HttpClient _httpClient;

    public ProdutoHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Response<Produto>> Create(Produto produto)
    {
        // Chamada será /api/Produto -> YARP encaminha para OMG.Api
        var response = await _httpClient.PostAsJsonAsync("api/Produto", produto);

        if (response.IsSuccessStatusCode)
            return new Response<Produto>(
                data: await response.Content.ReadFromJsonAsync<Produto>(),
                code: (int)response.StatusCode);

        return new Response<Produto>(
            code: (int)response.StatusCode, 
            message: await response.Content.ReadAsStringAsync());
    }
    
    public async Task<Response<Produto>> Update(Produto produto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Produto/{produto.Id}", produto);

        if (response.IsSuccessStatusCode)
            return new Response<Produto>(
                data: produto,
                code: (int)response.StatusCode);

        return new Response<Produto>(
            code: (int)response.StatusCode, 
            message: await response.Content.ReadAsStringAsync());
    }
}
```

3. **Registrar no Program.cs do Client**:
```csharp
// OMG.BlazorApp.Client/Program.cs
builder.Services.AddScoped<IProdutoHandler, ProdutoHandler>();
```

## Como Usar Handlers nas Páginas

```razor
@page "/produtos"
@inject IProdutoHandler ProdutoHandler

<MudButton OnClick="CriarProduto">Criar Produto</MudButton>

@code {
    private async Task CriarProduto()
    {
        var produto = new Produto { Nome = "Produto Teste" };
        var response = await ProdutoHandler.Create(produto);
        
        if (response.IsSuccess)
        {
            // Sucesso
        }
        else
        {
            // Erro
            var mensagem = response.Message;
        }
    }
}
```

## Fluxo de Dados

```
┌─────────────────────┐
│  BlazorApp.Client   │  (WASM)
│  - Pages            │
│  - Components       │
│  - Handlers         │
└──────────┬──────────┘
           │ HttpClient
           │ /api/Produto
           ▼
┌─────────────────────┐
│  BlazorApp Server   │
│  - YARP Proxy       │
└──────────┬──────────┘
           │ Encaminha
           │ /api/Produto
           ▼
┌─────────────────────┐
│     OMG.Api         │
│  - Controllers      │
│  - Services         │
│  - Repository       │
└─────────────────────┘
```

## Próximos Passos

1. ✅ Estrutura base configurada
2. ✅ YARP configurado
3. ✅ HttpClient configurado
4. ✅ Exemplo de handler criado
5. ⏳ Implementar autenticação (JWT via API)
6. ⏳ Migrar páginas de Account se necessário
7. ⏳ Criar handlers para todas as entidades
8. ⏳ Implementar páginas de CRUD

## Diferenças com OMG.WebApp

- **OMG.WebApp**: Usa Interactive Server + WebAssembly (modo Auto)
- **OMG.BlazorApp**: Usa apenas WebAssembly (modo puro)
- Ambos usam YARP para proxy reverso
- Ambos chamam a mesma OMG.Api
- OMG.BlazorApp é mais leve no servidor (sem SignalR)

## Autenticação e Autorização

### ⚠️ Importante: Client-Side Only

Este projeto usa **Blazor WebAssembly Puro**, portanto:

- ✅ **Autenticação está 100% no Client (WASM)**
- ✅ **Usar `<AuthorizeView>` para controle de UI**
- ❌ **NÃO usar `[Authorize]` attribute** (causaria erro no servidor)
- ❌ **Servidor não tem serviços de autenticação**

### Como Proteger Páginas

**❌ NÃO FAÇA ISSO:**
```razor
@page "/minha-pagina"
@attribute [Authorize]  <!-- ERRO: Servidor não tem autenticação! -->
```

**✅ FAÇA ISSO:**
```razor
@page "/minha-pagina"

<AuthorizeView>
    <Authorized>
        <!-- Conteúdo protegido -->
    </Authorized>
    <NotAuthorized>
        <RedirectToLogin/>
    </NotAuthorized>
</AuthorizeView>
```

### Segurança Real

- 🔒 **A segurança REAL está na API!**
- Client-side auth é apenas para UX (esconder botões, etc)
- API deve validar JWT em TODAS as requisições protegidas
- Nunca confie apenas em validação client-side
