# CLAUDE.md - Aos Adjutant

## Overview
A project with various tools to use relating to Warhammer: Age of Sigmar 4th edition.

The project will consist of three main parts:
- Admin dashboard: For managing all the Game data like factions, units, commands, ...
- List builder: Create and save army configurations with list validations.
- Play mode: Start a play session with one of the lists. This will adjust the data format to be easily accessible during gameplay.

## Project Overview

Project is structured as a monorepo with two top-level apps:

- **`backend/`** — .NET 10 / ASP.NET Core REST API backed by PostgreSQL database.
- **`frontend/`** — React 19 + TypeScript + Vite SPA consuming the backend API.

## Shared rules

Always use Context7 MCP when requiring library/API documentation, code generation, setup or configuration steps.
Docs-researcher agent can be used to retrieve these docs.

