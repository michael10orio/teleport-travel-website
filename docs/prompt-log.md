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
