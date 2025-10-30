Mini Project Manager (Full-stack)

Tech:
- Backend: .NET 8, EF Core (SQLite default, InMemory fallback), JWT auth
- Frontend: React + TypeScript (Vite), React Router v6, Axios

Repository structure:
- `/backend` — ASP.NET Core Web API with EF Core, JWT, Services/Repositories/DTOs/Controllers
- `/backend.Tests` — xUnit tests for Auth and Projects services
- `/frontend` — React + TS app

Environment variables:
- `JWT_SECRET` — secret to sign tokens
- `JWT_ISSUER` — token issuer (default: ProjectManager)
- `CONNECTION_STRING` — optional override for EF Core connection string (default from appsettings.json → SQLite `Data Source=app.db`)

Quick start (Windows PowerShell):
1) Backend
```
cd backend
dotnet restore
dotnet build
dotnet run --urls http://localhost:5000
```
The API is now running at `http://localhost:5000`. Swagger UI at `/swagger` in Development.

2) Frontend
```
cd ../frontend
npm install
npm run dev
```
The app runs at `http://localhost:5173`. Vite dev server proxies `/api` to the backend.

Database and migrations:
- Default uses SQLite at `backend/app.db`. The app will attempt `db.Database.Migrate()` then `EnsureCreated()` as fallback.
- To add migrations (optional, recommended when using SQLite):
```
cd backend
dotnet tool install --global dotnet-ef
dotnet ef migrations add Init
dotnet ef database update
```

Auth endpoints:
- `POST /api/auth/register` → { username, password, email? } returns `{ token, username }`
- `POST /api/auth/login` → { username, password } returns `{ token, username }`

Projects endpoints (Bearer required):
- `GET /api/projects`
- `GET /api/projects/{projectId}`
- `POST /api/projects` → { title, description? }
- `DELETE /api/projects/{projectId}`

Tasks endpoints (Bearer required):
- `GET /api/projects/{projectId}/tasks`
- `POST /api/projects/{projectId}/tasks` → { title, dueDate? }
- `PUT /api/projects/{projectId}/tasks/{taskId}` → { title, dueDate?, isCompleted }
- `DELETE /api/projects/{projectId}/tasks/{taskId}`
- `PATCH /api/projects/{projectId}/tasks/{taskId}/toggle`

Demo user:
- Username: `demo`
- Password: `demo1234`

Running tests:
```
dotnet test
```

Curl examples:
```
# Register
curl -X POST http://localhost:5000/api/auth/register -H "Content-Type: application/json" -d '{"username":"alice","password":"password123"}'

# Login → extract token
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d '{"username":"alice","password":"password123"}' | powershell -Command "$input | ConvertFrom-Json | Select-Object -ExpandProperty token")

# Create project
curl -X POST http://localhost:5000/api/projects -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"My Project"}'

# List projects
curl http://localhost:5000/api/projects -H "Authorization: Bearer $TOKEN"

# Add a task (assuming project 1)
curl -X POST http://localhost:5000/api/projects/1/tasks -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"title":"My Task"}'
```

Git & GitHub:
```
git init
git add .
git commit -m "feat: initial full-stack project manager (backend + frontend)"
git branch -M main
git remote add origin https://github.com/<your-username>/mini-project-manager.git
git push -u origin main
```

Deploy notes:
- Backend (Render):
  - Build command: `dotnet build --configuration Release`
  - Start command: `dotnet backend/bin/Release/net8.0/ProjectManager.Api.dll` (or use `dotnet run` in `/backend`)
  - Env vars: `JWT_SECRET`, `JWT_ISSUER`, `CONNECTION_STRING`
- Frontend (Vercel):
  - Framework: Vite
  - Build command: `npm run build`
  - Output dir: `dist`
  - Set env var for API base if needed and adjust proxy for production; or host backend separately and configure absolute API URL in `frontend/src/lib/api.ts`.

Postman:
- A minimal Postman collection is provided at `postman_collection.json`.

Optional Docker:
- Backend Dockerfile at `/backend/Dockerfile`
- Frontend Dockerfile at `/frontend/Dockerfile`


