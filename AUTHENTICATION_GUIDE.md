# Sistema de Autenticação OMG - Guia de Uso

## 📋 Visão Geral

Foi implementado um sistema completo de autenticação JWT (JSON Web Token) no projeto OMG, incluindo:

- ✅ Registro de novos usuários
- ✅ Login com geração de token JWT
- ✅ Alteração de senha
- ✅ Proteção de todas as rotas da API
- ✅ Interface de usuário completa no Blazor WebApp
- ✅ Gerenciamento automático de tokens nas requisições HTTP

## 🏗️ Arquitetura Implementada

### 1. **OMG.UserIdentity** - Camada de Identidade
- **`ApplicationUser`**: Entidade customizada que estende `IdentityUser`
- **`UserIdentityDBContext`**: DbContext do Identity usando SQL Server
- **`UserIdentityDI`**: Configuração de DI do Identity com políticas de senha

### 2. **OMG.Domain** - Lógica de Negócio
- **`AuthService`**: Serviço que gerencia autenticação e geração de JWT
- **`IAuthService`**: Contrato do serviço de autenticação
- **DTOs de Autenticação**:
  - `RegisterRequest`: Registro de usuário
  - `LoginRequest`: Login
  - `ChangePasswordRequest`: Alteração de senha
  - `AuthResponse`: Resposta unificada com token JWT

### 3. **OMG.Api** - Endpoints REST
- **`AuthController`**: Rotas de autenticação
  - `POST /api/auth/register`: Cadastro
  - `POST /api/auth/login`: Login (retorna JWT)
  - `POST /api/auth/change-password`: Alterar senha (requer autenticação)
  - `GET /api/auth/me`: Obter usuário atual (requer autenticação)
- **Proteção Global**: Todos os controllers existentes agora exigem autenticação via `[Authorize]`

### 4. **OMG.WebApp** - Interface de Usuário
- **Páginas**:
  - `/login`: Tela de login
  - `/register`: Tela de cadastro
  - `/change-password`: Tela de alteração de senha
- **`AuthHandler`**: Gerencia chamadas HTTP de autenticação
- **`CustomAuthenticationStateProvider`**: Gerencia estado de autenticação no Blazor
- **`AuthorizationMessageHandler`**: Adiciona token JWT automaticamente em todas as requisições
- **Layout com Login/Logout**: Barra superior mostra email do usuário e botão de logout

## 🚀 Como Usar

### Primeira Execução

1. **Iniciar o Aspire** (já está rodando):
```bash
aspire run
```

2. **O sistema criará automaticamente**:
   - Banco de dados `OMG` (pedidos, clientes, produtos)
   - Banco de dados de Identity (usuários, roles, claims)

### Fluxo de Uso

#### 1️⃣ **Cadastro de Novo Usuário**
1. Acesse `https://localhost:porta/register`
2. Preencha:
   - Email (obrigatório, deve ser único)
   - Nome (opcional)
   - Senha (mínimo 6 caracteres, deve conter maiúscula, minúscula e número)
   - Confirmação de senha
3. Clique em "Cadastrar"
4. Após sucesso, você será redirecionado para o login

#### 2️⃣ **Login**
1. Acesse `https://localhost:porta/login`
2. Digite email e senha
3. Ao fazer login com sucesso:
   - Um token JWT é gerado (válido por 8 horas)
   - Token é armazenado no LocalStorage do navegador
   - Você é redirecionado para a home
   - O email aparece na barra superior

#### 3️⃣ **Acessando o Sistema**
- Todas as páginas agora exigem autenticação
- Se não estiver logado, será redirecionado para `/login`
- O token é adicionado automaticamente em todas as requisições à API

#### 4️⃣ **Alteração de Senha**
1. Acesse `https://localhost:porta/change-password` (deve estar logado)
2. Digite:
   - Senha atual
   - Nova senha
   - Confirmação da nova senha
3. Clique em "Alterar Senha"

#### 5️⃣ **Logout**
- Clique no ícone de logout (🚪) na barra superior
- Token é removido do LocalStorage
- Você é redirecionado para a tela de login

## 🔐 Configurações de Segurança

### Políticas de Senha (configuradas em `UserIdentityDI.cs`)
- Mínimo 6 caracteres
- Requer pelo menos 1 letra maiúscula
- Requer pelo menos 1 letra minúscula
- Requer pelo menos 1 dígito
- Não requer caractere especial
- Email deve ser único

### Bloqueio de Conta
- Após 5 tentativas de login falhas
- Conta bloqueada por 15 minutos

### Token JWT
- **Emissor (Issuer)**: `OMG.Api`
- **Audiência (Audience)**: `OMG.WebApp`
- **Validade**: 8 horas
- **Chave Secreta**: Configurada em `appsettings.json`

⚠️ **IMPORTANTE**: Em produção, altere a chave secreta em `appsettings.Production.json` e use um valor mais seguro armazenado em variáveis de ambiente ou Azure Key Vault.

## 📁 Arquivos Criados/Modificados

### Novos Arquivos

**OMG.UserIdentity:**
- `Entities/ApplicationUser.cs`
- `Context/UserIdentityDBContext.cs`
- `UserIdentityDI.cs`

**OMG.Domain:**
- `Request/AuthRequest.cs`
- `Contracts/IAuthService.cs`
- `Services/AuthService.cs`

**OMG.Api:**
- `Controllers/AuthController.cs`

**OMG.WebApp:**
- `Handler/IAuthHandler.cs`
- `Handler/AuthHandler.cs`
- `Authentication/CustomAuthenticationStateProvider.cs`
- `Authentication/AuthorizationMessageHandler.cs`
- `Components/Pages/Login.razor`
- `Components/Pages/Register.razor`
- `Components/Pages/ChangePassword.razor`
- `Components/RedirectToLogin.razor`

### Arquivos Modificados

**OMG.Api:**
- `Program.cs`: Configuração de JWT e Identity
- `appsettings.json`: Adicionadas configurações JWT
- `Controllers/BaseCRUDController.cs`: Adicionado `[Authorize]`
- `Controllers/ViewController.cs`: Adicionado `[Authorize]`

**OMG.WebApp:**
- `Program.cs`: Configuração de autenticação e LocalStorage
- `Components/Routes.razor`: Suporte a `AuthorizeRouteView`
- `Components/_Imports.razor`: Importações de Authorization
- `Components/Layout/MainLayout.razor`: Botão de login/logout

**OMG.Domain:**
- `DomainDI.cs`: Registro de `AuthService`

## 🧪 Testando a API com Swagger

1. Acesse o Swagger da API: `https://localhost:porta-api/swagger`
2. **Registrar usuário**: Use `POST /api/auth/register`
3. **Fazer login**: Use `POST /api/auth/login` e copie o `token` da resposta
4. **Autorizar no Swagger**: 
   - Clique no botão "Authorize" no topo
   - Digite: `Bearer SEU_TOKEN_AQUI`
   - Clique em "Authorize"
5. Agora você pode testar os endpoints protegidos

## 🔄 Próximos Passos Sugeridos

1. **Roles e Permissões**: Implementar diferentes níveis de acesso (Admin, User, etc.)
2. **Refresh Token**: Implementar renovação automática de tokens
3. **Recuperação de Senha**: Implementar "Esqueci minha senha" com email
4. **Two-Factor Authentication (2FA)**: Adicionar autenticação de dois fatores
5. **Auditoria**: Registrar logins, tentativas de acesso, etc.
6. **Testes Unitários**: Criar testes para `AuthService` e `AuthController`

## 📝 Notas Técnicas

- O token JWT contém claims: email, userId, nome
- O `AuthorizationMessageHandler` injeta o token automaticamente em todas as requisições HTTP
- O `CustomAuthenticationStateProvider` mantém o estado de autenticação sincronizado
- Blazored.LocalStorage é usado para persistir o token no navegador
- A conexão entre API e WebApp usa service discovery do Aspire

## 🐛 Troubleshooting

**Problema**: "Não autorizado" mesmo após login
- **Solução**: Verifique se o token está sendo salvo no LocalStorage (F12 > Application > Local Storage)

**Problema**: Token expirado
- **Solução**: Faça login novamente (validade de 8 horas)

**Problema**: Erro ao criar usuário
- **Solução**: Verifique se a senha atende aos requisitos (maiúscula, minúscula, número)

**Problema**: Não consegue acessar a API
- **Solução**: Verifique se os serviços Aspire estão rodando corretamente
