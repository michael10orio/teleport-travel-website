-- Keep this filename after 001_initial.sql so a fresh PostgreSQL volume seeds after schema creation.
BEGIN;

INSERT INTO destinations (name, region, category, description, image_url)
VALUES
    (
        'Lisbon',
        'Lisboa',
        'City',
        'Portuguese city with heritage sites including Castelo de São Jorge, Jerónimos Monastery, and Belém Tower.',
        NULL
    ),
    (
        'Porto',
        'Porto',
        'City',
        'Portuguese city with heritage attractions including its Historic Centre and Clérigos Tower.',
        NULL
    )
ON CONFLICT (name, region) DO NOTHING;

INSERT INTO activities (destination_id, name, description)
SELECT destination.id, activity.name, activity.description
FROM (
    VALUES
        ('Lisbon', 'Lisboa', 'Castelo de São Jorge', 'Visit the castle and explore its historic grounds.'),
        ('Lisbon', 'Lisboa', 'Jerónimos Monastery', 'Visit the monastery and explore its heritage architecture.'),
        ('Lisbon', 'Lisboa', 'Belém Tower', 'Visit the historic riverside tower.'),
        ('Porto', 'Porto', 'Historic Centre of Porto', 'Explore the heritage area of Porto''s historic centre.'),
        ('Porto', 'Porto', 'Clérigos Tower', 'Visit the tower and its historic complex.')
) AS activity(destination_name, destination_region, name, description)
JOIN destinations AS destination
    ON destination.name = activity.destination_name
    AND destination.region = activity.destination_region
ON CONFLICT (destination_id, name) DO NOTHING;

COMMIT;
