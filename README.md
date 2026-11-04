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
- Podman (local amd64 runtime verified); Docker Compose (application containers and CI)
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
- PostgreSQL 18 (or the Docker Compose PostgreSQL 17 service)
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

### Run with Docker Compose

Create a local `.env` from `.env.example`, replace its placeholder with a unique local-only password, then start the stack:

```sh
docker compose up --detach --build
```

The Compose stack binds its ports to localhost. Stop containers without deleting the database volume using `docker compose down`; removing volumes destroys their stored database data.

## Results and Limitations

- Reviewed demonstration data: 2 destinations and 5 activities. The ETL transform validates 7 raw rows and is idempotent.
- Current operational database: 0 trips, 0 itinerary items, and 0 expenses. Therefore no real spending analysis is possible; expense `SUM`/`AVG` are `NULL`, not measured zero spending.
- The business interpretation is limited to curated activity coverage (Lisbon 3, Porto 2); this tiny sample does not establish popularity, quality, cost, or traveler preference.
- The GitHub Actions workflow builds the app/API, starts Docker Compose with an ephemeral password, checks service readiness, runs the ETL into a separate database, uploads cleaned data as a 7-day artifact, runs CRUD smoke tests, and cleans up. Successful run: [41903518461](https://github.com/michael10orio/teleport-travel-website/actions/runs/41903518461).
- Azure CLI sign-in succeeded, but Azure management calls return `SubscriptionNotFound`; AWS and Azure resources are not provisioned. No cloud monitoring service is deployed.
- The API has no authentication/authorization and is not production-ready. See [docs/security-and-secrets-v0.md](docs/security-and-secrets-v0.md) and [docs/monitoring-v0.md](docs/monitoring-v0.md) for security and monitoring scope.

## Implementation Status

- Angular/Nx frontend supports destination discovery/details, trip planning, itinerary and expense management, and dashboard views.
- The .NET 10 Minimal API provides OpenAPI, destination/activity reads, and trip, itinerary, and expense CRUD. Local API/database smoke tests passed and removed their temporary records.
- PostgreSQL has the five-table schema and a small sourced dataset: 2 destinations and 5 activities. No trips, itinerary items, or expenses remain.
- Week 6 ETL is verified and idempotent: 7 raw rows become 2 destinations and 5 activities in `teleport_etl_demo`.
- Week 7 has ten analysis/data-quality queries and a results interpretation; Week 8 has two reporting views, two functions, and an OLTP/OLAP design. No warehouse is deployed.
- GitHub Actions run `41903518461` passed frontend/API builds, Compose build/start, API/database/web readiness, scheduled ETL, 7-day cleaned-data artifact upload, CRUD smoke tests, and cleanup.
- Podman 5.5.2 is installed and its amd64 `hello-world` test passed; the Teleport stack itself is still run with Docker Compose. The Angular build succeeds with non-fatal bundle/style budget warnings.

## Remaining Requirements and Limitations

- AWS PostgreSQL and Azure Blob/ADLS are not provisioned. Azure CLI sign-in succeeded, but Azure management calls return `SubscriptionNotFound` because the ID supplied was the Vanier directory ID, not a subscription ID. No cloud resources or uploads were created.
- The API has no authentication/authorization and no cloud secret manager is deployed; this is a local PoC, not production-ready. See [docs/security-and-secrets-v0.md](docs/security-and-secrets-v0.md).
- Azure Monitor, Application Insights, Log Analytics, CloudWatch, persistent metrics, and alerting are not configured. Local health endpoints, Docker logs, and GitHub Actions history are documented in [docs/monitoring-v0.md](docs/monitoring-v0.md).
- The small dataset supports only limited activity-coverage observations. With zero trip/expense records, no spending conclusion is supported.
- Trello updates, screenshot review, and Omnivox submission remain project-owner tasks; screenshot files are not treated as submitted evidence.
- Week 6–8, CI, and monitoring milestones are pushed. Other frontend, CSV, API-plan, and Azure-helper edits remain uncommitted and need review before final repository submission.

The project is intentionally a small PoC. Data and results are described only where they have been locally verified; proposed cloud and warehouse layers are labeled as plans.

## Development Principles

The project will be developed honestly and incrementally. Documentation, evidence, test results, and progress claims will be added only when the corresponding work has actually been completed.
