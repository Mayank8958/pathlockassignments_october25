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

Scheduling (Bearer required):
- `POST /api/v1/projects/{projectId}/schedule`

Request body example:
```
{
  "days": 5,
  "hoursPerDay": 4,
  "startDate": "2025-11-01",   // optional, defaults to project CreatedAt
  "strategy": "earliest_due"    // earliest_due | longest_task
}
```

Response example:
```
{
  "projectId": 1,
  "startDate": "2025-11-01T00:00:00Z",
  "schedule": [
    {
      "date": "2025-11-01T00:00:00Z",
      "tasks": [
        { "taskId": 10, "title": "Write spec", "estimatedHours": 2 },
        { "taskId": 11, "title": "Design UI", "estimatedHours": 2 }
      ],
      "totalHours": 4
    }
  ],
  "overflow": [
    { "taskId": 12, "title": "Long task", "estimatedHours": 3 }
  ]
}
```

Algorithm notes:
- Considers only incomplete tasks.
- Uses default `estimatedHours = 1` per task (no EstimatedHours field yet).
- Sorts by strategy: `earliest_due` (by DueDate asc), `longest_task` (by EstimatedHours desc).
- Fills each day up to `hoursPerDay`, splitting a task across days if needed.

Frontend usage:
- Open a project → Project page shows a Scheduler section.
- Set Days, Hours/day, optional Start date, Strategy and click "Generate schedule".
- Export results as JSON or CSV via buttons.

Demo user:
- Username: `demo`
- Password: `demo1234`

Running tests:
```
dotnet test
```
