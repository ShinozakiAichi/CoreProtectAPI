# CoreProtect API

Read-only REST API that exposes CoreProtect audit data via HTTP. The service targets CoreProtect MySQL/MariaDB backends and surfaces blocks, container interactions, item transactions, commands, chat, sessions, and signs with decoded metadata.

## Features

* REST endpoints under `/v1` with consistent filtering for world, users (including historical usernames), time ranges, coordinates, and pagination.
* Safe SQL templates that respect CoreProtect indexing strategy and never touch `rowid`.
* Binary metadata decoding (Java Serialization subset + NBT) with graceful fallback to raw hex/base64.
* API key protection via `X-Api-Key`, structured error responses, and basic health probes (`/healthz`, `/readyz`).
* Slow SQL logging (>500 ms), HTTP request logging, and configurable limits via environment variables.
* Docker-ready `.NET 8` service with clean layering (Domain → Application → Infrastructure → API).

## Project structure

```
CoreProtectAPI
├── CoreProtectAPI.sln
├── README.md
├── src/
│   ├── CoreProtect.Domain/       # Entities and value objects
│   ├── CoreProtect.Application/  # Query services and abstractions
│   ├── CoreProtect.Infrastructure/ # MySQL access, metadata decoding, DI
│   └── CoreProtect.Api/          # ASP.NET Core host and controllers
└── tests/
    └── CoreProtect.Tests/        # xUnit test suite
```

## Getting started

### Requirements

* .NET SDK 8.0
* MySQL or MariaDB with CoreProtect schema accessible via read-only user (`cp_reader`).

### Configuration

Configuration is provided through `appsettings.json` and overridable environment variables.

| Setting | Environment key | Default | Description |
| ------- | ---------------- | ------- | ----------- |
| Connection string | `ConnectionStrings__CoreProtect` | `Server=localhost;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=change-me;SslMode=None` | CoreProtect database connection. |
| Default limit | `API__DefaultLimit` | `10` | Default page size when `limit` is omitted. |
| Max limit | `API__MaxLimit` | `500` | Maximum allowed `limit`. |
| API key toggle | `API__EnableApiKey` | `false` | Enables header based authentication. |
| API key value | `API__ApiKey` | _empty_ | Value compared against `X-Api-Key`. |
| CORS origins | `API__Cors__Origins` | `[]` | Comma-separated list of allowed origins. |

### Running locally

```bash
# Restore dependencies
dotnet restore

# Run the web API
dotnet run --project src/CoreProtect.Api/CoreProtect.Api.csproj
```

### Docker

```
docker build -t coreprotect-api .
docker run -p 8080:8080 \
  -e ConnectionStrings__CoreProtect="Server=db;Port=3306;Database=coreprotect;Uid=cp_reader;Pwd=secret;SslMode=None" \
  -e API__EnableApiKey=true \
  -e API__ApiKey=change-me \
  coreprotect-api
```

### Testing

```bash
dotnet test
```

> **Note:** Ensure the .NET 8 SDK is installed before running the commands above.

## API overview

* `GET /v1/worlds` – list available worlds.
* `GET /v1/users/search?name=...` – partial username search (max 50 results).
* `GET /v1/users/resolve?name=...` – resolve historical usernames.
* `GET /v1/blocks` – block place/break history.
* `GET /v1/containers` – container interactions with metadata decoding.
* `GET /v1/items` – item pickup/drop events.
* `GET /v1/commands` – command executions (`message` returned as `command`).
* `GET /v1/chat` – chat messages.
* `GET /v1/sessions` – login/logout events.
* `GET /v1/signs` – sign placements/updates with lines and colors.

All endpoints accept shared query parameters:

| Parameter | Description |
| --------- | ----------- |
| `limit` | Page size (1–500, default configured via `API__DefaultLimit`). |
| `offset` | Offset for pagination (default 0). |
| `sort` | `asc` or `desc` (default `desc`). |
| `from` / `to` | Unix seconds (inclusive/exclusive). |
| `world` | Exact world name. |
| `user` | Exact username (matches current or historical). |
| `userLike` | Case-insensitive substring match. |
| `xMin`–`xMax`, `yMin`–`yMax`, `zMin`–`zMax` | Coordinate bounds. |

Endpoint specific filters are available (e.g. `action`, `blockTypeId`, `command`, `message`). Responses include both Unix time (`time`) and ISO-8601 (`timestamp`) values.

## Security and operations

* Create a dedicated database user (`cp_reader`) with `SELECT` privileges only.
* Enable the API key header check for public deployments.
* Monitor slow query logs – anything over 500 ms is logged at `Warning` level.
* Health probes: `/healthz` for liveness, `/readyz` for readiness.

## Metadata decoding

The metadata decoder attempts, in order:

1. Java serialization (supporting `HashMap`, `LinkedHashMap`, `ArrayList`, enums, Guava immutable map payloads).
2. NBT (uncompressed, Java edition layout).
3. Fallback to truncated hex/base64.

Decoded JSON is emitted inline (`metaJson`), with raw payloads provided separately when decoding fails.

## Indexing and performance

The SQL queries assume the CoreProtect indices listed in the technical specification. Ensure they exist to maintain the SLO target (≤300 ms p95 for typical queries). Timeout defaults: 5 s for SQL commands, 10 s for HTTP requests.

## License

MIT
