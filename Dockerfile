
# Étape 1 : compilation de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet publish src/GestionInterventions.Api/GestionInterventions.Api.csproj \
    -c Release \
    -o /app/publish

# Étape 2 : image finale pour exécuter l'API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Garantir que le dossier des fichiers statiques existe au démarrage
RUN mkdir -p /app/wwwroot/uploads/equipements

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "GestionInterventions.Api.dll"]
