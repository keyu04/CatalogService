# ── Stage 1: Build ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies first
# Docker caches this layer — only re-runs if .csproj changes
COPY ["CatalogService.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ── Stage 2: Runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy built output from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "CatalogService.dll"]