# OMG Project - AI Coding Agent Instructions

## Architecture Overview

This is a **multi-layered .NET 9 solution** using **.NET Aspire** for cloud-native orchestration. The system manages a fragrance/candle product ordering system ("OMG que Cheiro").

### Core Components
- **OMG.Api** - REST API backend (ASP.NET Core Web API)
- **OMG.WebApp** - Blazor Server frontend using MudBlazor components
- **OMG.Domain** - Business logic, entities, services, and contracts
- **OMG.Repository** - Data access layer with Entity Framework Core
- **OMG.UserIdentity** - Authentication/authorization module
- **OMG.Aspire/OMG.AppHost** - .NET Aspire orchestration host

### Solution Organization
Logical folders in `OMG.sln` group projects by concern:
- **Domain**: Business logic (`OMG.Domain`, `OMG.Domain.Test`)
- **DataAccess**: Repository layer (`OMG.Repository`, `OMG.Repository.Test`)
- **Api**: Web API (`OMG.Api`, `OMG.Api.Test`)
- **WebApp**: Blazor frontend (`OMG.WebApp`)
- **Identity**: User management (`OMG.UserIdentity`)
- **Aspire**: Cloud orchestration (`OMG.AppHost`, `OMG.ServiceDefaults`)

## Running the Application

**Primary method**: Use .NET Aspire orchestration
```bash
aspire run
```
This command (already running in terminal) starts all services with proper dependencies and service discovery.

**Infrastructure**: SQL Server 2022 in Docker
- Connection managed by Aspire AppHost
- See `docker-compose.yml` for standalone SQL Server container (SA_PASSWORD: `Pass@word`)
- Database auto-created and seeded on API startup via `OMGDbContext.SeedData()`

## Key Architectural Patterns

### 1. Dependency Injection Extensions
Each layer exposes DI registration via extension methods:
- `DomainDI.AddOMGServices()` - Registers domain services
- `RepositoryDI.AddOMGRepository()` - Registers repositories and DbContext with Aspire integration
- Both use **transient** lifetime for services/repositories

### 2. Generic CRUD Controller Pattern
`BaseCRUDController<IEntity>` provides standard REST endpoints for entities:
```csharp
public class CorController(IRepositoryEntity<Cor> repository) : BaseCRUDController<Cor>(repository)
```
- Inherit for automatic GET/POST/PUT/DELETE
- Add custom actions like `GetSearchCores()` for entity-specific queries
- See `src/OMG.Api/Controllers/CorController.cs` for reference implementation

### 3. Soft Delete Implementation
All entities inherit from `Entity` base class implementing `ISoftDeletable`:
- `IsDeleted` and `DeletedAt` properties on every entity
- EF Core query filters automatically exclude soft-deleted records (see `PedidoMap.cs`)
- Index on `IsDeleted` for performance

### 4. Entity Mappings
EF Core configurations use `IEntityTypeConfiguration<T>` in `src/OMG.Repository/Mappings/`:
- Fluent API for relationships, constraints, precision
- Applied via `modelBuilder.ApplyConfigurationsFromAssembly()` in `OMGDbContext`

### 5. Service Discovery and HttpClient
Blazor WebApp communicates with API using named HttpClient:
```csharp
builder.Services.AddHttpClient(Configuracao.HttpClientNameOMGApi, opt => {
    opt.BaseAddress = new Uri("https://omg-api");
});
```
- Service name `"omg-api"` resolved by Aspire service discovery
- Resilience handlers and service discovery auto-configured via `ConfigureHttpClientDefaults()`

## Domain Model

### Core Entities (all in `src/OMG.Domain/Entities/`)
- **Pedido** - Orders with status tracking (EPedidoStatus enum), includes event sourcing for status changes
- **PedidoItem** - Order line items
- **Cliente** - Customer records
- **Produto, Cor, Aroma, Formato, Embalagem** - Product characteristics

### Business Logic in Services
Service implementations live in `src/OMG.Domain/Services/`:
- **PedidoService** orchestrates order creation, injecting multiple services (Cliente, Cor, Aroma, etc.)
- Services use repositories via interfaces (`IRepositoryEntity<T>`, `IPedidoRepository`)
- Example: `CreateNewPedido()` builds complete Pedido graph from `NewPedidoRequest`

### Event Sourcing Pattern
Status changes tracked via `EventChangeStatus` entity:
- `EventRepository.EventChangeStatusPedido()` logs old→new status transitions
- See `src/OMG.Domain/Events/EventChangeStatus.cs`

## Testing Conventions

### Unit Testing with xUnit + NSubstitute + FluentAssertions
Example from `src/OMG.Api.Test/Controllers/CorControllerTest.cs`:
```csharp
[Fact]
public async Task GetEntities_ShouldReturnOkWithCorList()
{
    var cores = new List<Cor> { new Cor { Nome = "Azul" } };
    _repository.GetAll(null).Returns(cores);
    
    var result = await _controller.GetEntities();
    
    var okResult = result.Result as OkObjectResult;
    okResult.Should().NotBeNull();
}
```
- Mock repositories using `NSubstitute.For<IRepositoryEntity<T>>()`
- Verify action results using FluentAssertions
- Test projects mirror source structure (Api.Test, Domain.Test, Repository.Test)

### Running Tests
```bash
dotnet test --configuration Release
```
CI pipeline (`.github/workflows/dotnet-ci.yml`) runs on PR and main branch pushes.

## Blazor Frontend Specifics

- **MudBlazor** for UI components
- **Interactive Server** render mode
- **Portuguese (pt-BR)** culture default
- Handlers (`IPedidoHandler`, `IClienteHandler`) abstract API calls from components
- Components organized in `src/OMG.WebApp/Components/{Pages,Layout,Cliente,Pedido}/`

## Common Development Tasks

### Adding a New Entity
1. Create entity class in `src/OMG.Domain/Entities/` inheriting from `Entity`
2. Add `IEntityTypeConfiguration<T>` in `src/OMG.Repository/Mappings/`
3. Add `DbSet<T>` to `OMGDbContext`
4. Create migration: `dotnet ef migrations add AddNewEntity --project src/OMG.Repository`
5. Create controller inheriting `BaseCRUDController<T>` in `src/OMG.Api/Controllers/`
6. Register service in `DomainDI` if custom logic needed

### Aspire Service Integration
API and WebApp reference `OMG.ServiceDefaults` which configures:
- OpenTelemetry (metrics, tracing, logging)
- Health checks at `/health` and `/alive`
- Service discovery and resilience handlers
- Call `builder.AddServiceDefaults()` in `Program.cs`

## Project Conventions

- **Primary constructor syntax** used throughout (C# 12 feature)
- **Virtual properties** on entities for lazy loading proxy support
- **Required properties** (`required` keyword) enforced on critical entity relationships
- **pt-BR locale** for date/number formatting
- **CORS** allows all origins in API (configured in `OMG.Api/Program.cs`)
