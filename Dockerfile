# ==========================================
# Fase 1: Compilar Frontend Angular
# ==========================================
FROM node:20-alpine AS client-build
WORKDIR /app/client

COPY scoutasset.client/package*.json ./
RUN npm install

COPY scoutasset.client/ ./
RUN npm run build

# ==========================================
# Fase 2: Compilar Backend ASP.NET Core
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ScoutAsset.Server/ScoutAsset.Server.csproj ScoutAsset.Server/
RUN dotnet restore ScoutAsset.Server/ScoutAsset.Server.csproj

COPY ScoutAsset.Server/ ScoutAsset.Server/

# Copiar artefactos de Angular a wwwroot de ASP.NET Core
COPY --from=client-build /app/client/dist/scoutasset.client/browser/ ScoutAsset.Server/wwwroot/

WORKDIR /src/ScoutAsset.Server
RUN dotnet publish ScoutAsset.Server.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==========================================
# Fase 3: Imagen de Ejecución (Runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ScoutAsset.Server.dll"]
