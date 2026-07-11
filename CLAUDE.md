# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (net10.0) Web API demonstrating a shared microservice template. The `docker-demo` host is thin; almost all reusable infrastructure lives in two class libraries (`CommonLibrary`, `CommonRepo`) that are meant to be copied/shared across services. The single working feature is `SubscriptionController.ValidateTruck`, which checks a truck plate against the `Subscriptions` table.

## Build, Run, Test

Solution file is `docker-demo-test.slnx` (new XML solution format — requires a recent .NET 10 SDK / VS 2022 17.13+).

```bash
dotnet restore docker-demo-test.slnx
dotnet build docker-demo-test.slnx
dotnet run --project docker-demo --launch-profile Dev   # http://localhost:5035, Swagger at /swagger
```

- **Environment selection drives everything.** Config is layered `appsettings.json` + `appsettings.{ASPNETCORE_ENVIRONMENT}.json`. Valid environments (see `docker-demo/Properties/launchSettings.json`): `Dev`, `Local`, `Qa`. The base `appsettings.json` has **no** connection strings/Redis/Kafka — the app only works with an environment-specific file loaded, so always set the launch profile or `ASPNETCORE_ENVIRONMENT`.
- **Swagger is only registered in `Dev` and `Local`** (`Program.cs`). Other environments expose no API explorer.
- **Docker:** `docker build -f docker-demo/Dockerfile -t docker-demo .` (build context must be the repo root — the Dockerfile copies all three `.csproj` files by path). Runs on Linux, exposes 8080/8081. `DockerDefaultTargetOS` is Linux.
- **Tests:** there is no test project in the solution. `docker-demo-test` is just the solution name, not a test suite.

## Project structure & dependency direction

```
CommonLibrary   → no project references (leaf). Framework primitives.
CommonRepo      → references CommonLibrary. Domain + data + app plumbing.
docker-demo     → references both. HTTP host + feature code.
```

- **CommonLibrary** — cross-cutting primitives with no app knowledge: typed exceptions (`Exceptions/`), `BaseController`, `ApiResponse`/`Error`/`PaginatedResponse` (`Responses/`), custom `ValidationAttribute`s (`Validators/`, many Egypt-specific: national ID, plate, phone), Redis cache engine (`Caching/Redis/`), `RequestInfo`, and utilities (date/file/string/log helpers).
- **CommonRepo** — internally split into four folders that map to layers, each with its own DI extension method (see next section):
  - `Domain/` — entities (`Entities/`, mostly `Lookups/`) and interfaces. Repository contracts live here.
  - `Persistence/` — EF Core `DbContext`s and `AddPersistenceServices`.
  - `Infrastructure/` — repository implementations, Redis/Kafka caching, `Messaging/`, config binding.
  - `Application/` — MVC/formatter wiring, `Middlewares/ExceptionMiddleware`, MVC filter attributes, log services.
- **docker-demo** — `Controllers/`, `Services/` (Interfaces + Implementations), `DTOs/`. Feature services are the only thing registered in `ServiceExtensions.RegisterServices`.

## Composition / DI (the key wiring to understand)

`Program.cs` builds the container from four extension methods, one per layer, in this order:

```csharp
builder.Services.AddApplicationServices(builder.Configuration);    // CommonRepo/Application  — MVC, Swagger, RequestInfo, logging, formatters
builder.Services.AddPersistenceServices(builder.Configuration);    // CommonRepo/Persistence  — DbContexts
builder.Services.AddInfrastructureServices(builder.Configuration); // CommonRepo/Infrastructure — repositories, Redis, Kafka
builder.Services.RegisterServices(builder.Configuration);          // docker-demo/ServiceExtensions — feature services
```

When adding a feature you almost always touch **three** places: register the repo in `Infrastructure/InfrastructureServiceExtensions.AddRepositoriesServices`, register the service in `docker-demo/ServiceExtensions.RegisterServices`, and add the controller. The generic `IRepository<>`→`Repository<>` and `ICacheRepository<>`→`CacheRepository<>` are already open-registered, so entity-specific repos only need registering when they add custom methods.

## Request flow & conventions

- **All controllers inherit `CommonLibrary.Controllers.BaseController`.** Its `OnActionExecuting` (1) parses request headers into `RequestInfo` (notably the pipe-delimited `ClientInfo` and `CombinedHeaders` headers carrying user/ownership/app identity), and (2) converts any `ModelState` failure into a thrown `ValidationException` (never returns the default 400). Do not re-validate manually; rely on data-annotation attributes + this pipeline.
- **Wrap successful responses** with the protected `_Ok(...)` helpers so payloads are `ApiResponse<T>`. Tuple overload `_Ok((data, count))` produces paginated responses.
- **Exceptions are the control-flow mechanism.** Throw the typed exceptions from `CommonLibrary/Exceptions` (`NotFoundException`, `ForbiddenException`, `ValidationException`, etc.); `ExceptionMiddleware` (registered first in the pipeline) maps them to status codes + `ApiResponse`. Don't build error responses by hand.
- **Repository ownership filtering:** `Repository<TEntity>` uses `IRequestInfoService` to scope queries by ownership derived from headers. Reads/writes are implicitly tenant-scoped — bypassing the repository bypasses that filter.
- **Cache-backed entities** use `CacheRepository<TEntity>` (extends `Repository<TEntity>`), which mirrors writes to Redis (`Redis.OM`) and publishes change events to Kafka (`SyncWithKafka`, topics from `KafkaSyncProducerConfiguration`). Use it for lookup/reference data that must stay in sync across services.

## Data access notes

- **Two `DbContext`s**, both registered against the `DbContext` service type: `ApplicationDbContext` (main, SQL Server + NetTopologySuite for spatial) and `SystemErrorsLogDbContext` (error logging). Because both register as `DbContext`, resolve concrete types explicitly when you need a specific one.
- EF Core packages are pinned to **8.0.10** even though the target framework is net10.0.
- **Entity properties are snake_case** matching DB columns (e.g. `Subscription.plate_number`, `plate_code_en`) rather than C# PascalCase — match the existing column-name style when adding entities/queries.
- `ApplicationDbContext` also maps types from the `docker_demo.Models` namespace (`COLUMN_NAME`, `KeyViews`) used by the caching/index-creation infrastructure.
- Connection strings, Redis, and Kafka endpoints are checked into the `appsettings.{Env}.json` files (internal IPs + SQL credentials). Treat these as environment config, not secrets to invent.

## External dependencies expected at runtime

SQL Server (`DefaultSQLConnection`), Redis (`RedisConnection` / `RedisConfiguration`), and Kafka (`KafkaSyncProducerConfiguration` / `KafkaSettings`). The service starts without them but cache/messaging paths fail gracefully (exceptions are caught and logged to console). `QuartzJobs` config blocks exist but no scheduler is currently wired in `ServiceExtensions` (commented placeholder).
