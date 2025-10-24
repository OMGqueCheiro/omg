# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["OMG.sln", "./"]

# Copy project files
COPY ["src/OMG.BlazorApp/OMG.BlazorApp/OMG.BlazorApp.csproj", "src/OMG.BlazorApp/OMG.BlazorApp/"]
COPY ["src/OMG.BlazorApp/OMG.BlazorApp.Client/OMG.BlazorApp.Client.csproj", "src/OMG.BlazorApp/OMG.BlazorApp.Client/"]
COPY ["src/OMG.Core/OMG.Core.csproj", "src/OMG.Core/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/OMG.ServiceDefaults.csproj", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "src/OMG.BlazorApp/OMG.BlazorApp/OMG.BlazorApp.csproj"

# Copy all source files
COPY ["src/OMG.BlazorApp/", "src/OMG.BlazorApp/"]
COPY ["src/OMG.Core/", "src/OMG.Core/"]
COPY ["src/OMG.Aspire/OMG.ServiceDefaults/", "src/OMG.Aspire/OMG.ServiceDefaults/"]

# Build
WORKDIR "/src/src/OMG.BlazorApp/OMG.BlazorApp"
RUN dotnet build "OMG.BlazorApp.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OMG.BlazorApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OMG.BlazorApp.dll"]
