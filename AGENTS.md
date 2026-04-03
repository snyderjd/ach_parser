AGENTS
======

Purpose
-------
This file documents how agentic coding agents should build, run, test and format code in this repository and summarizes the project's code style and conventions. Keep the changes small and conservative; follow existing patterns used in the code.

Quick Commands
--------------
- Restore dependencies for the whole solution:
  - `dotnet restore AchParser.sln`
- Build the solution:
  - `dotnet build AchParser.sln -c Debug`
- Run the API locally (requires Postgres from docker-compose):
  - `docker-compose up -d postgres`
  - `dotnet run --project ./src/AchParser.Api`
- Run the generator (CLI):
  - `dotnet run --project ./src/AchParser.Generator -- --entries 10 --output sample.ach`

Tests
-----
- Run all unit tests:
  - `dotnet test tests/AchParser.Api.UnitTests/AchParser.Api.UnitTests.csproj -c Debug`
- Run tests for the full solution (all test projects):
  - `dotnet test AchParser.sln`
- Run a single test class (example):
  - `dotnet test tests/AchParser.Api.UnitTests/AchParser.Api.UnitTests.csproj --filter "ClassName=AchParser.Api.UnitTests.Controllers.FileControllerTests"`
- Run a single test method (example):
  - `dotnet test tests/AchParser.Api.UnitTests/AchParser.Api.UnitTests.csproj --filter "FullyQualifiedName=AchParser.Api.UnitTests.Controllers.FileControllerTests.MyTestMethod"`
  - If the fully qualified name is long, use the fuzzy match operator `~` instead of `=`:
    `--filter "FullyQualifiedName~FileControllerTests.MyTestMethod"`

Notes on test filters:
- `--filter` supports expressions documented by Microsoft; the two most useful keys are `ClassName` and `FullyQualifiedName`.
- Tests use xUnit (packages are defined in tests project).

Linting and Formatting
----------------------
- Use `dotnet format` to apply code formatting and fix simple analyzer issues:
  - `dotnet tool install -g dotnet-format` (if not installed)
  - `dotnet format AchParser.sln`
- The projects enable Nullable and ImplicitUsings in the csproj files. Enable Roslyn analyzers in your IDE (VS / VS Code C# extension).
- If you need to run analyzers at build time, use MSBuild properties:
  - `dotnet build /p:EnableNETAnalyzers=true`

Repository Layout (important paths)
----------------------------------
- Solution: `AchParser.sln`
- API project: `src/AchParser.Api`
- CLI generator: `src/AchParser.Generator`
- Unit tests: `tests/AchParser.Api.UnitTests`
- Docker compose for local DB: `docker-compose.yml`

Coding Style Guidelines
-----------------------
The codebase is C# targeting net8.0. Follow these conventions when modifying or adding code.

General
- Target C# 10+ idioms (file-scoped namespaces are used across the repo). Prefer file-scoped namespaces (e.g. `namespace Foo;`).
- Keep changes minimal and consistent with surrounding code. New files should follow the same structure as existing files.
- UTF-8 encoding, LF line endings when possible.

Formatting
- Indent with 4 spaces.
- Max line length: 120 characters. Prefer shorter lines for readability.
- Use `dotnet format` for consistent formatting. Address formatting issues before committing.

Usings / Imports
- Group usings into these groups and insert a blank line between groups in this order:
  1. System namespaces (System.*)
 2. Microsoft / ASP.NET namespaces
 3. Third-party packages (Moq, EntityFramework, etc.)
 4. Project namespaces (AchParser.*)
- Within each group prefer alphabetical order.
- Prefer file-scoped usings when it reduces noise, but follow existing file patterns. Do not introduce global usings unless the change is coordinated.

Nullability & Types
- Nullable reference types are enabled across projects. Prefer explicit nullability handling and avoid null-forgiving operator (`!`) unless well-justified.
- Keep APIs and DTOs explicit about nullable properties (`string?` vs `string`).

Naming Conventions
- Types (classes, enums, structs, interfaces): PascalCase.
- Interfaces: prefix with `I` (e.g. `IAchRepository`).
- Methods and properties: PascalCase.
- Local variables and method parameters: camelCase.
- Private fields: `_camelCase` (leading underscore shown in existing code: `_dbContext`).
- Constants: PascalCase.
- Test class names: `<ClassUnderTest>Tests` (existing tests follow this pattern). Test methods: descriptive PascalCase or `Should_DoX_When_Condition` style is acceptable.

Async / Await
- Prefer async all the way: methods that perform I/O should be `async` and return `Task` or `Task<T>`.
- Use `ConfigureAwait(false)` only in library code where context capture is undesirable; for ASP.NET controllers it's not necessary.

Error Handling and Logging
- For controller actions return framework results (`BadRequest`, `NotFound`, `CreatedAtAction`, `Ok`, `Problem`) rather than throwing raw exceptions to avoid leaking details.
- Do not swallow exceptions silently. Log important exceptions with `ILogger<T>` and return an appropriate top-level response (use `Problem()` for unexpected failures).
- Validate input early and return `BadRequest` with meaningful messages when validation fails. Prefer model validation attributes and `[ApiController]` conventions for simple validation.

EF Core / Database
- Use `AsNoTracking()` for read-only queries where entities are not modified (this repo already uses it in GETs).
- Keep DbContext scoped to requests (use dependency injection as in existing controllers).
- Use explicit projection to DTOs in queries (`.Select(...)`) to avoid pulling entire entities when unnecessary.

Testing
- Unit tests use xUnit and Moq. Keep unit tests isolated and avoid hitting external services (use mocks for DbContext or use in-memory DB for integration scenarios).
- Test project layout is under `tests/AchParser.Api.UnitTests`.
- Use `dotnet test --filter` to run single tests during development.

Pull Requests and Changes
- Keep PRs small and focused. Describe why a change was made, not only what was changed.
- Fix style/formatting violations in the same PR that introduces new code.

Cursor / Copilot rules
- No .cursor rules directory or .cursorrules files found in the repository.
- No `.github/copilot-instructions.md` file found.
If you add rules there in the future, include them here and make sure agents consume them.

If you are an automated agent
--------------------------
- Read this file before making edits.
- Preserve existing patterns; prefer minimal, local changes over wide refactors unless the user requests them.
- When running commands from this document, use the explicit project paths shown above to avoid ambiguous behavior.

Contact / Further info
- Project README has usage examples for generator and local development: `README.md`.

EOF
