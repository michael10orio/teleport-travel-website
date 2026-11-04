-- Teleport Week 8 reporting proof of concept.
-- Read-only; empty trip/expense results reflect the current database contents.

SELECT *
FROM v_destination_activity_counts
ORDER BY destination_name;

SELECT *
FROM v_trip_expense_summary
ORDER BY trip_name;

SELECT
    destination.id AS destination_id,
    destination.name AS destination_name,
    fn_destination_activity_count(destination.id) AS activity_count
FROM destinations AS destination
ORDER BY destination.name;

SELECT
    trip.id AS trip_id,
    trip.name AS trip_name,
    fn_trip_expense_total(trip.id) AS total_expense_amount
FROM trips AS trip
ORDER BY trip.name;