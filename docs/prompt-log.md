# Prompt Log

This log records actual AI-assisted development work for the Teleport PoC.

## 2026-09-30 - Frontend Requirements Document

- **Task:** Create the initial frontend requirements document for the Teleport Travel Discovery & Trip Planning PoC.
- **Prompt:** "please do that for me in vs code"
- **AI tool used:** GitHub Copilot
- **AI suggestion:** Create `docs/requirements/frontend-requirements.md` with planned functional requirements, non-functional requirements, planned frontend areas, and an honest implementation status.
- **What was actually used:** The requirements document was created at `docs/requirements/frontend-requirements.md` and validated for the required sections and technology references.
- **Changes made by the student:** The student reviewed and directed the project planning work and Trello organization. The requirements remain planned; no frontend implementation was claimed.
- **Evidence status:** The requirements document is real project evidence. No screenshots or test results were fabricated.

## 2026-10-05 - Angular Nx Homepage Prototype

- **Task:** Start the Angular Nx frontend stage and build the first working Teleport homepage.
- **Prompt/authorization:** The user replied "yes" to starting frontend implementation with Angular Nx, Bootstrap, and a small working homepage.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add Nx to the existing repository, generate an Angular app, add Bootstrap, and implement an initial destination-discovery view without starting the API, database, Podman, or AWS stages.
- **What was actually used:** Nx 23.2.1, Angular 21.2.25, Bootstrap 5.3.8, a responsive homepage in `apps/web`, and local sample destination content with working search and category filters. Angular 21 was selected because it supports the available Node 20.20.0 runtime; Angular 22's engine requirement did not. A browser check found one blocked Unsplash image; its URL was replaced with a verified image that loaded successfully. The first card image was prioritized after Angular reported it as the Largest Contentful Paint image.
- **Changes made by the student:** The student authorized the frontend stage. No manual code edits by the student were reported for this entry.
- **Verification:** `npx nx build web` succeeded. The editor reported no errors in the changed frontend files. The local development server returned HTTP 200 at `http://localhost:4200/`. Browser checks confirmed destination search and category filters work, all three destination images load, the Largest Contentful Paint warning is gone after prioritizing the lead image, and the document has no horizontal overflow at checked mobile and desktop widths. The build emits a non-fatal component stylesheet budget warning (5.48 kB versus the generated 4 kB warning threshold).
- **Environment note:** npm reported 11 audit findings (9 high and 2 critical) during dependency installation; these have not been remediated or individually reviewed.

## 2026-10-07 - .NET Minimal API Foundation

- **Task:** Start the API stage with a minimal .NET API host, OpenAPI, and a real health response.
- **Prompt/authorization:** The user asked to do the code before screenshots and continue with the .NET Minimal API stage.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add an empty .NET 10 Minimal API project, built-in OpenAPI documentation, a health endpoint, and CORS limited to the local Angular development origin. Defer domain entities and PostgreSQL until the data model is planned.
- **What was actually used:** `apps/api/Teleport.Api.csproj` targets .NET 10; the API exposes `GET /api/health`, generates `/openapi/v1.json`, and permits CORS requests from `http://localhost:4200`. `apps/api/Teleport.Api.http` contains requests for the actual health and OpenAPI endpoints.
- **Changes made by the student:** The student authorized continuing implementation before taking the remaining screenshots. No manual API code edits by the student were reported.
- **Verification:** `dotnet build apps/api/Teleport.Api.csproj` succeeded. A real health request returned HTTP 200 with `{"status":"ok"}`; the OpenAPI document listed `/api/health`; a CORS preflight from the Angular origin returned HTTP 204 with the expected allow-origin header.
- **Scope note:** No destination or other domain data, database, or frontend integration has been added in this API step.

## 2026-10-07 - PostgreSQL Data Model and EF Core Mapping

- **Task:** Define the minimum relational model for destinations, activities, trips, itinerary items, and expenses, then map it in the .NET API.
- **Prompt/authorization:** The user approved the five-table data/API model and chose EF Core with Npgsql. The user confirmed the initial model should omit accounts and authentication.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Keep a five-table model based on the stated features, use EF Core/Npgsql for the initial relational CRUD prototype, and defer PostgreSQL runtime setup until the planned Podman stage.
- **What was actually used:** `database/schema/001_initial.sql`, entity classes and `TeleportDbContext` under `apps/api/Data`, and `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3.
- **Changes made by the student:** The student approved the model direction, selected EF Core/Npgsql, and chose to defer local PostgreSQL installation until the Podman stage.
- **Verification:** `dotnet build apps/api/Teleport.Api.csproj` succeeded after adding the EF Core model. The SQL schema has not been executed, and database-backed requests have not been tested because no local PostgreSQL server is running.
- **Scope note:** No seed data or database credentials were added. Expense currency remains unspecified; the schema stores a positive numeric amount without currency conversion.

## 2026-10-07 - Destination Read API

- **Task:** Add read-only API routes for browsing/searching destinations and viewing one destination.
- **Prompt/authorization:** The user asked to continue implementing project code before resuming screenshot work and approved EF Core with Npgsql.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add a service-backed `GET /api/destinations` with optional search/category filters and `GET /api/destinations/{id}`, using typed results, cancellation tokens, and OpenAPI metadata.
- **What was actually used:** The two GET routes, `IDestinationService`/`DestinationService`, and a `DestinationResponse` DTO were added under `apps/api`.
- **Changes made by the student:** The student approved the EF Core/Npgsql choice and confirmed that accounts are out of scope for the initial model. No manual API code edits were reported.
- **Verification:** `dotnet build apps/api/Teleport.Api.csproj` succeeded. OpenAPI lists both destination route patterns; `/api/health` returns HTTP 200 and Angular-origin CORS preflight returns HTTP 204.
- **Limitations:** No PostgreSQL server is running locally, so destination database queries and response bodies have not been verified. No sample rows or fake request results were created.

## 2026-10-07 - Angular Destination API Client

- **Task:** Replace the homepage's in-memory destination collection with an Angular HTTP request to the destination API.
- **Prompt/authorization:** The user asked to complete implementation before returning to screenshot work.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add an Angular destination service, environment-based API URLs, and honest loading, empty, and error states; do not fall back to local sample data when the API fails.
- **What was actually used:** `provideHttpClient`, `DestinationService`, development/production API URL configuration, and destination loading/empty/error UI states in `apps/web`.
- **Changes made by the student:** The student authorized continuing with implementation before the remaining screenshot sessions. No manual frontend edits by the student were reported.
- **Verification:** `npx nx build web` succeeded. The API host and OpenAPI route remained available. A real `GET /api/destinations` returned HTTP 500 Problem Details because PostgreSQL is not running; no successful data response is claimed.
- **Limitations:** The database runtime and reviewed destination rows are pending the planned Podman/database stage, so the browser cannot yet show destination records from PostgreSQL.

## 2026-10-07 - Trip, Itinerary, Expense, and Activity API Routes

- **Task:** Add the remaining basic API operations for activities, trips, itinerary items, and expenses.
- **Prompt/authorization:** The user asked to complete project code before returning to screenshots and approved the five-table model.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add grouped Minimal API routes backed by EF Core services, typed status-code responses, cancellation support, and basic validation.
- **What was actually used:** Activity listing; trip list/detail/create/update/delete; itinerary list/add/reorder/remove; and expense list/detail/create/update/delete routes. `apps/api/Teleport.Api.http` includes request templates.
- **Changes made by the student:** The user approved the model and asked to continue the code-first work. No manual API code edits by the student were reported.
- **Verification:** `dotnet build apps/api/Teleport.Api.csproj` succeeded. No database-backed API response is claimed because PostgreSQL is not running; the routes still require runtime verification in the Podman/database stage.

## 2026-10-07 - Trips Frontend Workflow

- **Task:** Add the first trip-management page to the Angular frontend.
- **Prompt/authorization:** The user asked to continue with project code before returning to screenshot capture and agreed to keep the application simple.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Add a routed Trips page with a small reactive form, trip list, date validation, and API service calls for create, read, update, and delete.
- **What was actually used:** A lazily loaded `/trips` route, `TripService`, and a responsive page with create/edit/delete controls in `apps/web/src/app/trips`.
- **Changes made by the student:** The student authorized code-first continuation. No manual frontend edits by the student were reported.
- **Verification:** `npx nx build web` succeeded. The production build reports the initial bundle at 501.18 kB against a 500 kB warning budget and the root stylesheet at 5.53 kB against a 4 kB warning budget; both are warnings, not build failures.
- **Limitations:** The Trips page calls the real API but cannot load or save trips until PostgreSQL is running. The database runtime remains deferred to the Podman stage.
- **Environment note:** Installing `@angular/forms` reported 13 npm audit findings (11 high, 2 critical). They have not been individually reviewed or remediated.

## 2026-10-14 - Course Milestone Gap Closure and Local Data Flow

- **Task:** Complete the unfinished work described in the supplied Week 2, Week 3, and Week 5 instructions, while keeping the existing project brief's POC and Podman/AWS direction.
- **Prompt:** "lets do all tasks we havent done in all weeks of instruction"
- **AI tool used:** GitHub Copilot in VS Code.
- **Student direction:** The student selected the existing project brief's Podman + AWS PostgreSQL path rather than the course's Azure regular-project path.
- **What was actually used:** Refined project and data-collection requirements; architecture and sequence diagrams; sourced destination/activity CSVs and idempotent PostgreSQL seed SQL; an API smoke-test script that creates and removes only its own test trip; API/web Dockerfiles, Compose, and Nginx proxy configuration; and a course-progress tracker.
- **Changes made:** Seeded two destinations and five activities into local `teleport_dev`. Updated outdated frontend copy to match the implemented trip features. Did not modify the pre-existing trip. Did not provision AWS resources, update Trello/Omnivox, or push to GitHub because cloud costs/permissions and account/board/remote access are not available here.
- **Verification:** `dotnet build apps/api/Teleport.Api.csproj` succeeded with zero warnings/errors. `npx nx build web` succeeded with existing initial bundle and app stylesheet budget warnings. The API smoke test passed health, destination/search/activity reads, invalid-input validation, trip create/read/delete, itinerary add/list/reorder/delete, and expense create/list/update/delete; all test records were removed. Browser testing confirmed the UI created a test trip, added and reordered itinerary items, added and edited an expense, and removed those test records. Local counts after cleanup were two destinations, five activities, one pre-existing trip, zero itinerary items, and zero expenses. The Compose database also has zero test trips, itinerary items, or expenses after cleanup. Compose configuration validation passed.
- **Container/cloud status:** Docker Desktop built and started the API, web, and PostgreSQL Compose services. The database health check passed, the web container served Angular and proxied `/api`, and the complete API smoke test passed against the container database. Podman-specific execution is not claimed. AWS RDS remains unprovisioned pending explicit cost, region, sizing, and network approval.
- **Saved evidence:** `docs/frontend-destination-discovery.png`, `docs/frontend-trip-planning.png`, `docs/api-destinations-response.png`, `docs/openapi-document.png`, `docs/requirements-document.png`, and `docs/prompt-log-document.png` capture the frontend, integration flow, API response, OpenAPI document, requirements, and Prompt Log. The trip-planning image shows the temporary demonstration trip before it was deleted. Omnivox upload remains outstanding. Trello and GitHub remote access are also not configured.
