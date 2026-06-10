# ECommerce — Specialty Coffee Shop (Portfolio)

E-commerce B2C API built with **.NET 8** following **Clean Architecture + CQRS**.
API-only (no server-rendered frontend).

> 📄 Full business analysis & design spec: [`docs/PTTK_Portfolio_Ecommerce.md`](docs/PTTK_Portfolio_Ecommerce.md)

## Tech stack (planned)

.NET 8 · ASP.NET Core (Web API) · Clean Architecture · CQRS (MediatR) · EF Core 8 ·
FluentValidation · SignalR · Redis · Hangfire · Serilog · xUnit · Docker · GitHub Actions

## Solution structure

```
ECommerce/
├── src/
│   ├── ECommerce.Domain/          # Entities, value objects, enums, domain events (no dependencies)
│   ├── ECommerce.Application/     # CQRS features, behaviors, interfaces (depends on Domain)
│   ├── ECommerce.Infrastructure/  # EF Core, payment, shipping, email, cache, jobs (depends on Application)
│   └── ECommerce.Api/             # ASP.NET Core Web API (depends on Application + Infrastructure)
├── tests/
│   ├── ECommerce.Domain.UnitTests/
│   ├── ECommerce.Application.UnitTests/
│   └── ECommerce.IntegrationTests/
├── docker/                        # Dockerfile + docker-compose
├── docs/                          # spec, ADRs, screenshots
└── .github/workflows/             # CI/CD
```

### Dependency rule

`Api → Infrastructure → Application → Domain`. Domain depends on nothing.

## Requirements

- .NET SDK 8.0+ (the `.slnx` solution format needs SDK 9.0.200+ tooling to build via CLI; Visual Studio 2022 17.10+ also opens it).

## Getting started

```bash
dotnet restore ECommerce.slnx
dotnet build ECommerce.slnx
dotnet run --project src/ECommerce.Api
```

> Status: project skeleton only — implementation in progress.
