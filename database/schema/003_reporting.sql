CREATE OR REPLACE VIEW v_destination_activity_counts AS
SELECT
    destination.id AS destination_id,
    destination.name AS destination_name,
    destination.region,
    COUNT(activity.id)::BIGINT AS activity_count
FROM destinations AS destination
LEFT JOIN activities AS activity
    ON activity.destination_id = destination.id
GROUP BY destination.id, destination.name, destination.region;

CREATE OR REPLACE VIEW v_trip_expense_summary AS
SELECT
    trip.id AS trip_id,
    trip.name AS trip_name,
    COUNT(expense.id)::BIGINT AS expense_count,
    SUM(expense.amount) AS total_expense_amount,
    AVG(expense.amount) AS average_expense_amount
FROM trips AS trip
LEFT JOIN expenses AS expense
    ON expense.trip_id = trip.id
GROUP BY trip.id, trip.name;

CREATE OR REPLACE FUNCTION fn_destination_activity_count(p_destination_id BIGINT)
RETURNS BIGINT
LANGUAGE SQL
STABLE
AS $$
    SELECT COUNT(*)::BIGINT
    FROM activities
    WHERE destination_id = p_destination_id;
$$;

CREATE OR REPLACE FUNCTION fn_trip_expense_total(p_trip_id BIGINT)
RETURNS NUMERIC
LANGUAGE SQL
STABLE
AS $$
    SELECT SUM(amount)
    FROM expenses
    WHERE trip_id = p_trip_id;
$$;
