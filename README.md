# AoS Adjutant — Backend API

[![CI](https://github.com/dhunsel/aos-adjutant-backend/actions/workflows/ci.yml/badge.svg)](https://github.com/dhunsel/aos-adjutant-backend/actions/workflows/ci.yml)
[![codecov](https://codecov.io/github/dhunsel/aos-adjutant-backend/graph/badge.svg?token=Y9Y5NO8O1Z)](https://codecov.io/github/dhunsel/aos-adjutant-backend)

A REST API for managing Age of Sigmar army data. Allows clients to browse and manage factions, units, attack profiles, weapon effects, abilities, and battle formations.
Currently incomplete and very basic CRUD. Further completion of the features is necessary and afterwards a list builder + play mode are planned.

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| Database | PostgreSQL 18 + Entity Framework Core |
| Logging | Serilog → Grafana Loki |
| Tracing | OpenTelemetry → Grafana Tempo |
| Metrics | OpenTelemetry → Prometheus |
| Dashboards | Grafana |
| API docs | Scalar (OpenAPI) |
| Static analysis | Meziantou.Analyzer, SonarAnalyzer |
| Testing | xUnit, Testcontainers |

## Architecture

The API currently uses a very simple feature based structure. Each feature has its own controller, service, DTOs. Controllers talk to Services, which talk to EFCore's DbContext. Cross-cutting concerns (error handling, correlation IDs, result types) live in `Common/`.

```
src/AosAdjutant.Api/
├── Common/          # Result<T>, AppError, middleware
├── Database/        # DbContext + EF entity configurations
├── Features/
│   ├── Factions/
│   ├── Units/
│   ├── AttackProfiles/
│   ├── Abilities/
│   ├── BattleFormations/
│   └── WeaponEffects/
└── Migrations/
```

## Running Locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for the observability stack and PostgreSQL)

### Setup

1. Copy the environment file and fill in your values:
   ```bash
   cp .env.example .env
   ```

2. Start the infrastructure (PostgreSQL, Grafana, Loki, Prometheus, Tempo):
   ```bash
   docker compose up -d
   ```

3. Set the database connection string via [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):
   ```bash
   dotnet user-secrets set "AosAdjutant:DbContextConnectionString" "Host=localhost;Port=5432;Database=aos_adjutant;Username=<user>;Password=<password>" --project src/AosAdjutant.Api
   ```

4. Apply migrations:
   ```bash
   dotnet ef database update --project src/AosAdjutant.Api
   ```

5. Run the API:
   ```bash
   dotnet run --project src/AosAdjutant.Api
   ```

The API will be available at `http://localhost:5280`. The interactive API reference (Scalar) is at `http://localhost:5280/scalar`.

### Observability

| Service | URL |
|---|---|
| Grafana | http://localhost:3000 |
| Prometheus | http://localhost:9090 |
| Loki | http://localhost:3100 |
| Tempo | http://localhost:3200 |

## Running Tests

```bash
# Unit tests
dotnet test tests/AosAdjutant.UnitTests

# Integration tests (requires Docker for Testcontainers)
dotnet test tests/AosAdjutant.IntegrationTests
```

Integration tests spin up a PostgreSQL container via Testcontainers.
