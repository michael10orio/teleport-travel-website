#!/usr/bin/env python3
"""Validate and normalize the reviewed destination research CSV."""

from __future__ import annotations

import argparse
import csv
import json
import unicodedata
from pathlib import Path

EXPECTED_COLUMNS = {
    "destination",
    "region",
    "category",
    "record_type",
    "name",
    "description",
    "source_url",
}


def normalize(value: str | None) -> str:
    return " ".join(unicodedata.normalize("NFC", value or "").split())


def write_csv(path: Path, columns: list[str], rows: list[dict[str, str]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as output_file:
        writer = csv.DictWriter(output_file, fieldnames=columns, lineterminator="\n")
        writer.writeheader()
        writer.writerows(rows)


def transform(source_path: Path, output_directory: Path) -> dict[str, int]:
    destinations: dict[tuple[str, str], dict[str, str]] = {}
    activities: dict[tuple[str, str, str], dict[str, str]] = {}
    source_records = 0
    duplicate_records_removed = 0

    with source_path.open("r", encoding="utf-8-sig", newline="") as source_file:
        reader = csv.DictReader(source_file)
        columns = set(reader.fieldnames or [])
        missing_columns = sorted(EXPECTED_COLUMNS - columns)
        if missing_columns:
            raise ValueError(f"Source CSV is missing required columns: {', '.join(missing_columns)}")

        for line_number, raw_row in enumerate(reader, start=2):
            source_records += 1
            row = {key: normalize(value) for key, value in raw_row.items() if key is not None}
            record_type = row["record_type"].casefold()
            required_values = [
                row["destination"],
                row["region"],
                row["category"],
                row["name"],
                row["description"],
                row["source_url"],
            ]

            if record_type not in {"destination", "activity"}:
                raise ValueError(f"Line {line_number}: unsupported record_type {row['record_type']!r}")
            if any(not value for value in required_values):
                raise ValueError(f"Line {line_number}: required values must not be blank")

            if record_type == "destination":
                destination = {
                    "name": row["destination"],
                    "region": row["region"],
                    "category": row["category"],
                    "description": row["description"],
                    "image_url": "",
                }
                key = (destination["name"].casefold(), destination["region"].casefold())
                existing = destinations.get(key)
                if existing is not None:
                    if existing != destination:
                        raise ValueError(f"Line {line_number}: conflicting duplicate destination")
                    duplicate_records_removed += 1
                    continue
                destinations[key] = destination
            else:
                activity = {
                    "destination_name": row["destination"],
                    "destination_region": row["region"],
                    "name": row["name"],
                    "description": row["description"],
                }
                key = (
                    activity["destination_name"].casefold(),
                    activity["destination_region"].casefold(),
                    activity["name"].casefold(),
                )
                existing = activities.get(key)
                if existing is not None:
                    if existing != activity:
                        raise ValueError(f"Line {line_number}: conflicting duplicate activity")
                    duplicate_records_removed += 1
                    continue
                activities[key] = activity

    destination_keys = set(destinations)
    orphan_activities = [
        activity
        for activity in activities.values()
        if (
            activity["destination_name"].casefold(),
            activity["destination_region"].casefold(),
        ) not in destination_keys
    ]
    if orphan_activities:
        names = ", ".join(activity["name"] for activity in orphan_activities)
        raise ValueError(f"Activities refer to unknown destinations: {names}")

    destination_rows = list(destinations.values())
    activity_rows = list(activities.values())

    write_csv(
        output_directory / "destinations.csv",
        ["name", "region", "category", "description", "image_url"],
        destination_rows,
    )
    write_csv(
        output_directory / "activities.csv",
        ["destination_name", "destination_region", "name", "description"],
        activity_rows,
    )

    return {
        "source_records": source_records,
        "destination_rows": len(destination_rows),
        "activity_rows": len(activity_rows),
        "duplicate_records_removed": duplicate_records_removed,
        "orphan_activities": 0,
        "invalid_rows": 0,
    }


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--source",
        type=Path,
        default=Path("data/raw/destination-research.csv"),
        help="Reviewed raw research CSV input.",
    )
    parser.add_argument(
        "--output-directory",
        type=Path,
        default=Path("data/cleaned"),
        help="Directory for normalized destination and activity CSV files.",
    )
    arguments = parser.parse_args()
    print(json.dumps(transform(arguments.source, arguments.output_directory), indent=2))


if __name__ == "__main__":
    main()
