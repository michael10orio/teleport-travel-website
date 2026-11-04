# Teleport Monitoring v0

**Status:** Local liveness, database-readiness, and container-log evidence are implemented. No cloud monitoring service is deployed.

## Health Checks

- `GET /api/health` is an API-host liveness check. It returns HTTP 200 while the API process is serving requests; it does not prove database connectivity.
- `GET /api/health/database` calls EF Core's `CanConnectAsync` for PostgreSQL. It returns HTTP 200 with `{"status":"ok"}` when connected and HTTP 503 with `{"status":"unavailable"}` when PostgreSQL cannot be reached. It does not expose connection details or exception messages.
- Local Release verification on 2026-11-04 returned HTTP 200 from both `/api/health` and `/api/health/database` against PostgreSQL.
- `scripts/smoke-test-api.sh` checks both endpoints before exercising database-backed routes.
- GitHub Actions run `41908117744` passed frontend/API builds, Compose build/start, API/database/web readiness, the scheduled ETL, transformed-data artifact upload, CRUD smoke tests, and cleanup.

## Logs and Run History

- Local container logs are available through `docker compose logs api`, `docker compose logs postgres`, or `docker compose logs --follow` while the stack is running.
- GitHub Actions stores build, readiness, ETL, artifact, smoke-test, and failure-log output in each workflow run. The ETL CSV artifact is retained for 7 days; the workflow cleans up its temporary Compose volume after the job.
- .NET's built-in logging writes startup and exception logs to standard output/error for container collection.

## Limitations

- Azure Monitor, Application Insights, Log Analytics, AWS CloudWatch, persistent metrics, and alerting are not configured. Azure subscription access is currently blocked with `SubscriptionNotFound`; AWS resources are not provisioned. The CI run log is the only hosted build/run history currently available.
- The health endpoints are unauthenticated and intended for this local PoC. Do not expose them or the API publicly without adding appropriate access controls and production monitoring.