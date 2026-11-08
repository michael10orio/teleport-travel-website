# Teleport Monitoring v0

**Status:** Local liveness, database-readiness, and container-log evidence are implemented. No cloud monitoring service is deployed.

## Health Checks

- `GET /api/health` is an API-host liveness check. It returns HTTP 200 while the API process is serving requests; it does not prove database connectivity.
- `GET /api/health/database` calls EF Core's `CanConnectAsync` for PostgreSQL. It returns HTTP 200 with `{"status":"ok"}` when connected and HTTP 503 with `{"status":"unavailable"}` when PostgreSQL cannot be reached. It does not expose connection details or exception messages.
- The local Podman Compose deployment returned HTTP 200 from both endpoints on 2026-11-04; the API CRUD smoke test also passed.
- `scripts/smoke-test-api.sh` checks both endpoints before exercising database-backed routes.
- GitHub Actions run `41935325901` passed frontend/API builds, Compose build/start, API/database/web readiness, ETL, artifact upload, CRUD smoke tests, and cleanup for commit `8ec063b`.

## Logs and Run History

- Local container logs are available through `podman compose logs api`, `podman compose logs postgres`, or `podman compose logs --follow`. GitHub Actions uses Docker Compose.
- GitHub Actions stores build, readiness, ETL, artifact, smoke-test, and failure-log output in each workflow run. The ETL CSV artifact is retained for 7 days; the workflow cleans up its temporary Compose volume after the job.
- .NET's built-in logging writes startup and exception logs to standard output/error for container collection.

## Limitations

- Azure Monitor, Application Insights, Log Analytics, AWS CloudWatch, persistent metrics, and alerting are not configured. AWS reports the free plan ended and the account is closed, with content deletion scheduled after December 30, 2026; Azure management returns `SubscriptionNotFound`. No project cloud resources are deployed. Upgrading AWS may incur charges.
- The health endpoints are unauthenticated and intended for this local PoC. Do not expose them or the API publicly without adding appropriate access controls and production monitoring.