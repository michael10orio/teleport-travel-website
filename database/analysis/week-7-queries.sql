-- Teleport Week 7 analytical and data-quality queries.
-- Read-only queries; results depend on the current real database rows.

-- 1. Join each destination to its activities.
SELECT
    destination.name AS destination,
    destination.region,
    activity.name AS activity,
    activity.description
FROM destinations AS destination
JOIN activities AS activity
    ON activity.destination_id = destination.id
ORDER BY destination.name, activity.name;

-- 2. Filter destinations by a reviewed region label.
SELECT id, name, region, category
FROM destinations
WHERE region = 'Lisboa'
ORDER BY name;

-- 3. Count destinations by category.
SELECT category, COUNT(*) AS destination_count
FROM destinations
GROUP BY category
ORDER BY category;

-- 4. Count activities per destination, including destinations with none.
SELECT destination.name, COUNT(activity.id) AS activity_count
FROM destinations AS destination
LEFT JOIN activities AS activity
    ON activity.destination_id = destination.id
GROUP BY destination.id, destination.name
ORDER BY destination.name;

-- 5. Count itinerary entries for each trip.
-- Returns no rows while no real trips have been recorded.
SELECT trip.id, trip.name, COUNT(item.id) AS itinerary_item_count
FROM trips AS trip
LEFT JOIN itinerary_items AS item
    ON item.trip_id = trip.id
GROUP BY trip.id, trip.name
ORDER BY trip.name;

-- 6. Summarize expense count, total, and average per trip.
-- Returns no rows while there are no real trips.
SELECT
    trip.id,
    trip.name,
    COUNT(expense.id) AS expense_count,
    SUM(expense.amount) AS total_expense_amount,
    AVG(expense.amount) AS average_expense_amount
FROM trips AS trip
LEFT JOIN expenses AS expense
    ON expense.trip_id = trip.id
GROUP BY trip.id, trip.name
ORDER BY trip.name;

-- 7. Overall expense metrics.
-- With no expenses, COUNT is 0 and SUM/AVG are NULL; do not interpret them as measured zero spending.
SELECT
    COUNT(*) AS expense_count,
    SUM(amount) AS total_expense_amount,
    AVG(amount) AS average_expense_amount
FROM expenses;

-- 8. Count null, blank, duplicate, and inconsistent values by table.
SELECT 'destinations blank name' AS quality_check, COUNT(*) AS issue_count
FROM destinations
WHERE name IS NULL OR length(trim(name)) = 0
UNION ALL
SELECT 'destinations blank region', COUNT(*)
FROM destinations
WHERE region IS NULL OR length(trim(region)) = 0
UNION ALL
SELECT 'duplicate destination key', COUNT(*)
FROM (
    SELECT name, region
    FROM destinations
    GROUP BY name, region
    HAVING COUNT(*) > 1
) AS duplicate_keys
UNION ALL
SELECT 'activities without destination', COUNT(*)
FROM activities AS activity
LEFT JOIN destinations AS destination
    ON destination.id = activity.destination_id
WHERE destination.id IS NULL
UNION ALL
SELECT 'trips with invalid date range', COUNT(*)
FROM trips
WHERE start_date IS NOT NULL
  AND end_date IS NOT NULL
  AND end_date < start_date
UNION ALL
SELECT 'expenses with non-positive amount', COUNT(*)
FROM expenses
WHERE amount <= 0;

-- 9. Check whether destination categories are consistently populated.
SELECT category, COUNT(*) AS destination_count
FROM destinations
GROUP BY category
HAVING category IS NULL OR length(trim(category)) = 0
ORDER BY category;

-- 10. Identify activities whose destination natural key is missing.
SELECT activity.destination_id, activity.name
FROM activities AS activity
LEFT JOIN destinations AS destination
    ON destination.id = activity.destination_id
WHERE destination.id IS NULL
ORDER BY activity.destination_id, activity.name;
