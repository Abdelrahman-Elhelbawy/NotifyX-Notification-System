# Contributing to NotifyX

Thank you for your interest in contributing to **NotifyX**! We welcome contributions from the open source community to build a high-performance, resilient notification delivery platform.

Please review this document to understand our contribution process, coding standards, and project workflows before submitting issues or pull requests.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How to Report Bugs](#how-to-report-bugs)
- [How to Suggest Features](#how-to-suggest-features)
- [Development Environment & Setup](#development-environment--setup)
  - [Prerequisites](#prerequisites)
  - [Repository Setup](#repository-setup)
- [Branching Strategy](#branching-strategy)
- [Commit Message Conventions](#commit-message-conventions)
- [Coding Standards & Architectural Guidelines](#coding-standards--architectural-guidelines)
- [Database Migrations](#database-migrations)
- [Testing Expectations](#testing-expectations)
- [Pull Request Process](#pull-request-process)
- [Code Review Process](#code-review-process)

---

## Code of Conduct

All contributors and maintainers are expected to adhere to our [Code of Conduct](CODE_OF_CONDUCT.md). Please ensure interactions are respectful, welcoming, and constructive.

---

## How to Report Bugs

When reporting bugs, please help us identify and reproduce the problem quickly:

1. **Search Existing Issues**: Ensure the bug hasn't already been reported in [GitHub Issues](https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System/issues).
2. **Use the Bug Report Template**: Create a new issue using our [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md).
3. **Provide Detailed Information**:
   - Concise summary of the bug.
   - Clear reproduction steps.
   - Expected vs. actual behavior.
   - Environment details (.NET SDK version, OS, SQL Server version, Redis version).
   - Relevant error logs, stack traces, or terminal outputs.

> [!NOTE]
> If you discover a potential **security vulnerability**, do not file a public issue. Refer to [SECURITY.md](SECURITY.md) for private reporting instructions.

---

## How to Suggest Features

We encourage ideas that improve NotifyX's reliability, scalability, and developer experience:

1. **Check Existing Requests**: Review [GitHub Issues](https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System/issues) to see if a similar feature is already discussed or planned.
2. **Use the Feature Request Template**: Open an issue using the [Feature Request Template](.github/ISSUE_TEMPLATE/feature_request.md).
3. **Describe the Use Case**:
   - Detail the problem the feature addresses.
   - Outline the proposed design, API endpoints, or architecture changes.
   - Discuss any alternative approaches considered.

---

## Development Environment & Setup

### Prerequisites

Before building NotifyX locally, make sure you have installed:

- **.NET 11 SDK** (Preview channel, matching `global.json` / `NotifyX.Api.csproj`):
  Verify installation with:
  ```bash
  dotnet --version
  ```
- **Microsoft SQL Server**: Local instance, SQL Server Express, or a Docker container:
  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrongPassword123!" \
    -p 1433:1433 --name notifyx-mssql -d mcr.microsoft.com/mssql/server:2022-latest
  ```
- **Redis Server**: Local installation or Docker container:
  ```bash
  docker run -p 6379:6379 --name notifyx-redis -d redis:7-alpine
  ```
- **EF Core CLI Tools**:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- **IDE**: Visual Studio 2022 (v17.12+), JetBrains Rider (2024.2+), or Visual Studio Code with the C# Dev Kit extension.

### Repository Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System.git
   cd NotifyX-Notification-System
   ```

2. **Configure Local Secrets & Settings**:
   Never commit passwords or credentials to source control. Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development:
   ```bash
   cd NotifyX.Api
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=NotifyX;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
   dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
   cd ..
   ```

3. **Restore Dependencies**:
   ```bash
   dotnet restore NotifyX.slnx
   ```

4. **Apply Database Migrations**:
   ```bash
   dotnet ef database update --project NotifyX.Api
   ```

5. **Build and Run the Solution**:
   ```bash
   dotnet build NotifyX.slnx
   dotnet run --project NotifyX.Api
   ```

---

## Branching Strategy

We follow a structured branching convention. When starting work on an issue, branch off `main` (or the current development branch) using the appropriate prefix:

| Branch Prefix | Purpose                                            | Example                             |
| :------------ | :------------------------------------------------- | :---------------------------------- |
| `feature/`    | New features or functional enhancements            | `feature/scheduled-notifications`   |
| `bugfix/`     | Bug fixes for existing functionality               | `bugfix/rate-limiter-sliding-window`|
| `hotfix/`     | Urgent fixes intended for immediate patch release   | `hotfix/outbox-connection-leak`     |
| `docs/`       | Documentation updates, guides, or tutorials        | `docs/api-examples-update`          |

---

## Commit Message Conventions

We adhere to the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification.

### Commit Format

```text
<type>(<scope>): <short description>

[optional body]

[optional footer(s)]
```

### Commit Types

- `feat`: A new user- or system-facing feature.
- `fix`: A bug fix.
- `docs`: Documentation updates only.
- `style`: Changes that do not affect code meaning (formatting, whitespace).
- `refactor`: Code changes that neither fix a bug nor add a feature.
- `perf`: Code changes improving system performance.
- `test`: Adding missing tests or correcting existing tests.
- `build`: Changes affecting build system or external dependencies.
- `ci`: Changes to CI configuration files and scripts.
- `chore`: Routine maintenance, updating dependencies or tooling.

### Common Scopes in NotifyX

- `notifications`: Notifications service, repository, or controllers.
- `recipients`: Recipient state management, read/unread handlers.
- `outbox`: Transactional outbox background service and publisher.
- `idempotency`: Idempotency keys, hashing, and deduplication logic.
- `cache`: Redis distributed caching and versioning.
- `ratelimit`: Rate limiting policies and partitioning.
- `health`: Liveness, readiness, and worker health checks.

### Examples

```text
feat(notifications): implement bulk unread notification counter
fix(outbox): handle cancellation token in periodic timer loop
docs(contributing): add database migration guidelines
refactor(cache): decouple cache key formatting into dedicated helper
```

---

## Coding Standards & Architectural Guidelines

To maintain clean and consistent code, adhere to these guidelines:

1. **C# Language Features**:
   - Target modern C# (C# 13 / .NET 11).
   - Enable and respect nullable reference types (`<Nullable>enable</Nullable>`). Avoid suppressing warnings (`!`) unless guaranteed by design.
   - Use primary constructors and target-typed `new` where appropriate.
   - Use records for immutable DTOs and models (`CreateNotificationRequest`, `NotificationResponse`).

2. **Clean Layered Architecture**:
   - **Controllers**: Thin endpoints handling HTTP routing, versioning, and status codes.
   - **Services**: Business rules, transactional logic, caching orchestration, and error dispatch.
   - **Repositories**: Data access abstraction using Entity Framework Core.
   - **Entities**: Domain entities configuring relational schema and domain constraints.

3. **Error Handling**:
   - Prefer functional result types using `OneOf<TSuccess, Error>` instead of throwing exceptions for predictable domain errors.
   - Reserve exceptions for unexpected, unrecoverable system failures.
   - Standardize API errors via ASP.NET Core `ProblemDetails` managed by `GlobalExceptionHandler`.

4. **Reliability Patterns**:
   - Always propagate `CancellationToken` across all asynchronous calls (`Task`, EF Core queries, Redis calls).
   - Multi-step persistence operations (e.g. creating a notification and generating an outbox entry) must execute inside an EF Core `ExecutionStrategy` transaction.

5. **Validation**:
   - Define validation rules using `FluentValidation` in `NotifyX.Api/Validators/`.
   - Never allow invalid payloads to reach the service or repository layer.

6. **Logging**:
   - Use structured logging with named placeholders (e.g., `_logger.LogInformation("Processing notification {NotificationId}", id)`).
   - Never log sensitive client payloads or credentials.

---

## Database Migrations

When altering the database schema:

1. **Entity Configurations**: Update entity models and their corresponding configurations under `NotifyX.Api/Data/Configuration/`.
2. **Generate Migration**:
   Use `dotnet ef` to scaffold a migration with a clear, PascalCase description:
   ```bash
   dotnet ef migrations add <MigrationName> --project NotifyX.Api
   ```
3. **Review Migration Script**:
   Inspect the generated migration files in `NotifyX.Api/Data/Migrations/`. Verify:
   - Columns, indexes, and foreign keys match expectations.
   - No unintentional table or column drops occur.
   - Column types and lengths match the configuration limits.
4. **Test Applying and Rolling Back**:
   ```bash
   dotnet ef database update --project NotifyX.Api
   ```

---

## Testing Expectations

NotifyX strives for high reliability and resilience. When contributing code:

- **Unit Testing**: Focus on isolating business logic in services, factories, retry policies, and validators.
- **Integration Testing**: Verify repository operations with Entity Framework Core, Redis caching behavior, and API controller endpoints.
- **Test Project Setup**: When adding test projects to the solution, place them under a `tests/` directory (e.g., `tests/NotifyX.Api.Tests`), reference the `NotifyX.Api` project, and register the test project in `NotifyX.slnx`.
- Run tests via the command line:
  ```bash
  dotnet test NotifyX.slnx
  ```

---

## Pull Request Process

1. **Fork and Branch**: Create a feature or bugfix branch following the [Branching Strategy](#branching-strategy).
2. **Implement & Format**: Write clean code adhering to coding standards and verify the project builds:
   ```bash
   dotnet build NotifyX.slnx
   ```
3. **Commit Cleanly**: Ensure commit messages adhere to [Conventional Commits](#commit-message-conventions).
4. **Fill Out the PR Template**: Open a pull request targeting `main`. Complete all items in the [Pull Request Template](.github/PULL_REQUEST_TEMPLATE.md).
5. **CI Checks**: Ensure all GitHub Actions workflow checks pass cleanly.

---

## Code Review Process

- Maintainers review PRs for architecture consistency, reliability, security, and performance.
- Address reviewer comments promptly and push updates to your branch.
- Once all checks pass and approvals are received, a maintainer will merge the PR (typically using squash and merge).
