# Teleport - Travel Discovery & Trip Planning PoC

## Project Description

Teleport is a Proof of Concept travel website for students, young adults, and personal travelers. It is intended to support travel discovery and basic trip planning through a clear, responsive user experience.

## Project Goal

The PoC will demonstrate destination discovery, trip planning, itinerary management, and basic travel expense tracking. The project will be developed incrementally according to the planned sprints and documented requirements.

## Project Team

- Project owner and repository owner: Michael James Tenorio.
- Additional team members were not identified in the project materials available here; add them before final submission if applicable.

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
- Podman Compose (local deployment); Docker Compose (GitHub Actions CI)
- GitHub Actions (build, integration checks, scheduled ETL, and short-lived ETL artifact)
- AWS PostgreSQL and Azure Blob Storage/ADLS (planned, not provisioned)

The original project brief requires Podman and AWS PostgreSQL. The regular-project course brief adds Docker and Azure storage preparation. These are additive for this project; the separate AWS-to-Azure migration track does not apply.

## Architecture

The implemented local application architecture is:

```text
Traveler -> Angular/Nx + Bootstrap -> .NET Minimal API -> PostgreSQL (teleport_dev)
```

The automated data flow is:

```text
Reviewed raw CSV -> GitHub Actions (push / pull request / weekly / manual)
                                          -> Python validation and transformation
                                          -> isolated PostgreSQL ETL database
                                          -> cleaned CSV artifact (7-day retention)
```

The detailed operational/analytical design and its proposed, undeployed dimensional model are in [docs/analytics-architecture-v0.md](docs/analytics-architecture-v0.md). The ETL design and measured results are in [docs/data-pipeline-v0.md](docs/data-pipeline-v0.md).

## Setup and Run

### Local prerequisites

- Node.js 20 and npm
- .NET 10 SDK
- Podman 5.5.2 or Docker with Compose
- `psql` client for schema, query, and ETL commands

### Configure the local database and API

Create the local database and apply its schema and reviewed demo records:

```sh
createdb teleport_dev
psql -v ON_ERROR_STOP=1 -d teleport_dev -f database/schema/001_initial.sql
psql -v ON_ERROR_STOP=1 -d teleport_dev -f database/seed/002_demo_data.sql
psql -v ON_ERROR_STOP=1 -d teleport_dev -f database/schema/003_reporting.sql
```

Store the local API connection string in .NET User Secrets, replacing the placeholder locally; never commit the real value:

```sh
dotnet user-secrets set "ConnectionStrings:TeleportDatabase" "<local PostgreSQL connection string>" --project apps/api/Teleport.Api.csproj
```

Run the API and frontend in separate terminals:

```sh
dotnet run --project apps/api/Teleport.Api.csproj --launch-profile http
npm ci
npx nx serve web
```

The API listens on `http://localhost:5079`; the Angular frontend listens on `http://localhost:4200`. With both services and PostgreSQL running, validate the API and CRUD flow using:

```sh
bash scripts/smoke-test-api.sh
```

### Run the destination ETL locally

The reproducible runner applies the schema, transforms the reviewed CSV, and loads PostgreSQL. Set `PGDATABASE` to the isolated ETL target. The runner writes regenerated files to `data/cleaned/`; use a temporary output directory when you need to preserve local formatting edits.

```sh
PGHOST=localhost PGPORT=5432 PGDATABASE=teleport_etl_demo PGUSER="$(id -un)" bash scripts/run_destination_etl.sh
```

The reporting demonstration is read-only:

```sh
psql -h localhost -p 5432 -d teleport_dev -v ON_ERROR_STOP=1 -P pager=off -f database/analysis/week-7-queries.sql
psql -h localhost -p 5432 -d teleport_dev -v ON_ERROR_STOP=1 -P pager=off -f database/analysis/week-8-reporting-demo.sql
```

### Run the container stack

Copy the example file, replace its placeholder with a unique local-only password, then start the stack:

```sh
cp .env.example .env
# Edit .env and set POSTGRES_PASSWORD to a local-only value.
podman compose up --detach --build
```

Use `docker compose` instead if Docker Desktop is running. The app is at `http://localhost:4200`, the API at `http://localhost:5079`, and PostgreSQL at `localhost:55432`. Stop the stack with `podman compose down`; this keeps the database volume. Add `--volumes` only when intentionally deleting the local data.

## Results and Limitations

- Reviewed demonstration data: 2 destinations and 5 activities. The ETL transform validates 7 raw rows and is idempotent.
- Current operational database: 0 trips, 0 itinerary items, and 0 expenses. Therefore no real spending analysis is possible; expense `SUM`/`AVG` are `NULL`, not measured zero spending.
- The business interpretation is limited to curated activity coverage (Lisbon 3, Porto 2); this tiny sample does not establish popularity, quality, cost, or traveler preference.
- The GitHub Actions workflow builds the app/API, starts Docker Compose with an ephemeral password, checks readiness, runs the ETL in an isolated database, uploads a 7-day artifact, runs CRUD smoke tests, and cleans up. Run [41935325901](https://github.com/michael10orio/teleport-travel-website/actions/runs/41935325901) passed for implementation/evidence commit `8ec063b`; see the [CI workflow](https://github.com/michael10orio/teleport-travel-website/actions/workflows/ci.yml) for newer documentation-only runs.
- The local Podman Compose deployment has been built and tested. Web, API liveness, database readiness, and destinations returned HTTP 200; the API CRUD smoke test passed and removed its temporary records.
- The final presentation/demo sequence is outlined in [docs/final-demo-v0.md](docs/final-demo-v0.md).
- Sprint and Week 6–8 screenshots, including weekly Trello captures, are grouped in [docs/evidence/README.md](docs/evidence/README.md); the project owner reports uploading the required evidence to Omnivox.
- The AWS console reports the free plan ended and the account is closed, with content deletion scheduled after December 30, 2026. No project AWS database or CloudWatch resources are deployed. Azure sign-in works, but management returns `SubscriptionNotFound`. Ask the instructor about an alternative; upgrading AWS may incur charges.
- The API has no authentication/authorization and is not production-ready. See [docs/security-and-secrets-v0.md](docs/security-and-secrets-v0.md) and [docs/monitoring-v0.md](docs/monitoring-v0.md) for security and monitoring scope.

The project is intentionally a small PoC. Data and results are described only where they have been locally verified; proposed cloud and warehouse layers are labeled as plans.

## Development Principles

The project will be developed honestly and incrementally. Documentation, evidence, test results, and progress claims will be added only when the corresponding work has actually been completed.
