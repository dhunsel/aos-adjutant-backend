# CLAUDE.md - backend

Backend-specific guidance on top of the root CLAUDE.md.

## Overview
A REST API exposing endpoints to manage Age of Sigmar game data.

## Tech Stack
- .NET 10 / ASP.NET Core with Controller APIs
- Entity Framework Core 10 backed by PostgreSQL 18.1 database
- Scalar for OpenAPI documentation
- xUnit testrunner, integration tests use TestContainers
- Observability stack ouputting to Grafana dashboards

## Project Structure
`src/AosAdjutant.Api`:
- `src/AosAdjutant.Api/Common` - Shared concepts
- `src/AosAdjutant.Api/Database` - EF Core config
- `src/AosAdjutant.Api/Features` - Feature based folders
- `src/AosAdjutant.Api/Migrations` - EF Core migration files
`tests/IntegrationTests` - Integrations tests using TestContainers
`tests/UnitTests` - Lightweight unit tests

## Architecture
- Feature organization instead of separation by layer
- Pragmatic approach to architecture: YAGNI + KISS
- Data flow between features can only flow in 1 direction

Why the simple structure? Refactor later when needed naturally instead of dogmatically following principles.

## Commands
- Build - `dotnet build AosAdjutant.slnx`
- Test - `dotnet test --no-build`
- Run - `dotnet run --project src/AosAdjutant.Api`
- Format - `dotnet csharpier format .`
- Add migration - `dotnet ef migrations add <Name> --project src/AosAdjutant.Api`
- Update database - `dotnet ef database update --project src/AosAdjutant.Api`

## Conventions
- Result pattern instead of exceptions for expected errors

