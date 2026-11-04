# Week 7 SQL Analysis Results

**Source:** Local PostgreSQL `teleport_dev`, queried on 2026-11-04 using `database/analysis/week-7-queries.sql`.

## Observed Results

| Measure | Result |
| --- | ---: |
| Destinations | 2 |
| Activities | 5 |
| Trips | 0 |
| Itinerary items | 0 |
| Expenses | 0 |

The join query returned five reviewed activities: three associated with Lisbon and two with Porto. Both destinations are labeled `City`, so the category grouping returned one group with a count of two. The filtered `Lisboa` query returned Lisbon.

The data-quality checks reported zero blank destination names, blank regions, duplicate destination keys, activities without a destination, invalid trip date ranges, and non-positive expense amounts. The category-null/blank check and orphan-activity query returned no rows.

## Expense Analysis Limitation

The overall expense query returned an expense count of zero and `NULL` for both `SUM(amount)` and `AVG(amount)`. This means no spending has been recorded; it does **not** mean measured spending was zero. The trip-level expense query returned no groups because there are currently no trips.

## Business Interpretation

The current reviewed demonstration data confirms that the destination/activity relationship can be joined and counted: Lisbon has three listed activities and Porto has two. This can help a traveler compare the amount of curated activity information available per destination, but it does not measure popularity, quality, cost, or user preference. With only two destinations, both in the same category, the result is a technical demonstration rather than a reliable recommendation.

No conclusions about trip planning or spending can be drawn until genuine trip, itinerary, and expense records are collected through the application.

## Data Issues and Fixes

- The transformation pipeline normalized Unicode to NFC, trimmed and collapsed whitespace, rejected blank required fields, detected duplicate keys, and checked activity-to-destination references.
- The first pipeline attempt against a new database stopped because the schema had not been applied. The runner was updated to apply the existing idempotent schema before loading.
- The final pipeline run inserted two destinations and five activities. A repeat run inserted zero additional rows, confirming the conflict-safe load behavior.
- No source duplicates, blank required values, or orphan activities were found in this run.

No records were added to `teleport_dev` for this analysis. The ETL demonstration was run against the separate `teleport_etl_demo` database.
