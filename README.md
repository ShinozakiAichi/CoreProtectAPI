# CoreProtect API

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 8"/>
  <img src="https://img.shields.io/badge/MySQL-ready-4479A1?logo=mysql&logoColor=white" alt="MySQL"/>
  <img src="https://img.shields.io/badge/License-MIT-green.svg" alt="MIT"/>
</p>

> **EN**: Read-only REST API exposing CoreProtect audit trails (blocks, containers, items, commands, chat, sessions, signs) without touching the plugin.
>
> **RU**: REST API для безопасного чтения журнала CoreProtect — блоки, контейнеры, предметы, команды, чат, сессии и таблички, без вмешательства в оригинальный плагин.

---

## Navigation · Навигация

1. [Vision & Features](#vision--features)
2. [System map](#system-map)
3. [Repository layout](#repository-layout)
4. [Quick start (EN)](#quick-start-en)
5. [Быстрый старт (RU)](#быстрый-старт-ru)
6. [Configuration](#configuration)
7. [Local development](#local-development)
8. [Docker](#docker)
9. [API catalogue](#api-catalogue)
10. [Security & operations](#security--operations)
11. [Troubleshooting](#troubleshooting)
12. [License](#license)

---

## Vision & Features

| Pillar | Details |
| ------ | ------- |
| **Predictable** | Uniform pagination (`limit/offset`), stable sorting, ISO-8601 timestamps. |
| **Safe** | SQL templates are parameterised, API key enforcement lives behind `X-Api-Key`, secrets never land in logs. |
| **Performant** | Target response <150 ms with tuned indices and read-only transactions. |
| **Operable** | Health endpoints (`/healthz`, `/readyz`), slow query logging, structured log output. |

## System map

```
┌──────────────────────────────┐
│          CoreProtect         │
│        MySQL / MariaDB       │
└──────────────┬───────────────┘
               │ read-only
┌──────────────▼───────────────┐
│    Infrastructure layer      │  ← SQL access, binary decoding, DI
└──────────────┬───────────────┘
               │ services
┌──────────────▼───────────────┐
│    Application layer         │  ← validation, orchestration
└──────────────┬───────────────┘
               │ DTOs
┌──────────────▼───────────────┐
│          API layer           │  ← ASP.NET Core controllers
└──────────────┴───────────────┘
```

The domain stays pure; outer layers depend inward only.

## Repository layout

```
CoreProtectAPI
├── CoreProtectAPI.sln
├── Dockerfile
├── README.md
├── src/
│   ├── CoreProtect.Domain/          # Entities, aggregates, value objects
│   ├── CoreProtect.Application/     # Query services, DTO builders
│   ├── CoreProtect.Infrastructure/  # Persistence adapters, configuration, DI
│   └── CoreProtect.Api/             # ASP.NET Core host, controllers, filters
└── tests/
    └── CoreProtect.Tests/           # xUnit specifications
```

## Quick start (EN)

```bash
# 1. Install dependencies
dotnet restore

# 2. Apply database connection (optional)
export ConnectionStrings__CoreProtect="Server=localhost;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=change-me;SslMode=None"

# 3. Run the API
dotnet run --project src/CoreProtect.Api/CoreProtect.Api.csproj
```

Open `http://localhost:8080/swagger` (development profile) for an interactive contract.

## Быстрый старт (RU)

```bash
# 1. Установите зависимости
dotnet restore

# 2. Настройте строку подключения (при необходимости)
export ConnectionStrings__CoreProtect="Server=localhost;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=change-me;SslMode=None"

# 3. Запустите API
dotnet run --project src/CoreProtect.Api/CoreProtect.Api.csproj
```

Разработка по умолчанию слушает `http://localhost:8080`; Swagger доступен по `/swagger`.

## Configuration

Settings live in `appsettings.json` and can be overridden with environment variables (`__` replaces `:`).

| Setting | Environment variable | Default | Purpose |
| ------- | -------------------- | ------- | ------- |
| Connection string | `ConnectionStrings__CoreProtect` | `Server=localhost;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=change-me;SslMode=None` | Read-only MySQL endpoint. |
| Default limit | `API__DefaultLimit` | `10` | Fallback pagination size. |
| Maximum limit | `API__MaxLimit` | `500` | Hard cap to protect the database. |
| Require API key | `API__EnableApiKey` | `false` | Enable header-based auth. |
| API key secret | `API__ApiKey` | _(empty)_ | Value matched against `X-Api-Key`. |
| Allowed CORS origins | `API__Cors__Origins` | `[]` | CSV list of origins, e.g. `https://example.com`. |

> 🔐 Keep secrets out of Git: `dotnet user-secrets`, container secrets, or your cloud provider.

## Local development

```bash
# Format & analyzers
dotnet format

# Unit tests
dotnet test

# Hot reload
dotnet watch --project src/CoreProtect.Api/CoreProtect.Api.csproj run
```

Recommended IDE extensions: C# Dev Kit / Rider, SQL syntax highlighting, EditorConfig support.

## Docker

```bash
# Build
docker build -t coreprotect-api .

# Run
docker run -p 8080:8080 \
  -e ConnectionStrings__CoreProtect="Server=db;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=secret;SslMode=None" \
  -e API__EnableApiKey=true \
  -e API__ApiKey=change-me \
  coreprotect-api
```

Liveness: `/healthz` · Readiness: `/readyz` (validates CoreProtect schema tables).

## API catalogue

| Endpoint | Purpose |
| -------- | ------- |
| `GET /v1/worlds` | Enumerate worlds. |
| `GET /v1/users` | Enumerate users with UUIDs and first-seen timestamps. |
| `GET /v1/users/search?name=` | Fuzzy username search (≤50 results). |
| `GET /v1/users/resolve?name=` | Resolve historical usernames. |
| `GET /v1/username-log` | Username change history joined with the latest handle. |
| `GET /v1/maps/art` | Painting identifier to art name mapping. |
| `GET /v1/maps/blockdata` | Block data ID mapping. |
| `GET /v1/maps/entities` | Entity type ID dictionary. |
| `GET /v1/maps/materials` | Material ID dictionary. |
| `GET /v1/blocks` | Block placement & break history. |
| `GET /v1/containers` | Container interactions with metadata decoding and material names. |
| `GET /v1/items` | Item pickup/drop events with material names. |
| `GET /v1/entities` | Raw entity snapshots (Java-serialized payload). |
| `GET /v1/skulls` | Player head ownership history. |
| `GET /v1/commands` | Command executions (`message → command`). |
| `GET /v1/chat` | Chat messages. |
| `GET /v1/sessions` | Login/logout events. |
| `GET /v1/signs` | Sign placements with coloured text. |
| `GET /v1/database/locks` | Current CoreProtect database lock status. |
| `GET /v1/database/versions` | CoreProtect version history. |

Shared parameters: `limit`, `offset`, `sort`, `from`, `to`, `world`, `user`, `userLike`, coordinate ranges (`xMin/xMax`, `yMin/yMax`, `zMin/zMax`). Responses expose both Unix (`time`) and ISO-8601 (`timestamp`).

The mapping endpoints reuse `limit/offset`; timestamped tables (`/v1/entities`, `/v1/skulls`, `/v1/database/*`) honour optional `from`/`to` Unix seconds just like log queries. Containers and items now return `itemMaterial` alongside numeric IDs for easier lookups.

## Security & operations

* **Least privilege** — dedicate a `cp_reader` account with `SELECT` only.
* **Auth** — enable `API__EnableApiKey`, rotate `API__ApiKey` regularly.
* **Observability** — structured logs, slow query warnings (≥500 ms), health probes for orchestrators.
* **Timeouts** — SQL 5 s, HTTP 10 s by default; tune via configuration for large deployments.
* **Metadata decoding** — Java serialization → NBT → raw payload fallback (`metaJson` + original bytes preserved).

## Troubleshooting

| Symptom | Fix |
| ------- | --- |
| `Не удалось найти тип или имя пространства имен "Configuration"` | Add the abstractions package: `dotnet add src/CoreProtect.Infrastructure/CoreProtect.Infrastructure.csproj package Microsoft.Extensions.Configuration.Abstractions`. |
| `Не удалось найти тип или имя пространства имен "IOptionsMonitor<>"` | Ensure the project references `Microsoft.Extensions.Options` and re-run `dotnet restore`. |
| `Reference assembly ... CoreProtect.Infrastructure.dll could not be found` | Build the solution to generate the reference: `dotnet clean && dotnet build`. |
| `Не удалось найти файл метаданных ... CoreProtect.Infrastructure.dll` | Same as above; make sure the Infrastructure project builds successfully before referencing it. |
| Values from environment variables are ignored | Check casing: `API__Cors__Origins` not `Api__Cors__Origins`. Use `dotnet user-secrets list` to verify overrides. |
| Database connection fails | Validate network reachability, firewall rules, and that the MySQL user has only `SELECT` on the CoreProtect schema. |

For deeper diagnostics run `dotnet build -v n` or `dotnet build -bl` (binary log) and inspect with MSBuild Structured Log Viewer.

## License

MIT
