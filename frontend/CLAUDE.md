# CLAUDE.md - frontend

Frontend-specific guidance on top of the root CLAUDE.md.

## Overview
Web frontend for a dashboard to manage Age of Sigmar game data fetched from the backend REST API.

## Tech Stack:
- React 19 + Typescript 6 + Vite 8
- Tailwind v4 for styling
- TanStack Query v5 for server state
- React Router v7 for client side routing
- Zod 4 for validation
- shadcn/ui compontents built on top of Base UI

## Project Structure
- `src/api` - API calls to backend
- `src/components` - Shared components
- `src/features` - Feature folders
- `src/lib` - Reusable libraries
- `src/types` - Shared types
- `src/utils` - Shared utility functions

## Architecture
- Code is organized in feature folders
- Data access only through custom hooks around TanStack hooks

## Commands
- Install dependencies - `npm install`
- Build - `npm run build`
- Lint - `npm run lint`
- Format - `npm run format`
- Dev server - `npm run dev`

