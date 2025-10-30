# Basic Task Manager (Full-Stack)

A simple full-stack Task Manager built with:
- Backend: C# .NET 8 Web API (in-memory storage)
- Frontend: React + TypeScript (Vite) with Tailwind CSS
- HTTP: Axios on the frontend

## Repo Structure
```
/backend
  Controllers/
  DTOs/
  Models/
  Services/
  Program.cs
  appsettings.Development.json
  README-backend.md
/frontend
  src/
    pages/
    components/
    services/api.ts
  package.json
  README-frontend.md
```

## Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- (Optional) GitHub CLI: `gh`

## Run Backend (http://localhost:5000)
```bash
cd backend
# install deps
dotnet restore
# build
dotnet build
# run (or: dotnet watch run)
dotnet run --urls http://localhost:5000
```
CORS is configured to allow `http://localhost:3000`.

## Run Frontend (http://localhost:3000)
```bash
cd frontend
npm install
npm run dev
```

## Unit Tests (backend)
```bash
cd backend.tests
dotnet test
```


