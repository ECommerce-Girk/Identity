# AGENTS.md

## Project Structure
- **Root**: .NET 10 Web API project using ASP.NET Core
- **Main Application**: `Identity.Api`
- **Tests**: `Identity.Tests`

## Key Commands
- **Run API**: `dotnet run --project Identity.Api`
- **Run Tests**: `dotnet test`
- **Build Solution**: `dotnet build`

## Framework Details
- Framework: .NET 10 / C# 13
- Uses ASP.NET Core with Entity Framework Core
- PostgreSQL database (Npgsql provider)
- Uses MSTest for unit and integration testing
- Testcontainers (`postgres:18-alpine`) for integration testing with Postgres

## Code Style & Conventions

- **Primary Constructors:** Always use primary constructors for classes and record declarations.
- **Dependency Fields:** Private fields representing injected dependencies must start with an underscore (e.g., `_logger`, `_service`).
- **Async Method Naming:** Do not append the `Async` suffix to asynchronous method names.
- **Cancellation Tokens:** Controller actions must accept a `CancellationToken ct` parameter (bound to `HttpContext.RequestAborted`) and pass it through to all asynchronous service, handler, and db calls.
- **Namespaces:** Always use file-scoped namespace declarations (e.g., `namespace MyProject.Services;`).
- **DTO Properties:** Use the `required` modifier on mandatory properties in DTOs:
  ```csharp
  public required string Name { get; set; }
- **Enums:** They must live in a separate file and always add "Enum" suffix. (e.g., `public enum TaskStatusEnum`)

## Database & EF Core Rules
- Database context: `IdentityDbContext`
- Migrations location: `src/api/Identity.Infra/Database/Migrations`
- Connection string in `appsettings.json`
- **Do NOT** manually edit migration snapshot files unless explicitly asked.
- Prefer LINQ pattern matching over complex raw SQL statements where possible.
- Use arrays when returning collections.
- Each entity must live in its own .cs file.

## Testing Notes
- Integration tests use `CustomWebApplicationFactory` and inherit from `TestsBase`.
- Tests require Docker desktop / daemon active to run `Testcontainers.PostgreSql`.
- Always verify tests pass (`dotnet test`) after refactoring domain logic or endpoint handlers.
- When creating a resource assert for the response model and the db state.

## Error Handling & Exception Rules
- **No `try-catch` in Controllers:** Controller/endpoint actions must **NEVER** wrap service calls in `try-catch` blocks. Allow domain and validation exceptions to bubble up.
- **Global Exception Handling:** All domain failures must throw custom exceptions inheriting from `Ex` (e.g., `NotFoundEx`, `ValidationEx`, `NotAllowedEx`).
- **Handler Strategy:** Errors are caught globally by `CustomExceptionHandler` (`IExceptionHandler`), which serializes them into standardized `ProblemDetails` responses with optional `ReasonCode` metadata.
- **Service Responsibility:** Domain and business logic validation belongs strictly in the service layer. Services must throw the appropriate custom `Ex` type with a descriptive message and optional `ReasonCode`.

## DTO & Entity Mapping Rules

- **Static Factory Mapping in Response Models:** Whenever an entity contains all required data to populate a response DTO, mapping must **NOT** be done manually inside services or handlers. The response DTO must expose a static factory method (e.g., `From[Entity]`), and services must delegate mapping entirely to it:
  ```csharp
  // Service layer when Entity is self-sufficient:
  var user = await _database.GetById(userId, ct);
  return UserResponse.FromUser(user);

## Agent Context & Safety Rules
- **NEVER** read, inspect, or modify files inside `bin/` or `obj/` directories.
- Do NOT generate verbose, conversational explanations. Output precise file paths, concise summaries, and clean code blocks.