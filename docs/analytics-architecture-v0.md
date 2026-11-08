# Teleport Analytics Architecture v0

**Status:** Week 8 design proposal. No separate analytics warehouse has been deployed.

## Current Operational Architecture

```mermaid
flowchart LR
    Web[Angular Nx + Bootstrap] -->|HTTP / JSON| API[.NET Minimal API]
    API -->|EF Core / Npgsql| OLTP[(PostgreSQL: teleport_dev)]
```

The existing PostgreSQL database is the operational system of record. The app and API use normalized tables for destinations, activities, trips, itinerary items, and expenses.

## Supabase Database Prototype

A Supabase Free PostgreSQL project has been provisioned and initialized with the same five-table schema, the 2-destination/5-activity seed, and the reporting views/functions. The Supabase Data API is disabled. The running Angular/.NET app still uses local `teleport_dev`; API-to-Supabase connection and CRUD have not yet been tested. Supabase is a candidate substitute for the AWS database requirement only if the instructor approves it.

## Implemented Automated CI and ETL Flow

GitHub Actions runs on pushes, pull requests, a weekly schedule, or manual dispatch. The workflow builds both application projects, starts an ephemeral Docker Compose stack, checks service readiness, runs the ETL against an isolated database, publishes the cleaned CSV output as a 7-day artifact, runs the CRUD smoke test, and removes the temporary stack.

```mermaid
flowchart LR
    Trigger[Push / PR / weekly schedule / manual] --> CI[GitHub Actions]
    CI --> Build[Build Angular and .NET API]
    Build --> Stack[Ephemeral Docker Compose stack]
    Stack --> Ready[API, database, and web readiness]
    Ready --> Source[Reviewed raw destination CSV]
    Source --> Transform[Python validation and normalization]
    Transform --> Clean[Cleaned destination/activity CSVs]
    Transform --> ETLDB[(Isolated PostgreSQL teleport_etl_demo)]
    ETLDB --> Quality[Staging quality checks and idempotent load]
    Clean --> Artifact[GitHub Actions artifact<br/>7-day retention]
    Quality --> Smoke[API CRUD smoke test]
    Smoke --> Cleanup[Compose cleanup]
```

This automation is hosted by GitHub Actions; the PostgreSQL target and Compose stack exist only for the run. It is not an Azure Data Factory or AWS service deployment.

## Proposed Future Warehouse Data Flow

```mermaid
flowchart LR
    OLTP[(Operational PostgreSQL)] -->|Read-only batch extract| ETL[Python ETL + data-quality checks]
    ETL -->|Proposed dimensional output| MART[(Proposed analytics schema)]
    MART --> REPORT[SQL reports / course analysis]
```

The separate operational-PostgreSQL-to-analytics-schema batch is a proposed next step, not a completed or scheduled warehouse service. The current implemented ETL reads reviewed CSV research, not application OLTP tables.

## OLTP vs. OLAP

| Aspect | OLTP: operational data | OLAP: analytical data |
| --- | --- | --- |
| Purpose | Support application actions such as reading destinations and creating trips. | Summarize data for questions such as activity coverage or recorded spending. |
| Shape | Normalized relational tables with primary and foreign keys. | Read-oriented fact and dimension structures or aggregate views. |
| Workload | Small, frequent application reads and writes. | Grouping, filtering, joins, and aggregate queries across a set of records. |
| Teleport example | `destinations`, `activities`, `trips`, `itinerary_items`, `expenses`. | Proposed expense fact joined to trip and date dimensions. |

## Proposed Fact/Dimension Grain

A minimal expense analytical model could use:

- **FactExpense:** one row per recorded expense; measures are `amount` and an implicit row count.
- **DimTrip:** one row per trip with trip name and date range.
- **DimDate:** one row per calendar date with year, month, and day attributes.

The fact would link to trip and expense date. The current expense table has no destination foreign key, so spending must not be attributed to a destination unless a future requirement adds that relationship. Currency is also unspecified, so amounts cannot be safely combined across currencies if mixed currencies are later allowed.

## Small Analytical Demonstration

`database/analysis/week-7-queries.sql` and `database/analysis/week-8-reporting-demo.sql` provide read-only analysis and reporting examples. `database/schema/003_reporting.sql` defines two views and two functions over the operational schema. The November 4, 2026 verification returned 2 destination rows, with 3 activities for Lisbon and 2 for Porto; the functions returned the same activity counts.

The current database still has 0 trips, 0 itinerary items, and 0 expenses. The trip-expense view and trip-based query therefore return no rows, and the expense-total function returns `NULL`. These are empty-state results, not measured zero spending. No spending conclusion is supported yet.

## Implementation Boundary

The proposed dimensional output is not implemented. First collect reviewed trip and expense records through the actual application; then decide whether a separate schema/table or a reporting view is appropriate. No Azure/AWS analytics or storage resources are created by this design document.
