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

## 2026-10-25 - Week 6 Destination ETL Pipeline

- **Task:** Implement and verify a small ETL workflow for sourced destination and activity data.
- **Prompt/authorization:** The user asked to complete the course assignments using the supplied references and to continue code-first.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Keep the workflow reproducible and local: validate/normalize the raw CSV, write cleaned outputs, load PostgreSQL tables idempotently, and report actual data-quality counts.
- **What was actually used:** `scripts/transform_destination_research.py`, `scripts/load_destination_data.sql`, `scripts/run_destination_etl.sh`, and `docs/data-pipeline-v0.md`. The isolated `teleport_etl_demo` database is the target; credentials are not committed.
- **Changes made by the student:** The student reviewed the project data; formatting edits to cleaned CSV files preserve the same parsed records.
- **Verification:** Seven raw rows produced two destinations and five activities. Blank required values, duplicate keys, orphan activities, and invalid rows counted zero. A repeat load inserted zero additional records.
- **Limitations:** This is a local demonstration pipeline, not a cloud-hosted ETL service. No Azure resources were provisioned.

## 2026-10-25 - Week 7 SQL Analysis and Data Quality

- **Task:** Add assignment-aligned SQL analysis and run it against the current Teleport database.
- **Prompt/authorization:** The user asked to complete the course assignments and refer to the supplied assignment references.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Use read-only joins, filters, grouping/counts, SUM/AVG, and explicit data-quality checks, then report the observed results without drawing claims the data cannot support.
- **What was actually used:** Ten queries in `database/analysis/week-7-queries.sql` and the results/interpretation in `docs/analysis/week-7-results.md`.
- **Verification:** The database had two destinations and five activities, with zero trips, itinerary items, or expenses. Lisbon had three activities and Porto had two; all recorded quality-issue counts were zero. Expense aggregates are `NULL` because no expense rows exist.
- **Limitations:** The destination sample is small and does not support broad travel-market conclusions; no spending conclusion is possible without expense data.

## 2026-10-25 - Week 8 Analytics Architecture

- **Task:** Document an operational-versus-analytical architecture and a small reporting proof of concept.
- **Prompt/authorization:** The user asked to complete the course assignments and refer to the supplied assignment references.
- **AI tool used:** GitHub Copilot.
- **AI suggestion:** Keep current OLTP operations separate from proposed analytical processing, identify a minimal fact/dimension grain, and label undeployed components as proposals.
- **What was actually used:** `docs/analytics-architecture-v0.md`, `database/schema/003_reporting.sql`, and the Week 7 read-only analysis queries. The SQL adds two reporting views and two functions; the architecture proposes `FactExpense`, `DimTrip`, and `DimDate` without creating a warehouse.
- **Verification:** The reporting schema was applied to `teleport_dev`; the views/functions and analysis queries returned against the local database. No separate warehouse or cloud analytics service was created.
- **Limitations:** There are no trip or expense rows, so trip/expense reporting currently returns empty or `NULL` results. The proposed dimensional model is not deployed.

## 2026-10-25 - Docker Compose Runtime Recheck

- **Task:** Resume and verify the local containerized application for the course containerization milestone.
- **Prompt/authorization:** The user asked to continue the assignment work and save verified progress.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** The previously created Docker Compose containers were started without recreating services or resetting the database volume. A random local-only password was passed to the command environment and was not written to a file or displayed.
- **Verification:** PostgreSQL reported healthy; the API health and destination endpoints returned HTTP 200; the web root returned HTTP 200; and `scripts/smoke-test-api.sh` passed, including cleanup of its uniquely named trip and child records.
- **Limitations:** A fresh `docker compose up --build` still fails when Docker returns HTTP 500 while pulling `postgres:17-alpine`. Podman runtime verification is recorded in a later entry; the Teleport stack itself remains on Docker Compose.

## 2026-10-25 - Azure Blob Data Preparation

- **Task:** Prepare the reviewed raw and cleaned project data for the regular-project Azure Blob Storage/ADLS requirement.
- **Prompt/authorization:** The user asked to complete the requirements in the supplied assignment references.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** `scripts/upload-data-to-azure.sh` uploads the existing `data/` tree under a `teleport/` prefix in an existing Azure Blob container. It uses Azure CLI login/RBAC, stores no keys, and does not create a storage account. The data layout and prerequisites are documented in `data/README.md`.
- **Verification:** `bash -n` passed; with placeholder account/container values, the script stopped before network operations because Azure CLI was not yet installed.
- **Limitations:** No Azure login, storage account/container, cloud upload, or cloud cost was initiated. The owner reports having no Azure account. Homebrew's source build was stopped after it required a lengthy OpenSSL compile on macOS 14; Azure CLI 2.91.0 was installed in an isolated Python 3.14 virtual environment instead.

## 2026-10-25 - Podman Runtime and Docker Cleanup

- **Task:** Install and verify Podman on the Intel Mac, and remove Docker artifacts unrelated to Teleport.
- **Prompt/authorization:** The user requested Podman installation and safe removal of Docker items not used by this project.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** The current Homebrew Podman formula required arm64, so the official Podman 5.5.2 universal macOS package was used instead. Its published SHA-256 matched and macOS reported the developer signature as trusted and notarized. The Podman CLI is installed under `/opt/podman/bin`; the default machine uses 2 CPUs, 2 GB memory, and a 30 GB disk.
- **Verification:** Podman machine reports amd64 and responds to `podman info`; `podman run --rm docker.io/library/hello-world:latest` succeeded. Docker’s Teleport web/API/PostgreSQL containers remain running. Stopped gym membership and SQL Server containers, their images, finance-only images, and the orphaned gym Postgres volume were removed. `docker builder prune --all --force` removed the reclaimable build cache; the final Docker report showed 0 B build cache. Teleport’s named volume was preserved.
- **Limitations:** The Teleport Compose stack was not migrated to Podman. Azure CLI is installed but cannot be signed in because the owner reports having no Azure account; no cloud upload occurred.

## 2026-10-25 - Azure CLI Installation and Upload Preflight

- **Task:** Install Azure CLI and prepare the local workflow for the reviewed data upload.
- **Prompt/authorization:** The user requested Azure CLI installation/sign-in and identified that no Azure account was available.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** Azure CLI 2.91.0 was installed in the isolated Python 3.14 environment at `~/.local/venvs/azure-cli`. The Homebrew route was stopped after its macOS 14 source build proved lengthy; the Python installation built `cryptography` in its isolated environment.
- **Verification:** `az version` reported 2.91.0; `az storage blob upload-batch --help` confirmed the helper's flags. `scripts/upload-data-to-azure.sh` passed `bash -n`, located the venv CLI, and exited before cloud changes because no Azure login is active.
- **Limitations:** The owner reports having no Azure account. No login, resource creation, upload, or cloud cost was initiated. The helper is ready for an approved account/container once available.

## 2026-10-26 - Azure Subscription Access Check

- **Task:** Sign in to Azure CLI and verify the account can access a subscription before creating project storage.
- **Prompt/authorization:** The user clarified that they have a Vanier College Microsoft account and approved private Blob Storage in Canada East with Standard_LRS, including possible storage charges.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** Browser-based `az login --allow-no-subscriptions` completed using the isolated Azure CLI installation. No credentials or verification codes were sent through chat.
- **Verification:** The ID supplied in chat matches the Portal's `Directory ID`, not a subscription ID. `az account show` reports `state: Enabled`; a read-only `az group list` and storage-account name check return `SubscriptionNotFound`. No resource group, storage account, container, or upload was created.
- **Next step:** In Azure Portal, open **Subscriptions** and locate the actual subscription ID, or ask the school administrator whether a subscription is assigned to this directory. Retry read-only resource checks before provisioning.

## 2026-11-02 - Week 6 ETL Reverification

- **Task:** Re-run the Week 6 transform and database load without changing the project's reviewed CSV formatting.
- **Prompt/authorization:** The user asked to continue and complete the ETL assignment evidence.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** `scripts/transform_destination_research.py` wrote the transformed CSVs to a temporary directory. Their parsed records matched the reviewed files in `data/cleaned/`. The existing loader SQL was run against the isolated `teleport_etl_demo` database using those temporary outputs.
- **Verification:** Seven raw rows produced two destination rows and five activity rows. All seven staging checks reported zero data-quality issues; the idempotent insert added zero duplicate records and final counts remained 2 / 5.
- **Limitations:** This verifies the local batch pipeline only. No Azure resource or upload was created; Azure management calls currently fail with `SubscriptionNotFound` for the supplied directory ID.

## 2026-11-04 - Week 7 SQL Analysis Rerun

- **Task:** Re-run the saved Week 7 analytical and quality-check query pack for the assignment evidence.
- **Prompt/authorization:** The user asked to continue the next assignment and verify its current results.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** Read-only queries in `database/analysis/week-7-queries.sql` against local PostgreSQL `teleport_dev`.
- **Verification:** The destination/activity join returned five rows; Lisbon had three activities and Porto two. The `Lisboa` filter returned Lisbon, the category grouping showed two City destinations, all six quality checks returned zero, and expense count was zero with `SUM`/`AVG` as `NULL`.
- **Limitations:** No trips, itinerary items, or expenses exist. No records were inserted to populate analytical measures; spending conclusions remain unsupported.

## 2026-11-04 - Week 8 Reporting Demonstration

- **Task:** Verify the operational-versus-analytical reporting proof of concept for Week 8.
- **Prompt/authorization:** The user asked to complete the next assignment, including analytics architecture and a small analytical demonstration.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** Read-only queries in `database/analysis/week-8-reporting-demo.sql` against views and functions defined in `database/schema/003_reporting.sql` on `teleport_dev`.
- **Verification:** The destination activity-count view returned Lisbon=3 and Porto=2; the destination-count function returned 3 for Lisbon. Trip/expense views returned no trip rows, and the trip-expense function returned `NULL`, consistent with the current database's 0 trips and 0 expenses.
- **Limitations:** This is a reporting proof of concept over operational PostgreSQL, not a deployed warehouse. No synthetic trip or expense facts were added.

## 2026-11-04 - Final Project Security Review

- **Task:** Review repository secret handling and document the remaining security gaps for final-project evidence.
- **Prompt/authorization:** The user asked to work through the final-project requirements one at a time.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** `docs/security-and-secrets-v0.md` documents .NET User Secrets, ignored local `.env`, placeholder-only `.env.example`, per-run CI password generation, least-privilege GitHub workflow permissions, and Azure CLI login/RBAC use by the upload helper.
- **Verification:** `dotnet user-secrets list` confirmed the `ConnectionStrings:TeleportDatabase` key without exposing its value. `.env` is ignored and absent. A high-confidence scan found no AWS access-key, GitHub-token, or private-key patterns in the current workspace.
- **Limitations:** The API has no authentication/authorization, no Azure Key Vault or AWS secret manager is deployed, no cloud application is running, and the token-pattern scan is not a full historical Git-secret audit. The current build is for local PoC use only.

## 2026-11-04 - Final Project CI and Monitoring Verification

- **Task:** Add a hosted build/integration workflow and a local PostgreSQL readiness check.
- **Prompt/authorization:** The user asked to complete the final-project requirements one at a time.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** `.github/workflows/ci.yml` builds Angular and .NET, builds and starts Docker Compose with a per-run random database password, checks API liveness/database readiness/web HTTP responses, runs the API CRUD smoke test, prints Compose logs on failure, and removes the ephemeral stack. `GET /api/health/database` checks EF Core's database connectivity without returning exception details.
- **Verification:** Workflow run `41897036664` passed all steps, including Compose build, readiness, smoke test, and cleanup. Local Release API checks returned HTTP 200 for liveness and database readiness.
- **Limitations:** Monitoring evidence is GitHub Actions history, Docker logs, and health endpoints only. No Azure Monitor, Application Insights, Log Analytics, AWS CloudWatch, persistent metrics, or alerting is configured because cloud resources are not deployed.

## 2026-11-04 - Scheduled ETL Automation Verification

- **Task:** Automate the Week 6 raw-data transform/load as part of the final-project cloud automation evidence.
- **Prompt/authorization:** The user asked to complete the final-project requirements, including at least one automated workflow with input, processing, result, execution history, and failure handling.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** GitHub Actions run `41903518461` triggered from the pushed workflow. It builds the app and API, starts a temporary Compose stack with a runner-only random password, creates an isolated `teleport_etl_demo` database, runs `scripts/run_destination_etl.sh`, uploads `data/cleaned/` as a 7-day artifact, runs the API CRUD smoke test, and tears down the containers/volume.
- **Verification:** The run completed successfully; build, service readiness, ETL, artifact upload, smoke test, and cleanup steps passed. The manual local ETL verification remains in `docs/data-pipeline-v0.md`.
- **Limitations:** The workflow is GitHub-hosted automation, not Azure Data Factory or another deployed cloud data service. No Azure/AWS storage or database resource is configured; Azure subscription access remains blocked by `SubscriptionNotFound`.

## 2026-11-04 - Final Presentation Demo Guide

- **Task:** Prepare a truthful run-of-show for the final project presentation and live demonstration.
- **Prompt/authorization:** The user asked to complete the final-project requirements and review remaining work.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** `docs/final-demo-v0.md` outlines the project objective, actual local architecture, app workflow, ETL and SQL findings, CI run, security/monitoring status, limitations, and contribution attribution.
- **Verification:** The guide cites GitHub Actions run `41935325901` and actual database results. It clearly labels Azure/AWS deployment and the dimensional warehouse as not deployed.
- **Limitations:** The owner still needs to confirm additional team members, rehearse the demo, and submit screenshots/files to Omnivox.

## 2026-11-04 - Final Submission Verification

- **Task:** Verify the pushed project and pending local work, prepare the submission checklist, and test the user-facing workflows.
- **Prompt/authorization:** The user asked to verify that everything is updated and working and prepare/list submission items.
- **AI tool used:** GitHub Copilot.
- **What was actually used:** Fresh Angular production and .NET Release builds; `scripts/smoke-test-api.sh`; browser checks of home, destination details, trips, trip details, itinerary destination/activity additions, expense creation, dashboard visibility, and trip deletion. A local Podman Compose deployment was built and started; web/API/database readiness checks and the API smoke test passed. Temporary records were removed; the database returned to 2 destinations, 5 activities, and 0 trips, itinerary items, or expenses.
- **Changes made:** Replaced stale future-work copy, fixed trip details remaining in a loading state after success, and resolved itinerary foreign keys to readable destination/activity names.
- **Verification:** GitHub Actions run `41935325901` passed for commit `8ec063b`, including app builds, Compose readiness, ETL artifact, CRUD smoke tests, and cleanup. The implementation and evidence are pushed to `main`.
- **Limitations:** The Angular build reports existing bundle/style budget warnings; no dedicated frontend unit-test files were found. The first Podman Compose build required several minutes of image downloads. AWS is suspended and Azure has no accessible subscription. Trello updates and Omnivox submission remain owner actions.

## 2026-11-07 - Omnivox Evidence Status

- **Task:** Update project status after the owner confirmed the remaining screenshot evidence was submitted.
- **AI tool used:** GitHub Copilot.
- **What was updated:** Current documentation now records that the weekly screenshot sets, including Trello captures, were uploaded to Omnivox per the project owner's report.
- **Limitation:** The live Trello board itself was not accessed or independently verified; cloud deployment remains blocked as previously documented.

## 2026-11-07 - Omnivox Screenshot Submission Update

- **Task:** Update current project status after the owner completed the remaining screenshot upload.
- **Prompt/authorization:** The project owner reported that the remaining Omnivox screenshot updates were completed.
- **AI tool used:** GitHub Copilot.
- **What was updated:** README, course tracker, evidence index, and demo guide now mark the required screenshot submission complete per the owner's report. Trello and cloud-resource statuses were not changed.

## 2026-11-07 - AWS Free-Plan Closure Notice

- **Task:** Correct the cloud blocker status after the owner shared the AWS sign-in notice.
- **AI tool used:** GitHub Copilot.
- **What was updated:** Current documentation now says AWS reports the free plan ended, the account is closed, and content may be deleted after December 30, 2026. The project has no known AWS resources.
- **Decision:** The owner considers a new AWS account or paid upgrade not worthwhile for this small POC unless the instructor confirms AWS RDS is required and approves the cost. The cloud database requirement remains unmet; seek an instructor-approved alternative rather than bypassing the account closure.
