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

## Sample curl commands
```bash
# List all
curl -s http://localhost:5000/api/tasks | jq .

# Create
curl -s -X POST http://localhost:5000/api/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"My task","description":"optional"}' | jq .

# Get by id
curl -s http://localhost:5000/api/tasks/1 | jq .

# Update
curl -s -X PUT http://localhost:5000/api/tasks/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Updated","description":"new","isCompleted":true}' -i

# Delete
curl -s -X DELETE http://localhost:5000/api/tasks/1 -i
```

## Push to GitHub
```bash
git init
git add .
git commit -m "Initial commit: Basic Task Manager (backend + frontend)"
# Using GitHub CLI (recommended):
gh repo create <repo-name> --public --source=. --remote=origin --push
# Or add remote manually then push
# git remote add origin https://github.com/<you>/<repo-name>.git
# git branch -M main
# git push -u origin main
```

## Screenshots / Video (optional)
- Suggestion: use a light, single-column screenshot of the Task List.
- A short GIF: adding, toggling, deleting tasks, and switching filters.

For more details, see `backend/README-backend.md` and `frontend/README-frontend.md`.
