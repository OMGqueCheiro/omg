# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["OMG.sln", "./"]

# Copy project files
COPY ["src/OMG.Worker/OMG.MigrationWorker/OMG.MigrationWorker.csproj", "src/OMG.Worker/OMG.MigrationWorker/"]
COPY ["src/OMG.Repository/OMG.Repository.csproj", "src/OMG.Repository/"]
COPY ["src/OMG.Core/OMG.Core.csproj", "src/OMG.Core/"]
COPY ["src/OMG.UserIdentity/OMG.UserIdentity.csproj", "src/OMG.UserIdentity/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/OMG.ServiceDefaults.csproj", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "src/OMG.Worker/OMG.MigrationWorker/OMG.MigrationWorker.csproj"

# Copy all source files
COPY ["src/OMG.Worker/", "src/OMG.Worker/"]
COPY ["src/OMG.Repository/", "src/OMG.Repository/"]
COPY ["src/OMG.Core/", "src/OMG.Core/"]
COPY ["src/OMG.UserIdentity/", "src/OMG.UserIdentity/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Build
WORKDIR "/src/src/OMG.Worker/OMG.MigrationWorker"
RUN dotnet build "OMG.MigrationWorker.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OMG.MigrationWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OMG.MigrationWorker.dll"]
