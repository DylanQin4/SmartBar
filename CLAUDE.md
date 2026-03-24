# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
# Build entire solution
dotnet build SmartBar.slnx

# Run API (dev: https://localhost:5001, http://localhost:5000)
dotnet run --project src/SmartBar.API

# Run Admin dashboard (http://localhost:4200)
cd src/SmartBar.Admin && npm install && npm start

# Run Tablet app (pick target framework)
dotnet build src/SmartBar.Tablet -f net10.0-windows10.0.19041.0
```

## Database

SQL Server via EF Core. Connection string key is in `SmartBar.Shared/DatabaseSettings.cs`.

```bash
# Add migration (run from repo root)
dotnet ef migrations add <Name> --project src/SmartBar.Infrastructure --startup-project src/SmartBar.API

# Apply migrations
dotnet ef database update --project src/SmartBar.Infrastructure --startup-project src/SmartBar.API
```

In development, `ApplicationDbContextInitialiser` deletes and recreates the database on startup, then seeds an admin user (`admin@localhost` / `Administrator1!`) and the Administrator role.

## Tests

```bash
# Admin UI tests (Vitest)
cd src/SmartBar.Admin && npm test
```

No .NET test projects currently exist (tests/Test was removed).

## Architecture

Clean Architecture with CQRS intent via MediatR. .NET 10.0, nullable reference types and implicit usings enabled throughout.

**Layers (dependency flows inward):**
- **SmartBar.Domain** — Entities, base classes, constants. No dependencies.
- **SmartBar.Application** — Interfaces, models, validation (FluentValidation), command/query dispatch (MediatR). References Domain.
- **SmartBar.Infrastructure** — EF Core DbContext (inherits IdentityDbContext), identity services, JWT Bearer auth, interceptors. References Application + Shared.
- **SmartBar.API** — ASP.NET Core controllers entry point. References Application + Infrastructure.
- **SmartBar.Shared** — Shared constants (e.g., connection string keys).
- **SmartBar.Tablet** — .NET MAUI client (Android/iOS/Windows/macOS). Uses CommunityToolkit.Mvvm, Syncfusion.Maui.Toolkit, local SQLite.
- **SmartBar.Admin** — Angular 21 + Tailwind CSS dashboard.

## Key Conventions

- **Domain entities use factory pattern**: private constructors, static `Create()` methods, `Update()` for mutations (see `Category` entity).
- **Guard clauses**: Use `Ardalis.GuardClauses` for argument validation.
- **Auditing**: `BaseAuditableEntity` fields (Created, CreatedBy, LastModified, LastModifiedBy) are auto-populated by `AuditableEntityInterceptor` on SaveChanges.
- **Result pattern**: `Application/Common/Models/Result.cs` for operation outcomes instead of throwing exceptions.
- **EF Core configurations**: Fluent API in `Infrastructure/Data/Configurations/` using `IEntityTypeConfiguration<T>`, auto-discovered via `ApplyConfigurationsFromAssembly`.
- **DI registration**: Infrastructure services registered in `Infrastructure/DependencyInjection.cs` via `AddInfrastructureServices()` extension method.
- **Identity**: ASP.NET Core Identity with custom `ApplicationUser` (FirstName, LastName). Bearer token auth. Roles defined in `Domain/Constants/Roles.cs`.
- **Note**: The `Entites` folder under Domain is intentionally spelled that way (typo in original).
