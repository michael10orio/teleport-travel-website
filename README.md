# Teleport - Travel Discovery & Trip Planning PoC

## Project Description

Teleport is a Proof of Concept travel website for students, young adults, and personal travelers. It is intended to support travel discovery and basic trip planning through a clear, responsive user experience.

## Project Goal

The PoC will demonstrate destination discovery, trip planning, itinerary management, and basic travel expense tracking. The project will be developed incrementally according to the planned sprints and documented requirements.

## Scope

Planned functionality includes:

- Browse, search, filter, and view destination details.
- View activities associated with destinations.
- Create, view, update, and delete trips.
- Organize destinations and activities into a basic itinerary.
- Add, view, update, and delete expenses associated with trips.
- View relevant trip information in a simple dashboard.

This PoC does not include flight booking, hotel booking, payment processing, real-time services, real-time flight or hotel pricing, complex AI features, or a complex recommendation engine.

## Technology Stack

- Angular with Nx
- Bootstrap
- .NET Minimal API
- PostgreSQL
- Podman
- AWS-hosted PostgreSQL (planned)

The project follows its POC brief and the Podman/AWS direction selected for this work. The container definitions use OCI-compatible Dockerfiles and Compose; Podman itself still needs verification in an environment where it is installed.

## Architecture

Local development and the planned hosted database path are:

```text
Browser
   | HTTP / JSON
   v
Angular / Nginx
   | /api proxy
   v
.NET Minimal API
   | EF Core / Npgsql
   v
PostgreSQL (local development)
   |
   +---- later: AWS-hosted PostgreSQL
```

The course's Azure Blob/ADLS and AWS-to-Azure migration checklists were not selected: this project is a POC following the original Podman/AWS brief, not an AWS-to-Azure migration.

## Project Status

**Current stage: Week 2/3 frontend and API flows implemented; Week 5 local containerization and cloud preparation in progress.**

Implemented and verified:

- Refined project context, functional/non-functional requirements, and user-entered field validation in `docs/requirements/frontend-requirements.md`.
- Documented API contracts, relational model, architecture, and frontend/API/database sequence flows in `docs/data-and-api-v0.md`.
- Added reviewed destination/activities source notes and normalized CSVs under `data/`; seeded two destinations and five activities into local PostgreSQL with an idempotent script.
- Angular destination discovery, trip create/edit/delete, and trip detail itinerary and expense workflows are wired to the .NET API.
- The local PostgreSQL 18.1 `teleport_dev` database has the five expected tables. Current counts: two destinations, five activities, one pre-existing trip, zero itinerary items, and zero expenses.
- `scripts/smoke-test-api.sh` passed health, destination/search/activity reads, validation, trip CRUD, itinerary CRUD/order update, and expense CRUD. It created and removed only a uniquely named test trip; the pre-existing trip was preserved.
- Browser verification confirmed destination data loads and the UI can create a trip, add/reorder/remove itinerary entries, and add/edit/delete an expense. Temporary browser-test records were removed.
- `dotnet build apps/api/Teleport.Api.csproj` and `npx nx build web` succeeded. The web build reports existing bundle and stylesheet budget warnings.
- API/web Dockerfiles, Nginx API proxy, `.dockerignore`, and `compose.yaml` built and ran successfully with Docker Desktop. The web container served the frontend through Nginx, the API container returned HTTP 200, and the full API smoke test passed against the Compose PostgreSQL container.

Not yet implemented:

- Podman-specific runtime verification; Podman is not installed, so the compatible Compose setup was exercised with Docker Desktop.
- AWS-hosted PostgreSQL provisioning and API connection; this requires account, region, sizing, network, and cost approval.
- Actual Trello board updates, GitHub push, and Omnivox submission; no Trello URL or Git remote is configured here.
- Dashboard and formal use-case document.

See [`docs/course-progress.md`](./docs/course-progress.md) for the supplied Week 2, Week 3, and Week 5 checklist status and evidence limitations. The API health endpoint checks only host liveness; database readiness was verified using real destination and CRUD requests.

## Local Container Development

The Compose setup builds the Angular/Nginx frontend and .NET API and starts them with a fresh PostgreSQL service. On macOS, start Podman or Docker Desktop first.

```sh
cp .env.example .env
# Edit .env and replace POSTGRES_PASSWORD with a local-only password.
docker compose up --build
```

- Frontend: `http://localhost:4200`
- API health: `http://localhost:5079/api/health`
- Development OpenAPI: `http://localhost:5079/openapi/v1.json`
- PostgreSQL is published only on loopback, default port `55432`.
- The PostgreSQL volume persists between runs. The schema and seed scripts run only when the volume is first initialized.
- `docker compose down` stops the services and preserves the database volume. Do not use `docker compose down -v` unless you intend to erase this local container database.
- After the API is available and demo data exists, run `bash scripts/smoke-test-api.sh` to validate routes. It creates a uniquely named trip and removes only that test record.

## Development Principles

The project will be developed honestly and incrementally. Documentation, evidence, test results, and progress claims will be added only when the corresponding work has actually been completed.
