# Migração SQL Server para PostgreSQL

## Resumo das Alterações

Este documento descreve a migração completa do SQL Server para PostgreSQL no projeto OMG, mantendo a integração com .NET Aspire para service discovery.

## Alterações Realizadas

### 1. AppHost (OMG.AppHost)

**Arquivo**: `src/OMG.Aspire/OMG.AppHost/Program.cs`

- ✅ Substituído `AddSqlServer` por `AddPostgres`
- ✅ Adicionado volume persistente `omg-postgres-data` para dados do PostgreSQL
- ✅ Adicionado container PgWeb na porta 8081 para gerenciamento visual do banco
- ✅ Mantido o mesmo fluxo de dependências com migrations e API

**Arquivo**: `src/OMG.Aspire/OMG.AppHost/OMG.AppHost.csproj`

- ✅ Substituído `Aspire.Hosting.SqlServer` por `Aspire.Hosting.PostgreSQL`

### 2. OMG.Repository

**Arquivo**: `src/OMG.Repository/OMG.Repository.csproj`

Pacotes substituídos:
- ❌ `Aspire.Microsoft.EntityFrameworkCore.SqlServer`
- ❌ `Microsoft.EntityFrameworkCore.SqlServer`
- ✅ `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` (9.5.1)
- ✅ `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)

**Arquivo**: `src/OMG.Infrastructure/DI/RepositoryDI.cs`

- ✅ Substituído `AddSqlServerDbContext` por `AddNpgsqlDbContext`
- ✅ Atualizado comentário para mencionar "Aspire PostgreSQL integration"

**Arquivo**: `src/OMG.Repository/OMGDbContextFactory.cs` (NOVO)

- ✅ Criado factory para suportar migrations em design-time
- ✅ Connection string padrão: `Host=localhost;Database=OMGdb;Username=postgres;Password=postgres`

### 3. OMG.UserIdentity

**Arquivo**: `src/OMG.UserIdentity/OMG.UserIdentity.csproj`

Pacotes substituídos:
- ❌ `Microsoft.EntityFrameworkCore.SqlServer`
- ✅ `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.4)

**Arquivo**: `src/OMG.Infrastructure/DI/UserIdentityDI.cs`

- ✅ Substituído `UseSqlServer` por `UseNpgsql`

**Arquivo**: `src/OMG.UserIdentity/Context/UserIdentityDBContextFactory.cs`

- ✅ Atualizado para usar `UseNpgsql`
- ✅ Connection string padrão: `Host=localhost;Database=OMGdb;Username=postgres;Password=postgres`

### 4. Migrations

- ✅ Removidas todas as migrations antigas do SQL Server
- ✅ Criada nova migration `InitialPostgreSQL` para OMG.Repository
- ✅ Criada nova migration `InitialIdentityPostgreSQL` para OMG.UserIdentity

## Configuração do PostgreSQL via Aspire

### Volume Persistente

O PostgreSQL está configurado com um volume Docker nomeado `omg-postgres-data`, garantindo que os dados persistam entre reinicializações dos containers.

```csharp
var postgres = builder.AddPostgres("postgres-db-omg")
                      .WithImageTag("17-alpine")
                      .WithDataVolume("omg-postgres-data")
                      .WithLifetime(ContainerLifetime.Persistent);
```

### PgWeb - Interface Web

PgWeb está disponível para gerenciamento visual do banco de dados:
- **URL**: http://localhost:8081 (ou porta atribuída dinamicamente pelo Aspire)
- **Container**: `sosedoff/pgweb`

Para conectar ao PostgreSQL pelo PgWeb, use as credenciais fornecidas pelo Aspire Dashboard.

## Como Executar

### 1. Limpar containers e volumes anteriores (opcional)

```bash
docker stop $(docker ps -q --filter ancestor=mcr.microsoft.com/mssql/server:2022-latest)
docker rm $(docker ps -aq --filter ancestor=mcr.microsoft.com/mssql/server:2022-latest)
```

### 2. Executar o AppHost

```bash
cd src/OMG.Aspire/OMG.AppHost
dotnet run
```

O Aspire irá:
1. Criar e iniciar o container PostgreSQL com volume persistente
2. Criar e iniciar o container PgWeb
3. Executar as migrations automaticamente via MigrationWorker
4. Iniciar a API
5. Iniciar o BlazorApp

### 3. Acessar o Aspire Dashboard

O dashboard mostrará:
- PostgreSQL (postgres-db-omg)
- PgWeb
- API
- BlazorApp

### 4. Connection Strings

Em runtime, as connection strings são fornecidas automaticamente pelo Aspire via service discovery. O formato será:

```
Host=postgres-db-omg;Database=OMGdb;Username=postgres;Password=<gerado-pelo-aspire>
```

## Diferenças entre SQL Server e PostgreSQL

### Principais mudanças:

1. **Sintaxe SQL**: PostgreSQL usa sintaxe padrão SQL, mais rigorosa que SQL Server
2. **Tipos de dados**: 
   - `nvarchar(max)` → `text`
   - `datetime2` → `timestamp with time zone`
3. **Case Sensitivity**: PostgreSQL é case-sensitive para nomes de objetos
4. **Schemas**: PostgreSQL usa o schema `public` por default

### Migrations geradas automaticamente

O EF Core se encarrega de gerar o SQL correto para PostgreSQL automaticamente. As migrations criadas já contemplam essas diferenças.

## Comandos Úteis

### Criar nova migration (OMG.Repository)
```bash
cd src/OMG.Repository
dotnet ef migrations add <NomeDaMigration> --context OMGDbContext
```

### Criar nova migration (UserIdentity)
```bash
cd src/OMG.UserIdentity
dotnet ef migrations add <NomeDaMigration> --context UserIdentityDBContext
```

### Verificar status das migrations
```bash
dotnet ef migrations list --context OMGDbContext
```

### Reverter migration
```bash
dotnet ef migrations remove --context OMGDbContext
```

## Verificação

✅ Build completo sem erros  
✅ Migrations criadas com sucesso  
✅ AppHost configurado com PostgreSQL e volume persistente  
✅ PgWeb adicionado para gerenciamento visual  
✅ Todos os DbContexts atualizados para PostgreSQL  
✅ Aspire integration configurada corretamente  

## Próximos Passos

1. Testar a aplicação completamente
2. Verificar se todos os recursos funcionam corretamente
3. Validar consultas complexas e performance
4. Ajustar queries específicas se necessário (PostgreSQL pode ter otimizações diferentes)

## Notas Importantes

- O volume `omg-postgres-data` garante persistência dos dados
- PgWeb facilita a visualização e debug do banco
- As connection strings são gerenciadas automaticamente pelo Aspire
- Para desenvolvimento local, use as connection strings nos DbContextFactory
- Em produção, o Aspire fornecerá as connection strings via service discovery
