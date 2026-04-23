# AoS Adjutant

[![backend](https://github.com/dhunsel/aos-adjutant/actions/workflows/backend.yml/badge.svg)](https://github.com/dhunsel/aos-adjutant/actions/workflows/backend.yml)
[![codecov](https://codecov.io/github/dhunsel/aos-adjutant/graph/badge.svg?token=Y9Y5NO8O1Z)](https://codecov.io/github/dhunsel/aos-adjutant)

Monorepo for AoS Adjutant — a tool for managing Age of Sigmar army data.

## Layout

- [`backend/`](backend/) — .NET 10 / ASP.NET Core REST API, PostgreSQL + EF Core, Serilog + OpenTelemetry. See
  [`backend/README.md`](backend/README.md).
- [`frontend/`](frontend/) — React 19 + TypeScript + Vite SPA consuming the backend API.

## Quick start

```bash
# Backend
cd backend
docker compose up -d          # postgres + observability stack
dotnet restore AosAdjutant.slnx
dotnet run --project src/AosAdjutant.Api

# Frontend (in a second terminal)
cd frontend
npm install
npm run dev
```

