# syntax=docker/dockerfile:1

# --- Frontend: build Angular ---
FROM node:22-alpine AS frontend
WORKDIR /frontend
COPY Frontend/CromosList/package.json Frontend/CromosList/package-lock.json ./
RUN npm ci
COPY Frontend/CromosList/ ./
RUN npm run build -- --configuration production

# --- Backend: build & publish .NET ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Backend/CromosList/CromosList.csproj Backend/CromosList/
RUN dotnet restore Backend/CromosList/CromosList.csproj
COPY Backend/CromosList/ Backend/CromosList/
COPY --from=frontend /frontend/dist/CromosList/browser Backend/CromosList/wwwroot/
RUN dotnet publish Backend/CromosList/CromosList.csproj -c Release -o /app/publish -p:SkipAngularBuild=true

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CromosList.dll"]