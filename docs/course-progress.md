# Course Milestone Progress

This tracker maps the supplied Week 2, Week 3, and Week 5–8 instructions. The Week 5 brief labels its work as Week 6 of the course and Week 5 of the project. No detailed Week 1 or Week 4 assignment checklist was supplied, so those details are not invented here.

## Week 1 foundation prerequisites

| Foundation item | Status | Evidence |
| --- | --- | --- |
| Project idea, business problem, and scope | Documented | `README.md` and `requirements/frontend-requirements.md`. |
| Target users and project type | Documented | `requirements/frontend-requirements.md`; POC. |
| Initial architecture | Documented and aligned to the chosen local Podman / planned AWS PostgreSQL direction | `data-and-api-v0.md`. |
| Functional and non-functional requirements | Documented | `requirements/frontend-requirements.md`. |
| Git repository and development history | Project source, evidence, and documentation are pushed to `origin/main`. | GitHub commit history and CI runs linked in `README.md`. The project owner reports the required screenshots were uploaded to Omnivox; cloud resources remain blocked. |
| Trello board | Owner-managed; weekly Trello screenshots submitted | The owner reported a board named `Travel Website – PoC` with Backlog, To Do, Doing, Review, Testing, and Done lists. Weekly Trello captures are included in the submitted evidence; the live board was not independently checked. |

## Week 2 — requirements, frontend data collection, AI prompting

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Refine functional requirements and define user-entered fields | Complete | `requirements/frontend-requirements.md` contains project context, required/optional fields, validation, and user-visible results. |
| Create a frontend prototype and working data-collection flow | Implemented | Angular destination search, trip form, itinerary, and expense screens. Real API verification is recorded separately. |
| Document meaningful AI assistance and validation | Complete for this milestone | `prompt-log.md` records the prompt, implementation, and verification performed. |
| Keep Trello tasks and statuses current | Owner-managed | Weekly Trello screenshots were included in the submitted evidence; the current live board status was not independently verified. |
| Capture frontend/requirements/Prompt Log screenshots for Omnivox | Captured and submitted | `frontend-destination-discovery.png`, `frontend-trip-planning.png`, `requirements-document.png`, and `prompt-log-document.png` are saved under `docs/`; the project owner reports uploading the required screenshots to Omnivox. |

## Week 3 — API, data flow, and integration

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Derive API from frontend actions and define request/response contracts | Complete | `data-and-api-v0.md`, endpoint source, and `apps/api/Teleport.Api.http`. |
| Document a real data-flow diagram | Complete | Mermaid sequence diagrams in `data-and-api-v0.md`. |
| Implement API, OpenAPI, validation, and error responses | Implemented and locally verified | .NET Minimal API routes; `scripts/smoke-test-api.sh` passed validation and CRUD responses. |
| Connect frontend to API | Implemented and browser verified | Angular services use the configured API base URL; the browser created and removed a test trip and exercised itinerary and expense operations. |
| Capture API/Swagger and integration screenshots | Captured and submitted | `api-destinations-response.png` and `openapi-document.png` capture the live API response and OpenAPI document; `frontend-trip-planning.png` captures the integrated workflow. The project owner reports uploading the required screenshots to Omnivox. |

## Week 5 project track — original stack plus regular-project storage requirements

The original project brief requires **Podman + AWS PostgreSQL**. The supplied regular-project course brief additionally requires **Docker** containerization and preparation for **Azure Blob Storage or ADLS**. Treat these as additive requirements; the separate AWS-to-Azure migration track does not apply. Cloud resource creation and uploads remain pending account access and cost approval.

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Prepare app/API for containers | Files prepared | API and web Dockerfiles, Compose configuration, and `.dockerignore`. |
| Build/run and verify the API in a container | Podman Compose verified locally; Docker Compose verified in CI | The Podman stack built and started PostgreSQL, API, and web. Web, API liveness/readiness, and destination checks returned HTTP 200; `scripts/smoke-test-api.sh` passed. The first image pulls/build took several minutes. |
| Prepare and organize project data | Complete for a small demonstration set | Sourced research, normalized CSVs, and idempotent seed SQL under `data/` and `database/seed/`; Azure upload script maps the prepared tree to an existing container. |
| Cloud database and API connection | Not met; no cloud DB deployed | AWS reports the free plan ended and the account is closed, with content deletion scheduled after December 30, 2026. The owner will not create a replacement account or upgrade to paid AWS for this POC unless the instructor confirms the need and cost is approved. Local PostgreSQL is verified but does not fulfill AWS RDS; an alternative needs instructor approval. |
| Azure Blob/ADLS storage | Upload path prepared; waiting for actual subscription ID/access | `scripts/upload-data-to-azure.sh` uses Azure CLI login to upload `data/` under a `teleport/` prefix in an existing container. Azure CLI 2.91.0 is installed and browser sign-in succeeded. The ID provided was the Vanier directory ID, not a subscription ID; management calls return `SubscriptionNotFound`. No resource or upload has been created. |
| Architecture updates | Complete for current and planned layers | `data-and-api-v0.md` distinguishes the verified local DB path from planned AWS hosting. |
| Trello updates, GitHub push, and screenshot evidence | Source/evidence pushed; weekly screenshot sets submitted | Screenshots, including Trello captures, are organized under `docs/evidence/`; the owner reports uploading them to Omnivox. The live board remains owner-managed. |

## Week 6 — ETL / ELT Data Pipeline

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Pipeline design | Complete locally | `data-pipeline-v0.md` documents raw CSV → Python validation/normalization → cleaned CSV → PostgreSQL staging/quality checks → destination/activity tables. |
| Source and destination connections | Verified locally | Source is `data/raw/destination-research.csv`; destination was the isolated local `teleport_etl_demo` PostgreSQL database. Credentials are not stored in the repo. |
| Successful pipeline run | Verified locally and automated in GitHub Actions | Seven source records transformed to 2 destinations and 5 activities. Local reruns used temporary output files to preserve reviewed CSV formatting. CI run `41935325901` for commit `8ec063b` ran ETL against an isolated database and uploaded a 7-day CSV artifact. |
| Transform/data-quality evidence | Verified locally | Blank required fields, duplicate keys, orphan activities, and invalid rows counted zero; the repeated load inserted zero additional rows. See `data-pipeline-v0.md`. |
| Trello update, architecture screenshot, and run screenshots | Trello capture included in submitted screenshots | Week 6 evidence, including the Trello capture, is under `docs/evidence/week-6-etl/`; the owner reports uploading the weekly evidence set to Omnivox. The live board remains owner-managed. |

## Week 7 — SQL Analysis and Data Quality

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| SQL joins, filters, GROUP BY, COUNT, SUM, AVG | Complete as saved queries | `database/analysis/week-7-queries.sql` contains ten read-only queries. |
| Run analytical queries | Verified locally | Queries run on `teleport_dev`: 2 destinations, 5 activities, 0 trips, 0 itinerary items, 0 expenses. |
| Data-quality checks | Verified locally | Blank fields, duplicate destination keys, orphan activities, invalid trip ranges, and non-positive expense amounts all returned zero. |
| Results and business interpretation | Documented | `docs/analysis/week-7-results.md` states that destination coverage can be compared only for this tiny sample; no spending conclusion is supported because there are no expense rows. |
| Trello, SQL screenshots, and GitHub progress | Query/results pushed; Trello capture included in submitted screenshots | `database/analysis/week-7-queries.sql` and `docs/analysis/week-7-results.md` are in GitHub commit `c84b45c`. The screenshot set under `docs/evidence/week-7-sql/` includes the Trello capture; the owner reports uploading it to Omnivox. |

## Week 8 — Operational and Analytical Architecture

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Updated analytical architecture | Documented | `docs/analytics-architecture-v0.md` shows the implemented OLTP and GitHub Actions ETL/artifact path, with the separate dimensional warehouse clearly marked proposed. |
| OLTP vs. OLAP comparison | Documented | The architecture note explains the distinction using Teleport examples. |
| Fact/dimension-style design | Proposed | Minimal `FactExpense`, `DimTrip`, and `DimDate` grain is described; no warehouse tables are implemented. |
| Analytical proof of concept | Local reporting proof of concept implemented and verified | Two reporting views and two functions are defined in `database/schema/003_reporting.sql`; `database/analysis/week-8-reporting-demo.sql` returns the current destination/activity results. No separate warehouse is deployed, and trip/expense analysis remains empty because no such records exist. |
| Trello, architecture screenshot, and commits | Trello capture included in submitted screenshots | Week 8 architecture/reporting artifacts are in commit `7efac12`; the screenshot set under `docs/evidence/week-8-analytics/` includes the Trello capture. The owner reports uploading it to Omnivox. |

## Final Project Requirement Review

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| CI/CD | Implemented and passed | `.github/workflows/ci.yml`; run `41935325901` passed build, Compose, readiness, ETL, artifact, smoke test, and cleanup for commit `8ec063b`. |
| Security and secrets | Partially implemented | `docs/security-and-secrets-v0.md`; User Secrets, ignored `.env`, ephemeral CI password, and CLI/RBAC upload helper. API authentication, cloud secret manager, and deployed cloud permissions remain absent. |
| Monitoring | Local/CI monitoring implemented; cloud monitoring absent | `docs/monitoring-v0.md`; API and DB health checks, Compose logs, and GitHub Actions run history. No Azure Monitor/App Insights/Log Analytics or CloudWatch. |
| End-to-end validation | Verified locally and in CI | Angular web, .NET API, PostgreSQL, CRUD smoke test, and automated ETL run. No cloud-hosted app/data path. |
| Final architecture | Documented for implemented components | `docs/analytics-architecture-v0.md`; local operational flow and actual CI/ETL artifact path are shown. AWS/Azure and the warehouse remain unimplemented. |
| GitHub repository | Implementation and evidence pushed; CI passed | Commit `8ec063b` includes the reviewed frontend, data, Azure upload helper, and 30 screenshots; run `41935325901` passed. A workspace secret scan is not a full historical audit. |
| Trello and evidence | Weekly screenshot sets submitted; live board owner-managed | Screenshots, including weekly Trello captures, are indexed in `docs/evidence/README.md`; the project owner reports completing the Omnivox upload. The live board was not independently checked. |
| Final README/report | Updated; team roster needs confirmation | `README.md` covers objective, technologies, setup, results, and limitations; add any other team members before submitting. |
| Final presentation/demo | Demo guide prepared; live demonstration remains owner-run | `docs/final-demo-v0.md` provides the sequence and verified claims. Confirm additional contributors, rehearse the local app/ETL/CI flow, and present cloud services as unprovisioned. |
| AI Prompt Log | Documented | `docs/prompt-log.md` records assistance and validation for ETL, analysis, architecture, security, and CI work. |
| Data mining/business analysis | Completed for the small sample | Week 7 query/results files include joins, filters, aggregates, quality checks, interpretation, and sample-size/spending limitations. |
| Automated workflow | Implemented and passed | Scheduled/push/PR/manual GitHub Actions workflow runs raw CSV → transform → isolated PostgreSQL → 7-day artifact; run `41935325901`. |
| Database/cloud infrastructure | Local stack verified; cloud requirements not met | Local Podman Compose includes PostgreSQL, API, and web. AWS reports the free plan ended and the account is closed; Azure management returns `SubscriptionNotFound` because the supplied ID was a directory ID, not a subscription ID. No project cloud resources were created. Ask the instructor whether the local database satisfies an approved alternative. |

## Shared Verification Notes

- After smoke-test cleanup, the local app database contains 2 destinations, 5 activities, and no trips, itinerary items, or expenses. The separate `teleport_etl_demo` holds the same destination/activity output.
- `GET /api/health` and `GET /api/health/database` returned HTTP 200 in the local Podman deployment. The API smoke test passed and removed its temporary records.
- GitHub Actions run `41935325901` passed for implementation/evidence commit `8ec063b`. Documentation updates after that commit do not change the app or workflow; check the Actions page for their push result.
- The 30 supplied screenshots are organized in `docs/evidence/`; the project owner reports uploading the weekly sets, including Trello captures, to Omnivox. The live board remains owner-managed and was not independently reviewed.
- AWS reports the free plan ended and the account is closed, with content deletion scheduled after December 30, 2026. The owner is avoiding a replacement account or paid upgrade without instructor approval. Azure CLI sign-in succeeded, but management calls return `SubscriptionNotFound`; no project cloud resource or upload has been created. Cloud monitoring, API authentication, and a cloud secret manager are not implemented. See `docs/security-and-secrets-v0.md` and `docs/monitoring-v0.md`.
