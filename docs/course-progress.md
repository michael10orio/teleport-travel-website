# Course Milestone Progress

This tracker maps the supplied Week 2, Week 3, and Week 5–8 instructions. The Week 5 brief labels its work as Week 6 of the course and Week 5 of the project. No detailed Week 1 or Week 4 assignment checklist was supplied, so those details are not invented here.

## Week 1 foundation prerequisites

| Foundation item | Status | Evidence |
| --- | --- | --- |
| Project idea, business problem, and scope | Documented | `README.md` and `requirements/frontend-requirements.md`. |
| Target users and project type | Documented | `requirements/frontend-requirements.md`; POC. |
| Initial architecture | Documented and aligned to the chosen local Podman / planned AWS PostgreSQL direction | `data-and-api-v0.md`. |
| Functional and non-functional requirements | Documented | `requirements/frontend-requirements.md`. |
| Git repository and development history | Local repository and milestone commits exist; `origin` is configured. Current working-tree edits remain uncommitted and have not been pushed. | `git log`, `git remote -v`, and `git status`; review and commit the pending edits before claiming they are on GitHub. |
| Trello board | Created by the project owner; not changed by the assistant | The owner reported a board named `Travel Website – PoC` with Backlog, To Do, Doing, Review, Testing, and Done lists. No board URL or direct integration is available here. The owner asked to manage the board personally. |

## Week 2 — requirements, frontend data collection, AI prompting

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Refine functional requirements and define user-entered fields | Complete | `requirements/frontend-requirements.md` contains project context, required/optional fields, validation, and user-visible results. |
| Create a frontend prototype and working data-collection flow | Implemented | Angular destination search, trip form, itinerary, and expense screens. Real API verification is recorded separately. |
| Document meaningful AI assistance and validation | Complete for this milestone | `prompt-log.md` records the prompt, implementation, and verification performed. |
| Keep Trello tasks and statuses current | Blocked on access | Update the actual board manually or provide an accessible board/workflow. |
| Capture frontend/requirements/Prompt Log screenshots for Omnivox | Captured locally; submission pending | `frontend-destination-discovery.png`, `frontend-trip-planning.png`, `requirements-document.png`, and `prompt-log-document.png` are saved under `docs/`; the project owner must upload evidence to Omnivox. |

## Week 3 — API, data flow, and integration

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Derive API from frontend actions and define request/response contracts | Complete | `data-and-api-v0.md`, endpoint source, and `apps/api/Teleport.Api.http`. |
| Document a real data-flow diagram | Complete | Mermaid sequence diagrams in `data-and-api-v0.md`. |
| Implement API, OpenAPI, validation, and error responses | Implemented and locally verified | .NET Minimal API routes; `scripts/smoke-test-api.sh` passed validation and CRUD responses. |
| Connect frontend to API | Implemented and browser verified | Angular services use the configured API base URL; the browser created and removed a test trip and exercised itinerary and expense operations. |
| Capture API/Swagger and integration screenshots | Captured locally; submission pending | `api-destinations-response.png` and `openapi-document.png` capture the live API response and OpenAPI document; `frontend-trip-planning.png` captures the integrated workflow. The project owner must upload evidence to Omnivox. |

## Week 5 project track — original stack plus regular-project storage requirements

The original project brief requires **Podman + AWS PostgreSQL**. The supplied regular-project course brief additionally requires **Docker** containerization and preparation for **Azure Blob Storage or ADLS**. Treat these as additive requirements; the separate AWS-to-Azure migration track does not apply. Cloud resource creation and uploads remain pending account access and cost approval.

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Prepare app/API for containers | Files prepared | API and web Dockerfiles, Compose configuration, and `.dockerignore`. |
| Build/run and verify the API in a container | Docker Compose stack verified; Podman runtime verified separately | Teleport’s existing Compose stack is healthy; web/API requests return HTTP 200 and `scripts/smoke-test-api.sh` passes. A fresh `docker compose up --build` still fails pulling `postgres:17-alpine` with Docker engine HTTP 500. Podman 5.5.2 is installed from its notarized universal package on Intel macOS; its 2 GB machine and `hello-world` container pass, but the Teleport stack has not been run under Podman. |
| Prepare and organize project data | Complete for a small demonstration set | Sourced research, normalized CSVs, and idempotent seed SQL under `data/` and `database/seed/`; Azure upload script maps the prepared tree to an existing container. |
| Cloud database and API connection | Not provisioned | AWS RDS setup needs explicit region, size, network, and cost approval. No credentials or cloud resources are stored in the repo. |
| Azure Blob/ADLS storage | Upload path prepared; waiting for actual subscription ID/access | `scripts/upload-data-to-azure.sh` uses Azure CLI login to upload `data/` under a `teleport/` prefix in an existing container. Azure CLI 2.91.0 is installed and browser sign-in succeeded. The ID provided was the Vanier directory ID, not a subscription ID; management calls return `SubscriptionNotFound`. No resource or upload has been created. |
| Architecture updates | Complete for current and planned layers | `data-and-api-v0.md` distinguishes the verified local DB path from planned AWS hosting. |
| Trello updates, GitHub push, and screenshot evidence | Not complete | `origin` is configured, but current edits are uncommitted and not pushed. No Trello URL is available; Omnivox submission is a manual project-owner action. |

## Week 6 — ETL / ELT Data Pipeline

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Pipeline design | Complete locally | `data-pipeline-v0.md` documents raw CSV → Python validation/normalization → cleaned CSV → PostgreSQL staging/quality checks → destination/activity tables. |
| Source and destination connections | Verified locally | Source is `data/raw/destination-research.csv`; destination was the isolated local `teleport_etl_demo` PostgreSQL database. Credentials are not stored in the repo. |
| Successful pipeline run | Verified locally on 2026-10-25 and rechecked on 2026-11-02 | Seven source records transformed to 2 destinations and 5 activities. The latest run used temporary output files, matched the reviewed cleaned CSV records, and loaded to `teleport_etl_demo` without modifying their formatting. |
| Transform/data-quality evidence | Verified locally | Blank required fields, duplicate keys, orphan activities, and invalid rows counted zero; the repeated load inserted zero additional rows. See `data-pipeline-v0.md`. |
| Trello update, architecture screenshot, and run screenshots | Pending manual evidence | The owner manages the Trello board and personally captures/submits screenshots. The evidence map in `data-pipeline-v0.md` lists the real artifacts to capture; no screenshot is fabricated. |

## Week 7 — SQL Analysis and Data Quality

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| SQL joins, filters, GROUP BY, COUNT, SUM, AVG | Complete as saved queries | `database/analysis/week-7-queries.sql` contains ten read-only queries. |
| Run analytical queries | Verified locally | Queries run on `teleport_dev`: 2 destinations, 5 activities, 0 trips, 0 itinerary items, 0 expenses. |
| Data-quality checks | Verified locally | Blank fields, duplicate destination keys, orphan activities, invalid trip ranges, and non-positive expense amounts all returned zero. |
| Results and business interpretation | Documented | `docs/analysis/week-7-results.md` states that destination coverage can be compared only for this tiny sample; no spending conclusion is supported because there are no expense rows. |
| Trello, SQL screenshots, and GitHub progress | Query/results committed and pushed; manual screenshots pending | `database/analysis/week-7-queries.sql` and `docs/analysis/week-7-results.md` are in GitHub commit `c84b45c`. The saved query file is evidence; the owner still needs to capture any required result screenshots and update/submit the Trello evidence. |

## Week 8 — Operational and Analytical Architecture

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Updated analytical architecture | Drafted | `docs/analytics-architecture-v0.md` separates current operational PostgreSQL from a proposed analytical flow. |
| OLTP vs. OLAP comparison | Documented | The architecture note explains the distinction using Teleport examples. |
| Fact/dimension-style design | Proposed | Minimal `FactExpense`, `DimTrip`, and `DimDate` grain is described; no warehouse tables are implemented. |
| Analytical proof of concept | Local reporting proof of concept implemented and verified | Two reporting views and two functions are defined in `database/schema/003_reporting.sql`; `database/analysis/week-8-reporting-demo.sql` returns the current destination/activity results. No separate warehouse is deployed, and trip/expense analysis remains empty because no such records exist. |
| Trello, architecture screenshot, and commits | Pending evidence/commit | Use the actual diagram/document, update the real board, and commit/push verified changes. |

## Cross-week evidence limitations

- Local PostgreSQL 18.3 is reachable. `teleport_dev` currently contains two demonstration destinations, five activities, and zero trips, itinerary items, or expenses after smoke-test cleanup. The separate `teleport_etl_demo` database contains the ETL destination/activity output.
- `GET /api/health` checks API-host liveness only; database-backed destination requests and the successful CRUD smoke test separately verify API-to-database communication.
- Screenshots are saved locally in `docs/`, but their existence is not an Omnivox submission; the project owner must upload them.
- Azure upload and AWS resource provisioning are not done; both require account access and any applicable cost approval.
- Local credential handling was reviewed in `docs/security-and-secrets-v0.md`: `.env` is ignored, the API development connection string is in .NET User Secrets, and CI generates an ephemeral database password. The API has no authentication and no cloud secret manager is deployed; do not expose this PoC publicly.
- Podman 5.5.2 and its local amd64 runtime are verified with `hello-world`; the Teleport stack itself has not been run under Podman. Docker Desktop availability has been intermittent; while available, the Compose stack runs and passes verification. A fresh Compose build remains blocked by a Docker engine HTTP 500 during image pull.
- GitHub remote `origin` exists, but current uncommitted edits must be reviewed, committed, and pushed before claiming they are on GitHub.
- Screenshot files saved locally still need the project owner's review and manual Omnivox submission.
