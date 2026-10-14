# Destination Data Preparation

The files here are a small, reviewed demonstration dataset for the Teleport POC, not a live travel feed or a source of current prices/hours.

## Files

- `raw/destination-research.csv` records the researched labels, concise notes, and source URL for each destination or visitor activity.
- `cleaned/destinations.csv` normalizes each destination once for the database/API shape.
- `cleaned/activities.csv` normalizes activities and links each row to a destination by its stable name/region pair.
- `database/seed/002_demo_data.sql` inserts these rows into PostgreSQL. Re-running it does not overwrite existing destination or activity records.

## Review and sourcing

The attraction names and high-level descriptions were checked against official tourism pages on 2026-10-14:

- Visit Lisboa: [Castelo de São Jorge](https://www.visitlisboa.com/en/places/castelo-de-sao-jorge), [Jerónimos Monastery](https://www.visitlisboa.com/en/places/jeronimos-monastery), and [Belém Tower](https://www.visitlisboa.com/en/places/torre-de-belem).
- Visit Porto: [heritage attractions](https://visitporto.travel/en-GB/attraction/heritage) and [Clérigos Tower](https://visitporto.travel/en-GB/poi/5cd04b4bf979e00001939091).

The descriptions are short summaries, not copied marketing text. The region labels are the city labels used for this dataset; the app does not claim that these are formal tourism-region boundaries. No third-party images are included.

## Applying the seed

For an already initialized local `teleport_dev` database, apply the idempotent seed after confirming the schema exists:

```sh
psql -v ON_ERROR_STOP=1 -d teleport_dev -f database/seed/002_demo_data.sql
```

For a fresh local PostgreSQL container, Compose mounts the schema and seed as initialization scripts; PostgreSQL only runs those scripts when its data directory is first initialized. Back up any existing data before changing or recreating a database volume.
