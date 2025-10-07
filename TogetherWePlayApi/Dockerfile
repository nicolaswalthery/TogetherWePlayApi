# Étape de construction
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers .csproj pour restaurer les dépendances
COPY ["TogetherWePlayApi/TWP.Api.csproj", "TogetherWePlayApi/"]
COPY ["TWP.Api.Application/TWP.Api.Application.csproj", "TWP.Api.Application/"]
COPY ["TWP.Api.Core/TWP.Api.Core.csproj", "TWP.Api.Core/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["TWP.Api.Infrastructure/TWP.Api.Infrastructure.csproj", "TWP.Api.Infrastructure/"]
RUN dotnet restore "./TogetherWePlayApi/TWP.Api.csproj"

# Copier tous les fichiers restants
COPY . .

# Construire l'application
WORKDIR "/src/TogetherWePlayApi"
RUN dotnet build "./TWP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape de publication
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TWP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape de production
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final  # Définir l'image de base pour l'exécution
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TWP.Api.dll"]
