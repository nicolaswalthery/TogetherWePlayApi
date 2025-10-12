# Étape 1 : Image de base pour exécution
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Pas besoin de USER $APP_UID ici
EXPOSE 8080  # facultatif, juste informatif

# Étape 2 : Build du projet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copie des fichiers .csproj pour chaque projet avant restauration
COPY ["TogetherWePlayApi/TWP.Api.csproj", "TogetherWePlayApi/"]
COPY ["TWP.Api.Application/TWP.Api.Application.csproj", "TWP.Api.Application/"]
COPY ["TWP.Api.Core/TWP.Api.Core.csproj", "TWP.Api.Core/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["TWP.Api.Infrastructure/TWP.Api.Infrastructure.csproj", "TWP.Api.Infrastructure/"]

# Restaure les dépendances
RUN dotnet restore "TogetherWePlayApi/TWP.Api.csproj"

# Copie le reste du code
COPY . .

# Build en Release
WORKDIR "/src/TogetherWePlayApi"
RUN dotnet build "TWP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape 3 : Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TWP.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape 4 : Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ✅ Permet à Railway d’injecter son port
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

ENTRYPOINT ["dotnet", "TWP.Api.dll"]
