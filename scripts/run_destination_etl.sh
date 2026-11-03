#!/usr/bin/env bash
set -euo pipefail

project_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$project_root"

for command_name in python3 psql; do
    if ! command -v "$command_name" >/dev/null 2>&1; then
        printf 'Required command not found: %s\n' "$command_name" >&2
        exit 1
    fi
done

export PGHOST="${PGHOST:-localhost}"
export PGPORT="${PGPORT:-5432}"
export PGDATABASE="${PGDATABASE:-teleport_dev}"
export PGUSER="${PGUSER:-$(id -un)}"

psql --no-psqlrc --set=ON_ERROR_STOP=1 --file database/schema/001_initial.sql
python3 scripts/transform_destination_research.py
psql --no-psqlrc --set=ON_ERROR_STOP=1 --file scripts/load_destination_data.sql
