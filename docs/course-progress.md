# Course Milestone Progress

This tracker maps the supplied Week 2, Week 3, and Week 5–8 instructions. The Week 5 brief labels its work as Week 6 of the course and Week 5 of the project. No detailed Week 1 or Week 4 assignment checklist was supplied, so those details are not invented here.

## Week 1 foundation prerequisites

| Foundation item | Status | Evidence |
| --- | --- | --- |
| Project idea, business problem, and scope | Documented | `README.md` and `requirements/frontend-requirements.md`. |
| Target users and project type | Documented | `requirements/frontend-requirements.md`; POC. |
| Initial architecture | Documented and aligned to the chosen local Podman / planned AWS PostgreSQL direction | `data-and-api-v0.md`. |
| Functional and non-functional requirements | Documented | `requirements/frontend-requirements.md`. |
| Git repository and development history | Milestone and final report commits are pushed to `origin/main`; current frontend/data/API-helper edits have been reviewed locally but remain uncommitted. | GitHub commit history and the successful CI run linked in `README.md`; publish the current local edits before claiming they are on GitHub. Manual screenshots and cloud resources remain pending. |
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
| Build/run and verify the API in a container | Podman Compose verified locally; Docker Compose verified in CI | The Podman stack built and started PostgreSQL, API, and web. Web, API liveness/readiness, and destination checks returned HTTP 200; `scripts/smoke-test-api.sh` passed. The first image pulls/build took several minutes. |
| Prepare and organize project data | Complete for a small demonstration set | Sourced research, normalized CSVs, and idempotent seed SQL under `data/` and `database/seed/`; Azure upload script maps the prepared tree to an existing container. |
| Cloud database and API connection | Blocked; not provisioned | The user reports the AWS account is suspended. No cloud credentials or resources are stored in the repo; ask the instructor whether the verified local database is acceptable. |
| Azure Blob/ADLS storage | Upload path prepared; waiting for actual subscription ID/access | `scripts/upload-data-to-azure.sh` uses Azure CLI login to upload `data/` under a `teleport/` prefix in an existing container. Azure CLI 2.91.0 is installed and browser sign-in succeeded. The ID provided was the Vanier directory ID, not a subscription ID; management calls return `SubscriptionNotFound`. No resource or upload has been created. |
| Architecture updates | Complete for current and planned layers | `data-and-api-v0.md` distinguishes the verified local DB path from planned AWS hosting. |
| Trello updates, GitHub push, and screenshot evidence | Screenshots captured; current source updates need publishing | Screenshots are organized under `docs/evidence/`. The current frontend/data/API-helper changes are locally verified but uncommitted; Trello and Omnivox actions remain owner-managed. |

## Week 6 — ETL / ELT Data Pipeline

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Pipeline design | Complete locally | `data-pipeline-v0.md` documents raw CSV → Python validation/normalization → cleaned CSV → PostgreSQL staging/quality checks → destination/activity tables. |
| Source and destination connections | Verified locally | Source is `data/raw/destination-research.csv`; destination was the isolated local `teleport_etl_demo` PostgreSQL database. Credentials are not stored in the repo. |
| Successful pipeline run | Verified locally and automated in GitHub Actions | Seven source records transformed to 2 destinations and 5 activities. Local reruns used temporary output files to preserve reviewed CSV formatting. Latest successful CI run `41909631564` executes the ETL against an isolated database and uploads a 7-day CSV artifact. |
| Transform/data-quality evidence | Verified locally | Blank required fields, duplicate keys, orphan activities, and invalid rows counted zero; the repeated load inserted zero additional rows. See `data-pipeline-v0.md`. |
| Trello update, architecture screenshot, and run screenshots | Screenshots captured and organized; Trello/Omnivox action pending | Week 6 screenshots are under `docs/evidence/week-6-etl/`. The owner still needs to update the board and upload the required evidence to Omnivox. |

## Week 7 — SQL Analysis and Data Quality

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| SQL joins, filters, GROUP BY, COUNT, SUM, AVG | Complete as saved queries | `database/analysis/week-7-queries.sql` contains ten read-only queries. |
| Run analytical queries | Verified locally | Queries run on `teleport_dev`: 2 destinations, 5 activities, 0 trips, 0 itinerary items, 0 expenses. |
| Data-quality checks | Verified locally | Blank fields, duplicate destination keys, orphan activities, invalid trip ranges, and non-positive expense amounts all returned zero. |
| Results and business interpretation | Documented | `docs/analysis/week-7-results.md` states that destination coverage can be compared only for this tiny sample; no spending conclusion is supported because there are no expense rows. |
| Trello, SQL screenshots, and GitHub progress | Query/results pushed; screenshots captured; Trello/Omnivox action pending | `database/analysis/week-7-queries.sql` and `docs/analysis/week-7-results.md` are in GitHub commit `c84b45c`. Screenshots are under `docs/evidence/week-7-sql/`; update the board and upload required evidence to Omnivox. |

## Week 8 — Operational and Analytical Architecture

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Updated analytical architecture | Documented | `docs/analytics-architecture-v0.md` shows the implemented OLTP and GitHub Actions ETL/artifact path, with the separate dimensional warehouse clearly marked proposed. |
| OLTP vs. OLAP comparison | Documented | The architecture note explains the distinction using Teleport examples. |
| Fact/dimension-style design | Proposed | Minimal `FactExpense`, `DimTrip`, and `DimDate` grain is described; no warehouse tables are implemented. |
| Analytical proof of concept | Local reporting proof of concept implemented and verified | Two reporting views and two functions are defined in `database/schema/003_reporting.sql`; `database/analysis/week-8-reporting-demo.sql` returns the current destination/activity results. No separate warehouse is deployed, and trip/expense analysis remains empty because no such records exist. |
| Trello, architecture screenshot, and commits | Architecture screenshots captured; board/Omnivox action pending | Week 8 architecture/reporting artifacts are in commit `7efac12`; screenshots are under `docs/evidence/week-8-analytics/`. Trello updates and Omnivox upload remain owner-managed. |

## Final Project Requirement Review

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| CI/CD | Implemented and passed | `.github/workflows/ci.yml`; successful run `41909631564` covers build, Compose, readiness, ETL, artifact, smoke test, and cleanup. |
| Security and secrets | Partially implemented | `docs/security-and-secrets-v0.md`; User Secrets, ignored `.env`, ephemeral CI password, and CLI/RBAC upload helper. API authentication, cloud secret manager, and deployed cloud permissions remain absent. |
| Monitoring | Local/CI monitoring implemented; cloud monitoring absent | `docs/monitoring-v0.md`; API and DB health checks, Compose logs, and GitHub Actions run history. No Azure Monitor/App Insights/Log Analytics or CloudWatch. |
| End-to-end validation | Verified locally and in CI | Angular web, .NET API, PostgreSQL, CRUD smoke test, and automated ETL run. No cloud-hosted app/data path. |
| Final architecture | Documented for implemented components | `docs/analytics-architecture-v0.md`; local operational flow and actual CI/ETL artifact path are shown. AWS/Azure and the warehouse remain unimplemented. |
| GitHub repository | Milestones and final report pushed; current local source updates pending | Week 6–8, CI, and monitoring are on `origin/main`. Today's frontend, data, API-helper, evidence, and documentation updates still need a commit and CI run. A workspace secret scan is not a full historical audit. |
| Trello and evidence | Screenshots organized; owner action pending | Screenshots are indexed in `docs/evidence/README.md`. The owner maintains the board and must upload the evidence to Omnivox. |
| Final README/report | Updated; team roster needs confirmation | `README.md` covers objective, technologies, setup, results, and limitations; add any other team members before submitting. |
| Final presentation/demo | Demo guide prepared; live demonstration remains owner-run | `docs/final-demo-v0.md` provides the sequence and verified claims. Confirm additional contributors, rehearse the local app/ETL/CI flow, and present cloud services as unprovisioned. |
| AI Prompt Log | Documented | `docs/prompt-log.md` records assistance and validation for ETL, analysis, architecture, security, and CI work. |
| Data mining/business analysis | Completed for the small sample | Week 7 query/results files include joins, filters, aggregates, quality checks, interpretation, and sample-size/spending limitations. |
| Automated workflow | Implemented and passed | Scheduled/push/PR/manual GitHub Actions workflow runs raw CSV → transform → isolated PostgreSQL → 7-day artifact; run `41909631564`. |
| Database/cloud infrastructure | Local stack verified; cloud resources blocked | Local Podman Compose includes PostgreSQL, API, and web. AWS is unavailable due to account suspension; Azure management returns `SubscriptionNotFound` because the supplied ID is a directory ID, not a subscription ID. |

## Shared Verification Notes

- After smoke-test cleanup, the local app database contains 2 destinations, 5 activities, and no trips, itinerary items, or expenses. The separate `teleport_etl_demo` holds the same destination/activity output.
- `GET /api/health` and `GET /api/health/database` returned HTTP 200 in the local Podman deployment. The API smoke test passed and removed its temporary records.
- GitHub Actions run `41909631564` passed for pushed commit `4938d20`; it predates today's local source changes. The new changes need to be pushed and pass CI before being described as GitHub-verified.
- The 30 supplied screenshots are organized in `docs/evidence/`; Omnivox upload and Trello updates are still manual owner tasks.
- AWS is blocked by account suspension. Azure CLI sign-in succeeded, but management calls return `SubscriptionNotFound`; no cloud resource or upload has been created. Cloud monitoring, API authentication, and a cloud secret manager are not implemented. See `docs/security-and-secrets-v0.md` and `docs/monitoring-v0.md`.
