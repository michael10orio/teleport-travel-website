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
