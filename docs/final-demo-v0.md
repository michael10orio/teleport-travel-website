# Teleport Final Demo Guide v0

## Project Objective

Teleport is a proof of concept for destination discovery and basic trip planning. It supports browsing destinations and activities, creating trips, arranging itinerary items, and tracking expenses. Booking, payments, live travel prices, and personalized recommendations are outside scope.

## Demonstration Flow

1. **Objective and scope:** Explain the target traveler, problem, core scenarios, and explicit POC exclusions.
2. **Architecture:** Show the implemented Angular/Nx frontend → .NET Minimal API → PostgreSQL flow. Identify cloud hosting and the separate analytics warehouse as planned, not deployed.
3. **Application workflow:** Browse/filter destinations, view activities, create a trip, add an itinerary item, and add an expense. Use the existing smoke-test flow for a repeatable cleanup-safe API demonstration if needed.
4. **ETL and data:** Show raw CSV → Python validation/normalization → PostgreSQL staging/quality checks → destination/activity tables. Seven raw rows produce two destinations and five activities; quality checks are zero and repeat loading adds no duplicates.
5. **Analysis:** Show the Week 7 SQL and Week 8 reporting demo. Lisbon has three activities and Porto two in this small sample. There are no trip or expense rows, so expense totals/averages are `NULL`; do not present them as measured zero spending.
6. **Automation and CI/CD:** Show GitHub Actions run [41935325901](https://github.com/michael10orio/teleport-travel-website/actions/runs/41935325901), which passed app/API builds, Docker Compose startup, API/database/web readiness, ETL, a 7-day cleaned-data artifact, CRUD smoke tests, and cleanup for commit `8ec063b`.
7. **Security and monitoring:** Explain .NET User Secrets, ignored `.env`, the ephemeral CI database password, local liveness/database-readiness endpoints, and container/GitHub logs. State that API authentication, a cloud secret manager, and cloud monitoring are not deployed.
8. **Limitations and recommendation:** This is a local POC, not a production or cloud-hosted service. The AWS account is suspended; Azure management calls return `SubscriptionNotFound`. No cloud database, storage, or monitoring is deployed. The small dataset supports activity-coverage counts, not traveler preferences or spending conclusions.
9. **Contributions:** The repository owner is Michael James Tenorio. Add other contributors and individual contributions only if applicable.

## Practical Lessons

- The first ETL load into an empty database failed because the schema had not been applied. Adding schema setup to the runner fixed that and made repeat loads idempotent.
- Azure sign-in succeeded, but the ID initially used was a directory ID rather than a subscription ID. Checking account/subscription access earlier would have avoided planning around unavailable cloud resources. AWS access is also blocked by account suspension.
- The first Podman Compose run spent several minutes pulling and building images. The stack worked after that; leave time for first-run downloads.
- Two destinations and five activities are enough to test joins and counts, not to make a useful travel recommendation.

**Reflection draft to personalize:** I would verify cloud access before committing to a provider, and I would gather more reviewed data before making business recommendations. The local stack and small ETL sample were the most useful parts for proving the application and pipeline mechanics.

The 30 screenshots are organized in `docs/evidence/`. Review them and upload the assignment-required evidence to Omnivox; files in the repository are not a submission.