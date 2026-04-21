# SmartBar — POS for Grocery-Bars in Madagascar

## About

SmartBar is a Point of Sale (POS) system built from firsthand experience running a family-owned grocery-bar in Madagascar. It was created to solve real, everyday problems that generic POS software simply doesn't address:

- **Local Credit (Trosa)** — Customers regularly buy on credit with informal verbal agreements. SmartBar tracks every credit invoice per customer with strict FIFO repayment.
- **Alcohol sold by the pour** — Bottles are opened and sold in arbitrary amounts (e.g. 1,300 Ar worth of rum). SmartBar automates the fraction formula with a configurable margin rate and maintains separate pools for sealed vs. open bottles.
- **Untracked internal consumption** — SmartBar categorizes and values every internal withdrawal (owner, children, friends, employees).
- **Cash register discrepancies** — SmartBar traces every cash flow and computes the theoretical balance automatically at end of day.
- **No visibility into actual profitability** — SmartBar generates automatic internal pre-accounting entries for every business event, enabling real profit/loss reporting.
- **Cigarette unit management** — Bought by the pack, sold by the pack or by the piece. SmartBar handles unit conversions and tracks stock down to the individual cigarette.
- **Bottle deposits** — SmartBar tracks deposits out vs. returned; a non-returned deposit is automatically recorded as acquired revenue.
- **Table management** — Multiple simultaneous orders per table, table merging, and line transfers between orders.
- **Time-based pricing** — A product can have different prices applied automatically based on the time the line is added.
- **Supplier management** — Partially or fully paid purchases, supplier debt tracking, purchase price history.
- **Employee payroll and advances** — Monthly salary tracking, deductible advances, employee consumption recorded as a benefit or salary deduction.
- **Breakage** — A breakage event adds a penalty line to the invoice and triggers a dedicated stock movement.
- **Alerts** — Low stock, unpaid invoices, supplier debts.
- **Full audit trail** — History of every order modification, table transfer, expense, inventory count, and stock adjustment.
- **Sensitive action security** — QR code or password validation required for critical operations.
- **Offline-first** — The tablet works without a connection and syncs automatically when the network is restored.

## Architecture

SmartBar follows **Clean Architecture** with CQRS intent via MediatR.

```
┌───────────────────────────────────────────────────────────────┐
│                         Clients                               │
│                                                               │
│  ┌─────────────────┐       ┌────────────────────────────┐     │
│  │ SmartBar.Tablet │		 │ SmartBar.Admin             │     │
│  │ .NET MAUI       │       │ Angular 21 + Tailwind CSS  │     │
│  │ Offline-first   │       │ Admin dashboard            │     │
│  │ Local SQLite    │       │                            │     │
│  └────────┬────────┘       └─────────────┬──────────────┘     │
│           │                              │                    │
│           └───────────┐  ┌───────────────┘                    │
│                       ▼  ▼                                    │
│              ┌─────────────────┐                              │
│              │  SmartBar.API   │  ASP.NET Core REST API       │
│              └────────┬────────┘                              │
├───────────────────────┼───────────────────────────────────────┤
│                       ▼                                       │
│  ┌────────────────────────────────────────────────────────┐   │
│  │ SmartBar.Application                                   │   │
│  │ Interfaces · MediatR commands/queries · Validation     │   │
│  └───────────────────────┬────────────────────────────────┘   │
│                          │                                    │
│  ┌───────────────────────▼────────────────────────────────┐   │
│  │ SmartBar.Domain                                        │   │
│  │ Entities · Base classes · Constants · Business rules   │   │
│  └────────────────────────────────────────────────────────┘   │
├───────────────────────────────────────────────────────────────┤
│  ┌────────────────────────────────────────────────────────┐   │
│  │ SmartBar.Infrastructure                                │   │
│  │ EF Core (SQL Server) · ASP.NET Identity · JWT Bearer   │   │
│  └────────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌────────────────────────────────────────────────────────┐   │
│  │ SmartBar.Shared                                        │   │
│  │ Cross-cutting constants                                │   │
│  └────────────────────────────────────────────────────────┘   │
└───────────────────────────────────────────────────────────────┘
```

**Core principle:** Every business event simultaneously generates stock movements (`StockMovement`) and internal pre-accounting entries (`LedgerEntry`) in a single transaction. Reports always aggregate from these two sources — never from business tables directly.

### Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core (.NET 10.0) |
| Domain / Application | MediatR, FluentValidation, Ardalis.GuardClauses |
| Database | SQL Server, EF Core |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Admin Dashboard | Angular 21, Tailwind CSS |
| Tablet / Mobile | .NET MAUI (Android, iOS, Windows, macOS), SQLite offline |

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or Docker)
- Node.js (for Admin dashboard)

### Run

```bash
# Build
dotnet build SmartBar.slnx

# API (https://localhost:5001)
dotnet run --project src/SmartBar.API

# Admin dashboard (http://localhost:4200)
cd src/SmartBar.Admin && npm install && npm start
```

In development, migrations are applied automatically on API startup (`MigrateAsync`) with a seeded admin account (`admin@localhost` / `Administrator1!`).

### Tests

```bash
# Run all tests
dotnet test

# Run by project
dotnet test tests/SmartBar.Domain.Tests              # Unit tests (domain logic)
dotnet test tests/SmartBar.Application.Tests          # Unit tests (validators, handlers — mocked DB)
dotnet test tests/SmartBar.Application.IntegrationTests  # Integration tests (SQLite in-memory)

# Admin UI tests
cd src/SmartBar.Admin && npm test
```

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project src/SmartBar.Infrastructure --startup-project src/SmartBar.API

# Apply pending migrations manually
dotnet ef database update --project src/SmartBar.Infrastructure --startup-project src/SmartBar.API

# Revert to a specific migration
dotnet ef database update <MigrationName> --project src/SmartBar.Infrastructure --startup-project src/SmartBar.API
```

## License

This project is licensed under the [Business Source License 1.1](LICENSE).

- Personal and educational use permitted
- Commercial use reserved to the author
- No open-source conversion date defined at this time