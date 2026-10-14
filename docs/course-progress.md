# Course Milestone Progress

This tracker maps only the instructions supplied for Week 2, Week 3, and Week 5. The Week 5 attachment labels this as Week 6 of the course and Week 5 of the project. It does not include detailed Week 1 or Week 4 assignment checklists, so those are not invented here.

## Week 1 foundation prerequisites

| Foundation item | Status | Evidence |
| --- | --- | --- |
| Project idea, business problem, and scope | Documented | `README.md` and `requirements/frontend-requirements.md`. |
| Target users and project type | Documented | `requirements/frontend-requirements.md`; POC. |
| Initial architecture | Documented and aligned to the chosen local Podman / planned AWS PostgreSQL direction | `data-and-api-v0.md`. |
| Functional and non-functional requirements | Documented | `requirements/frontend-requirements.md`. |
| Git repository and development history | Local repository and milestone commits exist; there is no Git remote configured in this workspace. | `git log`; a GitHub push cannot be verified or performed without the repository remote. |
| Trello board | Not verified or updated | No board URL or Trello integration is available in this workspace. |

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

## Week 5 project track — selected POC direction

The project owner selected the existing project brief's **Podman + AWS PostgreSQL** direction. This is a POC, not an AWS-to-Azure migration. The Azure Blob/ADLS and AWS-to-Azure database migration checklists are therefore not implemented as if they were requirements for this project.

| Requirement | Status | Evidence / remaining action |
| --- | --- | --- |
| Prepare app/API for containers | Files prepared | API and web Dockerfiles, Compose configuration, and `.dockerignore`. |
| Build/run and verify the API in a container | Verified with Docker Desktop | Compose built and started web, API, and PostgreSQL; database health passed; Nginx served the app and proxied the API; the API smoke test passed against the Compose database. Podman-specific execution remains unverified because Podman is not installed. |
| Prepare and organize project data | Complete for a small demonstration set | Sourced research, normalized CSVs, and idempotent seed SQL under `data/` and `database/seed/`. |
| Cloud database and API connection | Not provisioned | AWS RDS setup needs explicit region, size, network, and cost approval. No credentials or cloud resources are stored in the repo. |
| Azure Blob/ADLS or migration artifacts | Not applicable to selected project path | Add only if the instructor or project brief changes the required platform. |
| Architecture updates | Complete for current and planned layers | `data-and-api-v0.md` distinguishes the verified local DB path from planned AWS hosting. |
| Trello updates, GitHub push, and screenshot evidence | Not complete | No Trello URL or Git remote is configured, and Omnivox submission is a manual project-owner action. |

## Cross-week evidence limitations

- Local PostgreSQL 18.1 is reachable. The current verified database contains two demonstration destinations, five activities, and one pre-existing trip. Smoke tests use a separate uniquely named trip and delete only that test trip.
- `GET /api/health` checks API-host liveness only; database-backed endpoint requests are needed to verify API-to-database communication.
- Screenshots are saved locally in `docs/`, but their existence is not an Omnivox submission; the project owner must upload them.
- Later pipeline, analytics, automation, CI/CD, and monitoring topics appear as future direction in the supplied instructions but have no detailed deliverables in the attachment. They remain outside this POC milestone until requirements are provided.
