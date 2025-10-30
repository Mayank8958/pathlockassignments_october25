# Backend - Basic Task Manager (ASP.NET Core .NET 8)

## Endpoints
- GET `/api/tasks` — list all tasks
- GET `/api/tasks/{id}` — get single task
- POST `/api/tasks` — create task
- PUT `/api/tasks/{id}` — update task
- DELETE `/api/tasks/{id}` — delete task

## Run locally
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```
- API: http://localhost:5000 (configure via `--urls` or `ASPNETCORE_URLS` if needed)
- CORS: allows `http://localhost:3000`

## Tests
```bash
cd backend.tests
dotnet test
```

## Notes
- In-memory storage only (resets on restart)
- DTO validation via DataAnnotations
