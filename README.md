# TaskFlow API

A small REST API for managing tasks, built with **ASP.NET Core 8**, **C#**, **Entity Framework Core** and **SQLite**.
Made as a portfolio project for a Software Development Intern (ASP.NET Core / C#) application.

## Features
- Full CRUD for tasks (`/api/todos`)
- Filter by status, search by title, pagination
- DTOs + DataAnnotations validation (automatic `400` responses)
- Service layer + dependency injection (controllers stay thin)
- EF Core with SQLite (auto-created on first run)
- Swagger UI for trying the API
- xUnit tests for the service layer (EF Core InMemory)

## Run it
Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
cd TaskFlow.Api
dotnet run
```
Open the URL shown in the console, then go to `/swagger`.

## Run the tests
```bash
dotnet test TaskFlow.Tests
```

## Endpoints
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/todos?status=Done&search=api&page=1&pageSize=10` | List (filter / search / paging) |
| GET | `/api/todos/{id}` | Get one |
| POST | `/api/todos` | Create |
| PUT | `/api/todos/{id}` | Update (title, description, status, due date) |
| DELETE | `/api/todos/{id}` | Delete |

Status values: `Todo`, `InProgress`, `Done`.

### Example
```json
POST /api/todos
{ "title": "testing project", "description": "Add README", "dueDate": "2026-11-01T00:00:00Z" }
```

## Structure
```
TaskFlow.Api/
  Controllers/   HTTP layer
  Services/      business logic (ITodoService / TodoService)
  Data/          EF Core DbContext
  Models/        entities
  Dtos/          request/response models + validation
TaskFlow.Tests/  xUnit tests
```

## Ideas to extend
JWT authentication, user accounts, Docker, EF Core migrations, GitHub Actions CI.
