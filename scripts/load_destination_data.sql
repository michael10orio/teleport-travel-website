\set ON_ERROR_STOP on
BEGIN;

CREATE TEMP TABLE stage_destinations (
    name TEXT,
    region TEXT,
    category TEXT,
    description TEXT,
    image_url TEXT
) ON COMMIT DROP;

CREATE TEMP TABLE stage_activities (
    destination_name TEXT,
    destination_region TEXT,
    name TEXT,
    description TEXT
) ON COMMIT DROP;

\copy stage_destinations (name, region, category, description, image_url) FROM 'data/cleaned/destinations.csv' WITH (FORMAT csv, HEADER true, NULL '')
\copy stage_activities (destination_name, destination_region, name, description) FROM 'data/cleaned/activities.csv' WITH (FORMAT csv, HEADER true, NULL '')

SELECT 'destination rows' AS quality_check, count(*) AS checked_rows
FROM stage_destinations
UNION ALL
SELECT 'blank destination fields', count(*)
FROM stage_destinations
WHERE name IS NULL OR length(trim(name)) = 0
   OR region IS NULL OR length(trim(region)) = 0
   OR category IS NULL OR length(trim(category)) = 0
   OR description IS NULL OR length(trim(description)) = 0
UNION ALL
SELECT 'duplicate destination keys', count(*)
FROM (
    SELECT name, region
    FROM stage_destinations
    GROUP BY name, region
    HAVING count(*) > 1
) AS duplicates
UNION ALL
SELECT 'activity rows', count(*)
FROM stage_activities
UNION ALL
SELECT 'blank activity fields', count(*)
FROM stage_activities
WHERE destination_name IS NULL OR length(trim(destination_name)) = 0
   OR destination_region IS NULL OR length(trim(destination_region)) = 0
   OR name IS NULL OR length(trim(name)) = 0
   OR description IS NULL OR length(trim(description)) = 0
UNION ALL
SELECT 'duplicate activity keys', count(*)
FROM (
    SELECT destination_name, destination_region, name
    FROM stage_activities
    GROUP BY destination_name, destination_region, name
    HAVING count(*) > 1
) AS duplicates
UNION ALL
SELECT 'activities without staged destination', count(*)
FROM stage_activities AS activity
LEFT JOIN stage_destinations AS destination
    ON destination.name = activity.destination_name
    AND destination.region = activity.destination_region
WHERE destination.name IS NULL;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM stage_destinations
        WHERE name IS NULL OR length(trim(name)) = 0
           OR region IS NULL OR length(trim(region)) = 0
           OR category IS NULL OR length(trim(category)) = 0
           OR description IS NULL OR length(trim(description)) = 0
    ) THEN
        RAISE EXCEPTION 'ETL stopped: destination fields contain null or blank values';
    END IF;

    IF EXISTS (
        SELECT 1 FROM stage_activities
        WHERE destination_name IS NULL OR length(trim(destination_name)) = 0
           OR destination_region IS NULL OR length(trim(destination_region)) = 0
           OR name IS NULL OR length(trim(name)) = 0
           OR description IS NULL OR length(trim(description)) = 0
    ) THEN
        RAISE EXCEPTION 'ETL stopped: activity fields contain null or blank values';
    END IF;

    IF EXISTS (
        SELECT 1 FROM stage_destinations
        GROUP BY name, region
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'ETL stopped: duplicate destination keys found';
    END IF;

    IF EXISTS (
        SELECT 1 FROM stage_activities
        GROUP BY destination_name, destination_region, name
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'ETL stopped: duplicate activity keys found';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM stage_activities AS activity
        LEFT JOIN stage_destinations AS destination
            ON destination.name = activity.destination_name
            AND destination.region = activity.destination_region
        WHERE destination.name IS NULL
    ) THEN
        RAISE EXCEPTION 'ETL stopped: activity references a destination missing from the staged source';
    END IF;
END $$;

INSERT INTO destinations (name, region, category, description, image_url)
SELECT name, region, category, description, NULLIF(image_url, '')
FROM stage_destinations
ON CONFLICT (name, region) DO NOTHING;

INSERT INTO activities (destination_id, name, description)
SELECT destination.id, activity.name, activity.description
FROM stage_activities AS activity
JOIN destinations AS destination
    ON destination.name = activity.destination_name
    AND destination.region = activity.destination_region
ON CONFLICT (destination_id, name) DO NOTHING;

SELECT 'destinations in database' AS result, count(*) AS row_count FROM destinations
UNION ALL
SELECT 'activities in database', count(*) FROM activities;

COMMIT;
