# 🔀 Feature Toggle - Cadastro de Usuários

Sistema de feature toggle implementado para controlar o cadastro de novos usuários no OMG.

## 📋 Como Funciona

### Backend (API)

- **Endpoint protegido**: `/api/auth/register`
- **Comportamento**: Quando desabilitado, retorna HTTP 404 com mensagem de erro
- **Configuração**: `appsettings.json` → `FeatureToggles.EnableUserRegistration`

### Frontend (Blazor)

- **Botão de cadastro**: Oculto na página de login quando desabilitado
- **Página de registro**: Mostra mensagem informativa quando acessada diretamente
- **Configuração**: `wwwroot/appsettings.json` → `FeatureToggles.EnableUserRegistration`

## ⚙️ Configuração

### Desenvolvimento Local

Edite os arquivos `appsettings.json`:

**API**: `src/OMG.Api/appsettings.json`
```json
{
  "FeatureToggles": {
    "EnableUserRegistration": true
  }
}
```

**Blazor**: `src/OMG.BlazorApp/OMG.BlazorApp.Client/wwwroot/appsettings.json`
```json
{
  "FeatureToggles": {
    "EnableUserRegistration": true
  }
}
```

### Produção

A feature toggle em produção é controlada por variável de ambiente no `docker-compose.prod.yml`:

```yaml
api:
  environment:
    - FeatureToggles__EnableUserRegistration=${ENABLE_USER_REGISTRATION:-false}
```

**Padrão**: `false` (cadastro desabilitado)

#### Habilitar Cadastro em Produção

1. **Via arquivo .env** (recomendado):
```bash
# Na VM, crie/edite o arquivo /opt/omg/.env
echo "ENABLE_USER_REGISTRATION=true" >> /opt/omg/.env
```

2. **Via variável de ambiente diretamente**:
```bash
export ENABLE_USER_REGISTRATION=true
```

3. **Aplicar as mudanças**:
```bash
cd /opt/omg
docker compose down
docker compose up -d
```

## 🎯 Casos de Uso

### ✅ Cadastro Habilitado (`true`)

- ✅ Botão "Não tem conta? Cadastre-se" visível na tela de login
- ✅ Página `/register` funcional
- ✅ Endpoint `/api/auth/register` aceita requisições

### ❌ Cadastro Desabilitado (`false`)

- ❌ Botão de cadastro oculto na tela de login
- ⚠️ Página `/register` mostra mensagem: "Cadastro Desabilitado"
- ⚠️ Endpoint `/api/auth/register` retorna 404

## 🧪 Testes

### Testar Feature Toggle

```bash
# 1. Desabilitar cadastro
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@test.com","password":"Test123!","confirmPassword":"Test123!"}'

# Resposta esperada (se desabilitado):
# 404 Not Found
# {"message": "Cadastro de usuários está desabilitado no momento."}
```

## 🔐 Segurança

- A feature toggle **protege tanto o frontend quanto o backend**
- Mesmo que alguém tente acessar `/register` ou chamar a API diretamente, será bloqueado
- A configuração em produção é controlada por variável de ambiente (não está hardcoded)

## 📝 Estrutura de Código

### Arquivos Criados/Modificados

```
src/OMG.Core/
├── Base/FeatureToggles.cs              # Modelo de configuração
└── Handler/IFeatureToggleHandler.cs    # Interface do serviço

src/OMG.Domain/
└── Services/FeatureToggleService.cs    # Implementação para API

src/OMG.BlazorApp/OMG.BlazorApp.Client/
└── Services/ClientFeatureToggleService.cs  # Implementação para Blazor

src/OMG.Api/
├── Controllers/AuthController.cs       # Validação no endpoint
├── Program.cs                          # Configuração DI
└── appsettings.json                    # Config desenvolvimento
└── appsettings.Production.json         # Config produção (default: false)

src/OMG.BlazorApp/OMG.BlazorApp.Client/
├── Pages/Login.razor                   # Esconde botão
├── Pages/Register.razor                # Bloqueia acesso
├── Program.cs                          # Configuração DI
└── wwwroot/appsettings.json           # Config cliente

docker-compose.prod.yml                 # Variável de ambiente
```

## 💡 Dicas

1. **Primeiro deploy**: Mantenha desabilitado até validar o ambiente
2. **Migração de dados**: Habilite temporariamente para criar usuários iniciais
3. **Manutenção**: Desabilite durante atualizações críticas
4. **Auditoria**: Monitore logs quando habilitar/desabilitar

## 🆘 Troubleshooting

### Feature não está funcionando

```bash
# Verificar configuração da API
docker exec omg-api printenv | grep FEATURE

# Verificar logs
docker compose logs api | grep -i feature
```

### Mudança não surtiu efeito

```bash
# Restart completo
cd /opt/omg
docker compose restart api blazorapp

# Ou force rebuild (se mudou config interna)
docker compose down
docker compose up -d
```

---

**Nota**: Em produção, a feature está **desabilitada por padrão** para maior segurança. Habilite apenas quando necessário.
