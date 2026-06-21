# CatalogService
Catalog microservice for managing categories, products, inventory, and product images in a grocery or retail platform.

## Tech Stack
- .NET 10 / ASP.NET Core
- SQL Server
- Entity Framework Core
- Docker + Docker Compose

## Features
- Category CRUD
- Product CRUD with filtering, search, and pagination
- Inventory lookup, update, reserve, and release operations
- Product image support and entity relationships
- Global exception handling middleware
- Consistent API response wrapper
- API versioning (v1)
- Rate limiting
- Automatic database migrations on startup
- Unit tests with xUnit and Moq

## Getting Started

### Run With Docker
docker-compose up --build
API available at: http://localhost:5070/swagger

### Run Without Docker
Update the connection string in `CatalogService/appsettings.Development.json` then:

dotnet run --project CatalogService/CatalogService.csproj

### Run Tests
dotnet test

## Project Structure

CatalogService/
├── Common/         → Constants, shared DTOs, settings, and logging helpers
├── Controllers/    → API endpoints for categories, products, and inventory
├── Data/           → EF Core DbContext and entity configuration
├── DTOs/           → Request and response models
├── Middlewares/    → Global error handling and validation configuration
├── Migrations/     → EF Core migrations
├── Models/         → Database entities
├── Repository/     → Data access layer
└── Services/       → Business logic layer

CatalogService.Tests/
└── Services/       → Unit tests for service layer logic
