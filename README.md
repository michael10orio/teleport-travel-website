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
- AWS-hosted PostgreSQL (planned for a later stage)

These technologies are required by the project brief. They will be introduced during the appropriate implementation stages.

## Architecture

The initial planned architecture is:

```text
Angular Nx + Bootstrap
        |
     HTTP / JSON
        v
.NET Minimal API
        |
       SQL
        v
   PostgreSQL
```

Podman containerization and AWS-hosted PostgreSQL are planned for later project stages. Architecture documentation will be expanded as the system is implemented.

## Project Status

**Current stage: Sprint 3 API and data-model foundation.**

Completed in this stage:

- Repository setup files created.
- Initial `.gitignore` added for the planned technology stack and local development files.
- This README added with the project scope, planned stack, and initial architecture.
- Git repository initialized with the initial setup commit.
- Frontend requirements document created at `docs/requirements/frontend-requirements.md`.
- Prompt Log created at `docs/prompt-log.md` with an actual AI-assisted development entry.
- Nx 23.2.1 initialized in the existing repository.
- Angular 21.2.25 application created under `apps/web` with Bootstrap 5.3.8.
- Responsive destination-discovery homepage with search/category filters and an environment-configured HTTP service for destination data.
- Production build completed successfully; local development server returned HTTP 200.
- .NET 10 Minimal API project created under `apps/api`.
- Built-in OpenAPI document and `GET /api/health` endpoint added.
- Destination list/search and destination-by-ID GET routes added to the API source.
- Destination activity listing, trip CRUD, itinerary management, and expense CRUD routes added to the API source.
- CORS configured for the local Angular development origin.
- API build, health response, OpenAPI route metadata, and local CORS preflight verified.
- Five-table PostgreSQL schema and matching EF Core/Npgsql model drafted in `database/schema/001_initial.sql` and `apps/api/Data`.

Not yet implemented:

- A running PostgreSQL server, applied schema/migrations, or verified database connection.
- A successful database-backed destination response; the GET routes require PostgreSQL to run.
- A running PostgreSQL server, applied schema/migrations, or verified database connection.
- Successful database-backed API responses; all data routes currently require PostgreSQL to run.
- Verified frontend-to-database-backed-API flow.
- Trip creation, itinerary, expense, and dashboard workflows.
- Podman configuration.
- AWS resources or credentials.
- Formal use-case and ERD documents.
- Live destination data; the homepage currently uses clearly labeled sample content.

The next project step is to run PostgreSQL during the planned Podman/database stage, apply and verify the schema, then exercise the CRUD routes and frontend flow against real database data. The current health endpoint does not prove database connectivity. AWS remains a later project stage.

## Development Principles

The project will be developed honestly and incrementally. Documentation, evidence, test results, and progress claims will be added only when the corresponding work has actually been completed.
