# Teleport Final Demo Guide v0

## Project Objective

Teleport is a proof of concept for destination discovery and basic trip planning. It supports browsing destinations and activities, creating trips, arranging itinerary items, and tracking expenses. Booking, payments, live travel prices, and personalized recommendations are outside scope.

## Demonstration Flow

1. **Objective and scope:** Explain the target traveler, problem, core scenarios, and explicit POC exclusions.
2. **Architecture:** Show the implemented Angular/Nx frontend → .NET Minimal API → PostgreSQL flow. Identify cloud hosting and the separate analytics warehouse as planned, not deployed.
3. **Application workflow:** Browse/filter destinations, view activities, create a trip, add an itinerary item, and add an expense. Use the existing smoke-test flow for a repeatable cleanup-safe API demonstration if needed.
4. **ETL and data:** Show raw CSV → Python validation/normalization → PostgreSQL staging/quality checks → destination/activity tables. Seven raw rows produce two destinations and five activities; quality checks are zero and repeat loading adds no duplicates.
5. **Analysis:** Show the Week 7 SQL and Week 8 reporting demo. Lisbon has three activities and Porto two in this small sample. There are no trip or expense rows, so expense totals/averages are `NULL`; do not present them as measured zero spending.
6. **Automation and CI/CD:** Show GitHub Actions run [41908117744](https://github.com/michael10orio/teleport-travel-website/actions/runs/41908117744), which passed app/API builds, Docker Compose startup, API/database/web readiness, scheduled ETL, a 7-day cleaned-data artifact, CRUD smoke tests, and cleanup.
7. **Security and monitoring:** Explain .NET User Secrets, ignored `.env`, the ephemeral CI database password, local liveness/database-readiness endpoints, and container/GitHub logs. State that API authentication, a cloud secret manager, and cloud monitoring are not deployed.
8. **Limitations and recommendation:** This is a local POC, not a production or cloud-hosted service. The small curated dataset supports only activity-coverage counts. Resolve the Azure subscription issue before provisioning approved storage; add production authentication, managed secrets, cloud monitoring, and broader reviewed data before drawing broader business conclusions.
9. **Contributions:** The repository owner is Michael James Tenorio. Add other contributors and individual contributions only if applicable.

Attach the actual screenshots, query/report files, and commit history required by the course. Local evidence files are not an Omnivox submission.