# NotifyX

[![.NET CI](https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Target Framework](https://img.shields.io/badge/.NET-11.0-512BD4.svg)](NotifyX.Api/NotifyX.Api.csproj)

**NotifyX** is a high-performance, resilient notification delivery and state management backend built on **ASP.NET Core (.NET 11)**. Engineered for distributed systems, NotifyX guarantees reliable message delivery via the **Transactional Outbox pattern**, enforces strict deduplication via **idempotency keys**, optimizes read throughput with **Redis distributed caching**, streams real-time updates over **SignalR**, and protects endpoints with multi-policy **rate limiting**.

---

## Table of Contents

- [Problem Statement](#problem-statement)
- [Key Features](#key-features)
- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Repository Structure](#repository-structure)
- [Prerequisites](#prerequisites)
- [Installation and Setup](#installation-and-setup)
- [Database Configuration & Migrations](#database-configuration--migrations)
- [Environment Variables & Configuration Reference](#environment-variables--configuration-reference)
- [Running the Application](#running-the-application)
- [API Documentation & Endpoints](#api-documentation--endpoints)
  - [API Versioning](#api-versioning)
  - [Notifications Management](#notifications-management)
  - [Recipient State Management](#recipient-state-management)
  - [Health Checks](#health-checks)
- [Authentication](#authentication)
- [SignalR Real-Time Streaming](#signalr-real-time-streaming)
- [Testing Instructions](#testing-instructions)
- [Contributing](#contributing)
- [Security Policy](#security-policy)
- [License](#license)

---

## Problem Statement

In modern distributed microservices and web platforms, notification systems commonly suffer from:
1. **Dual-write inconsistencies**: Writing to a database and attempting to broadcast real-time events over network sockets can fail intermittently, resulting in lost notifications or phantom notifications.
2. **Duplicate notification delivery**: Retries from network disconnects or client retries can spam end users with identical alerts.
3. **Database query strain**: High-volume notifications inundate relational databases with repetitive read and pagination queries.
4. **Endpoint abuse**: Lack of fine-grained rate limiting makes ingestion endpoints susceptible to traffic spikes and denial-of-service conditions.

**NotifyX** solves these problems using proven reliability patterns: atomic database transactions pairing domain entities with outbox messages, background polling workers with exponential backoff and dead-lettering, SHA-256 idempotency validation, version-keyed Redis cache-aside reads, and fine-grained token/window rate limiters.

---

## Key Features

- **Transactional Outbox Pattern**: Creates notifications and outbox records in a single atomic SQL transaction (`IsolationLevel.ReadCommitted`), guaranteeing at-least-once delivery to downstream consumers.
- **Asynchronous Outbox Dispatcher**: A hosted `BackgroundService` polling pending messages in batches of 20 with `PeriodicTimer` intervals.
- **Exponential Backoff with Jitter**: Automatically retries transient delivery failures with randomized exponential backoff ($2^{attempts} \times (1 + \text{jitter})$ capped at 60s) and moves unrecoverable messages to a dead-letter state after 5 failed attempts.
- **Idempotency Protection**: Rejects duplicate ingestion requests and eliminates race conditions using SHA-256 payload hashing and `Idempotency-Key` tracking.
- **Real-Time Client Streaming**: Broadcasts live notifications to targeted recipients using an ASP.NET Core SignalR hub (`/hubs/notifications`).
- **Distributed Caching (Cache-Aside with Versioning)**: Caches paginated notifications in Redis with automatic global version invalidation upon create, update, or delete operations.
- **Multi-Policy Rate Limiting**:
  - `CreateNotification`: Token Bucket (10 initial capacity, 1 replenishment per 6 seconds).
  - `ReadNotifications`: Sliding Window (60 requests/minute, 6 segments).
  - `NotificationStatus`: Sliding Window (30 requests/minute, 6 segments).
  - `UpdateNotification`: Fixed Window (20 requests/minute).
  - Rate limits partition by user identity or remote IP address and return RFC 7807 problem responses on 429 status.
- **Recipient Read Tracking**: Tracks per-user read/unread state, read timestamps, unread notification counts, and batch "read-all" operations.
- **API Versioning**: URL-segment API versioning configured via `Asp.Versioning.Mvc` (`/api/v1/...`).
- **Enterprise Health Probes**: Dedicated health check endpoints for liveness (`/health/live`), database connectivity (`/health/ready`), and background worker heartbeat (`/health/workers`).
- **Standardized Error Handling**: Unified RFC 7807 Problem Details with functional domain errors modeled via `OneOf`.

---

## Architecture Overview

NotifyX follows a layered, decoupled service architecture:

```
[ Client Request ] ───► [ Rate Limiter Middleware ] ───► [ API Controllers (v1) ]
                                                                   │
                                                                   ▼
                                                       [ Notification Service ]
                                                       ├── Validate (FluentValidation)
                                                       ├── Check Idempotency (SHA-256)
                                                       └── Invalidate Redis Cache
                                                                   │
                                                       (Atomic EF Core Transaction)
                                                                   ▼
                                                      ┌────────────────────────────┐
                                                      │    Microsoft SQL Server    │
                                                      │ ├── Notifications          │
                                                      │ ├── NotificationRecipients │
                                                      │ ├── IdempotencyKeys        │
                                                      │ └── OutboxMessages         │
                                                      └────────────────────────────┘
                                                                   ▲
                                                                   │ Polls pending
                                                      ┌────────────────────────────┐
                                                      │  OutboxBackgroundService   │
                                                      └────────────────────────────┘
                                                                   │ Dispatches
                                                                   ▼
                                                      ┌────────────────────────────┐
                                                      │ SignalRNotificationPublisher│
                                                      └────────────────────────────┘
                                                                   │ Broadcasts
                                                                   ▼
                                                    [ SignalR Hub (/hubs/notifications) ]
                                                                   │
                                                                   ▼
                                                          [ Connected Clients ]
```

---

## Technology Stack

| Layer / Concern | Technology | Package / Version |
| :--- | :--- | :--- |
| **Runtime & Framework** | .NET 11 (C# 13) | `Microsoft.NET.Sdk.Web` (11.0) |
| **Database ORM** | Entity Framework Core 11 | `Microsoft.EntityFrameworkCore.SqlServer` (11.0.0-rc.2) |
| **Database Engine** | Microsoft SQL Server | SQL Server 2022+ / Azure SQL |
| **Distributed Caching** | Redis | `Microsoft.Extensions.Caching.StackExchangeRedis` (11.0.0-rc.3) |
| **Real-Time Transport** | ASP.NET Core SignalR | `Microsoft.AspNetCore.SignalR` |
| **Validation** | FluentValidation | `FluentValidation.AspNetCore` (11.3.1), `FluentValidation` (12.2.0) |
| **API Versioning** | ASP.NET API Versioning | `Asp.Versioning.Mvc` (10.2.1), `Asp.Versioning.Mvc.ApiExplorer` (10.2.1) |
| **Result & Error Modeling**| OneOf | `OneOf` (3.0.271) |
| **Health Checks** | ASP.NET Diagnostics | `AspNetCore.HealthChecks.SqlServer`, `AspNetCore.HealthChecks.SignalR` |
| **API Specification** | ASP.NET OpenAPI | `Microsoft.AspNetCore.OpenApi` (11.0.0-rc.2) |

---

## Repository Structure

```
NotifyX/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md             # Bug report template
│   │   └── feature_request.md        # Feature request template
│   ├── PULL_REQUEST_TEMPLATE.md      # Pull request checklist & description template
│   └── workflows/
│       └── dotnet.yml                # GitHub Actions continuous integration workflow
├── NotifyX.slnx                      # Modern XML-format solution definition
├── NotifyX.Api/                      # Main ASP.NET Core Web API project
│   ├── BackgroundServices/           # Hosted background workers (OutboxBackgroundService)
│   ├── Common/                       # Cross-cutting constants, error codes, and exceptions
│   │   ├── Consts/                   # Idempotency SHA-256 computation
│   │   ├── Errors/                   # Domain error definitions (NotificationErrors, etc.)
│   │   └── Exceptions/               # GlobalExceptionHandler (RFC 7807 ProblemDetails)
│   ├── Configuration/                # Extension methods for DI services
│   │   ├── ApiVersioning/            # URL-segment versioning setup
│   │   ├── HealthChecks/             # Health check registration
│   │   └── RateLimiting/             # Token bucket & sliding window rate limiting
│   ├── Controller/                   # API v1 Controllers (Notifications, Recipients)
│   ├── Data/                         # EF Core DbContext, entity configurations, and migrations
│   │   ├── Configuration/            # Fluent API configurations for tables
│   │   └── Migrations/               # EF Core schema migration snapshots
│   ├── Entities/                     # Domain entities & enums (Notification, OutboxMessage, etc.)
│   ├── Factories/                    # Notification factory builders
│   ├── HealthChecks/                 # Custom health probes (Database, BackgroundWorker)
│   ├── Hubs/                         # SignalR hubs & user identifier providers
│   ├── Models/                       # Request/Response DTOs and pagination records
│   ├── Publishers/                   # Notification dispatcher abstractions & SignalR publisher
│   ├── Reliability/                  # Exponential backoff and retry policy calculations
│   ├── Repository/                   # Data access repositories and query abstractions
│   ├── Services/                     # Business logic, caching, idempotency, and outbox handlers
│   ├── Validators/                   # FluentValidation validator rules
│   ├── appsettings.json              # Base application configuration
│   ├── appsettings.Development.json  # Development configuration template
│   ├── DependancyInjection.cs        # Extension methods wiring application dependencies
│   ├── Program.cs                    # Application bootstrapping and middleware pipeline
│   └── NotifyX.Api.csproj            # Project configuration and package references
├── .gitignore                        # Git exclusion rules
├── CODE_OF_CONDUCT.md                # Contributor Covenant Code of Conduct v2.1
├── CONTRIBUTING.md                   # Comprehensive contribution guidelines
├── LICENSE                           # MIT License
├── README.md                         # Project documentation
└── SECURITY.md                       # Security policy and vulnerability reporting
```

---

## Prerequisites

Before setting up NotifyX locally, ensure you have:

1. **.NET 11 SDK** (Preview release):
   ```bash
   dotnet --version
   # Expected output: 11.0.x
   ```
2. **Microsoft SQL Server**: A running SQL Server instance (Docker, local, or Azure SQL).
3. **Redis**: A running Redis instance (version 6+ or 7+).
4. **EF Core CLI**:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

---

## Installation and Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Abdelrahman-Elhelbawy/NotifyX-Notification-System.git
   cd NotifyX-Notification-System
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore NotifyX.slnx
   ```

3. **Start infrastructure via Docker (Optional)**:
   If you don't have SQL Server and Redis running locally, you can start them with Docker:
   ```bash
   # Run SQL Server 2022
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourPassword123!" \
     -p 1433:1433 --name notifyx-sql -d mcr.microsoft.com/mssql/server:2022-latest

   # Run Redis
   docker run -p 6379:6379 --name notifyx-redis -d redis:7-alpine
   ```

---

## Database Configuration & Migrations

NotifyX uses Entity Framework Core with SQL Server.

### Connection Strings Configuration

Configure your connection strings in `NotifyX.Api/appsettings.json` or preferably via .NET User Secrets:

```bash
cd NotifyX.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=NotifyX;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
cd ..
```

### Applying Migrations

Apply the existing database migrations to create the database schema:

```bash
dotnet ef database update --project NotifyX.Api
```

### Adding New Migrations

If you make modifications to the entity models:

```bash
dotnet ef migrations add <MigrationName> --project NotifyX.Api
dotnet ef database update --project NotifyX.Api
```

---

## Environment Variables & Configuration Reference

NotifyX reads the following configuration keys from `appsettings.json` or matching environment variables:

| Setting Key | Environment Variable | Default / Example | Purpose |
| :--- | :--- | :--- | :--- |
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | `Server=localhost,1433;Database=NotifyX;...` | SQL Server database connection string |
| `ConnectionStrings:Redis` | `ConnectionStrings__Redis` | `localhost:6379` | Redis distributed cache connection host and port |
| `Logging:LogLevel:Default` | `Logging__LogLevel__Default` | `Information` | Default log level threshold |
| `Logging:LogLevel:Microsoft.AspNetCore` | `Logging__LogLevel__Microsoft__AspNetCore` | `Information` | Framework logging level |
| `Logging:LogLevel:Microsoft.EntityFrameworkCore` | `Logging__LogLevel__Microsoft__EntityFrameworkCore` | `Information` | EF Core logging level |
| `AllowedHosts` | `AllowedHosts` | `*` | Allowed host header filter |

---

## Running the Application

To launch the Web API project locally:

```bash
dotnet run --project NotifyX.Api
```

By default, the application listens on the ports defined in `Properties/launchSettings.json`:
- **HTTP**: `http://localhost:5050`
- **HTTPS**: `https://localhost:7290`

In development mode, OpenAPI definition endpoints are automatically exposed at `/openapi/v1.json`.

---

## API Documentation & Endpoints

### API Versioning

All API routes use URL segment versioning with a default version of `1.0`:
`http://localhost:5050/api/v1/[resource]`

---

### Notifications Management

#### 1. Create a Notification
Persists a notification, enqueues an outbox message, and schedules delivery. Requires an `Idempotency-Key` header.

- **Method**: `POST`
- **Path**: `/api/v1/notifications`
- **Headers**:
  - `Content-Type: application/json`
  - `Idempotency-Key: <unique-client-key>` *(string, max 200 characters)*
- **Rate Limit Policy**: `CreateNotification` (Token bucket: 10 capacity, 1 replenishment per 6 seconds)
- **Request Body**:
  ```json
  {
    "userIds": ["user-101", "user-102"],
    "title": "System Alert",
    "body": "Scheduled maintenance starting at midnight.",
    "type": 4,
    "priority": 1,
    "metadata": "{\"maintenanceWindowMinutes\": 60}"
  }
  ```
  *Note on `type`*: `1` = `General`, `2` = `TaskAssigned`, `3` = `CommentAdded`, `4` = `SystemAlert`  
  *Note on `priority`*: Must be an integer between 1 and 5.
- **Success Response** (`200 OK`):
  ```json
  {
    "id": "e3b0c442-98fc-1c14-9afbf4c8996fb924",
    "title": "System Alert",
    "body": "Scheduled maintenance starting at midnight.",
    "type": 4,
    "metadata": "{\"maintenanceWindowMinutes\": 60}"
  }
  ```
- **Error Responses**:
  - `400 Bad Request`: Validation failure or missing/invalid idempotency key.
  - `409 Conflict`: Idempotency key conflict (different payload submitted with the same key) or request in progress.
  - `429 Too Many Requests`: Rate limit exceeded.

#### 2. Get All Notifications (Paginated)
Retrieves a paginated list of active notifications. Responses are cached in Redis for up to 5 minutes or until cache invalidation.

- **Method**: `GET`
- **Path**: `/api/v1/notifications?pageNumber=1&pageSize=10`
- **Rate Limit Policy**: `ReadNotifications` (Sliding window: 60 requests/minute)
- **Success Response** (`200 OK`):
  ```json
  {
    "items": [
      {
        "id": "e3b0c442-98fc-1c14-9afbf4c8996fb924",
        "title": "System Alert",
        "body": "Scheduled maintenance starting at midnight.",
        "type": 4,
        "metadata": "{\"maintenanceWindowMinutes\": 60}"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 1
  }
  ```

#### 3. Get Notification by ID
- **Method**: `GET`
- **Path**: `/api/v1/notifications/{id}`
- **Success Response** (`200 OK`): Single `NotificationResponse` object.
- **Error Response**: `404 Not Found` if notification doesn't exist or is soft-deleted.

#### 4. Update Notification
Updates mutable fields of a notification and invalidates the Redis cache.

- **Method**: `PUT`
- **Path**: `/api/v1/notifications/{id}`
- **Rate Limit Policy**: `UpdateNotification` (Fixed window: 20 requests/minute)
- **Request Body**:
  ```json
  {
    "title": "Updated Alert Title",
    "body": "Updated maintenance window details.",
    "type": 4,
    "priority": 2,
    "scheduledAt": null,
    "metadata": "{\"maintenanceWindowMinutes\": 120}"
  }
  ```
- **Success Response**: `204 No Content`
- **Error Response**: `404 Not Found`

#### 5. Delete Notification
Soft-deletes the notification (`IsDeleted = true`) and invalidates the Redis cache.

- **Method**: `DELETE`
- **Path**: `/api/v1/notifications/{id}`
- **Success Response**: `204 No Content`
- **Error Response**: `404 Not Found`

---

### Recipient State Management

#### 1. Mark Notification as Read
- **Method**: `PUT`
- **Path**: `/api/v1/notifications/{notificationId}/read`
- **Request Body**:
  ```json
  {
    "userId": "user-101"
  }
  ```
- **Success Response**: `204 No Content`
- **Error Response**: `404 Not Found` or `400 Bad Request` (already read).

#### 2. Mark Notification as Unread
- **Method**: `PUT`
- **Path**: `/api/v1/notifications/{notificationId}/unread`
- **Request Body**:
  ```json
  {
    "userId": "user-101"
  }
  ```
- **Success Response**: `204 No Content`
- **Error Response**: `404 Not Found` or `400 Bad Request` (already unread).

#### 3. Get Unread Count for User
- **Method**: `GET`
- **Path**: `/api/v1/notifications/{notificationId}/{userId}/unread-count`
- **Success Response** (`200 OK`):
  ```json
  {
    "notificationId": "e3b0c442-98fc-1c14-9afbf4c8996fb924",
    "unreadCount": 3
  }
  ```

#### 4. Mark All Notifications as Read for User
- **Method**: `PUT`
- **Path**: `/api/v1/notifications/{userId}/read-all`
- **Success Response**: `204 No Content`

---

### Health Checks

| Endpoint | Method | Tag | Description |
| :--- | :--- | :--- | :--- |
| `/health/live` | `GET` | `live` | Verifies the API web host is alive and responding. |
| `/health/ready` | `GET` | `ready` | Pings the SQL Server database to ensure connectivity. |
| `/health/workers` | `GET` | `worker` | Checks the heartbeat of the `OutboxBackgroundService`. |

---

## Authentication

Authentication (JWT or OAuth2) is currently **not enforced** in the repository. All endpoints are currently accessible without bearer tokens.

- **Recipient Identification**: Target user IDs are currently supplied explicitly in request bodies (`userIds` array on notification creation) or URL path segments for recipient tracking.
- **Rate Limit Partitioning**: Without an active authenticated user claim (`NameIdentifier` or `sub`), the rate limiter automatically falls back to partitioning by the client's IP address.
- **Roadmap**: Full integration with ASP.NET Core Authentication (`JwtBearer`) and `[Authorize]` attributes is planned for upcoming milestones.

---

## SignalR Real-Time Streaming

NotifyX exposes a real-time SignalR Hub for streaming notifications directly to connected clients.

- **Hub Endpoint**: `/hubs/notifications`
- **Client Event**: `ReceiveNotification`

### SignalR Client Example (JavaScript / TypeScript)

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5050/hubs/notifications?userId=user-101")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveNotification", (notification) => {
    console.log("New Notification Received:", notification);
    // Payload structure:
    // {
    //   id: "e3b0c442-...",
    //   title: "System Alert",
    //   body: "Scheduled maintenance starting at midnight.",
    //   type: 4,
    //   metadata: "{...}",
    //   createdAt: "2026-10-31T13:45:00Z"
    // }
});

async function start() {
    try {
        await connection.start();
        console.log("Connected to NotifyX SignalR Hub");
    } catch (err) {
        console.error("SignalR Connection Error: ", err);
        setTimeout(start, 5000);
    }
}

start();
```

---

## Testing Instructions

Automated test suites (unit tests for services and validators, and integration tests using Testcontainers for SQL Server and Redis) are planned as part of the open source roadmap.

When test projects are introduced to the solution:

```bash
# Run all tests in the solution
dotnet test NotifyX.slnx

# Run tests with detailed console reporting
dotnet test NotifyX.slnx --logger "console;verbosity=detailed"
```

Contributors adding new features are strongly encouraged to include corresponding unit and integration tests. See [CONTRIBUTING.md](CONTRIBUTING.md#testing-expectations) for testing standards.

---

## Contributing

We welcome community contributions! Please read our [Contributing Guide](CONTRIBUTING.md) to learn about our development process, branching model, conventional commit standards, and pull request procedures.

---

## Security Policy

Security issues should be reported confidentially. Please refer to our [Security Policy](SECURITY.md) for details on supported versions and instructions for filing private security advisories.

---

## License

This project is licensed under the terms of the [MIT License](LICENSE).
