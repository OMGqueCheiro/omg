# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["OMG.sln", "./"]

# Copy project files
COPY ["src/OMG.Api/OMG.Api.csproj", "src/OMG.Api/"]
COPY ["src/OMG.Core/OMG.Core.csproj", "src/OMG.Core/"]
COPY ["src/OMG.Domain/OMG.Domain.csproj", "src/OMG.Domain/"]
COPY ["src/OMG.Infrastructure/OMG.Infrastructure.csproj", "src/OMG.Infrastructure/"]
COPY ["src/OMG.Repository/OMG.Repository.csproj", "src/OMG.Repository/"]
COPY ["src/OMG.UserIdentity/OMG.UserIdentity.csproj", "src/OMG.UserIdentity/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/OMG.ServiceDefaults.csproj", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "src/OMG.Api/OMG.Api.csproj"

# Copy all source files
COPY ["src/OMG.Api/", "src/OMG.Api/"]
COPY ["src/OMG.Core/", "src/OMG.Core/"]
COPY ["src/OMG.Domain/", "src/OMG.Domain/"]
COPY ["src/OMG.Infrastructure/", "src/OMG.Infrastructure/"]
COPY ["src/OMG.Repository/", "src/OMG.Repository/"]
COPY ["src/OMG.UserIdentity/", "src/OMG.UserIdentity/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Build
WORKDIR "/src/src/OMG.Api"
RUN dotnet build "OMG.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OMG.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OMG.Api.dll"]
