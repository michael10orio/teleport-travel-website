#!/usr/bin/env bash
set -euo pipefail

API_BASE_URL="${API_BASE_URL:-http://127.0.0.1:5079}"
API_BASE_URL="${API_BASE_URL%/}"

for command_name in curl node; do
  if ! command -v "$command_name" >/dev/null 2>&1; then
    printf 'Required command not found: %s\n' "$command_name" >&2
    exit 1
  fi
done

trip_id=""
last_body=""

request() {
  local method="$1"
  local path="$2"
  local body="${3:-}"

  if [[ -n "$body" ]]; then
    curl --silent --show-error \
      --request "$method" \
      --url "${API_BASE_URL}${path}" \
      --header 'Content-Type: application/json' \
      --data "$body" \
      --write-out $'\n%{http_code}'
  else
    curl --silent --show-error \
      --request "$method" \
      --url "${API_BASE_URL}${path}" \
      --write-out $'\n%{http_code}'
  fi
}

json_length() {
  node -e 'process.stdout.write(String(JSON.parse(process.argv[1]).length))' "$1"
}

json_id() {
  node -e 'process.stdout.write(String(JSON.parse(process.argv[1]).id))' "$1"
}

json_first_id() {
  node -e 'process.stdout.write(String(JSON.parse(process.argv[1])[0].id))' "$1"
}

json_has_name() {
  node -e 'process.exit(JSON.parse(process.argv[1]).some((item) => item.name === process.argv[2]) ? 0 : 1)' "$1" "$2"
}

assert_status() {
  local expected="$1"
  local response="$2"
  local status="${response##*$'\n'}"

  if [[ "$status" != "$expected" ]]; then
    printf 'Expected HTTP %s, got HTTP %s\n' "$expected" "$status" >&2
    printf 'Response: %s\n' "${response%$'\n'*}" >&2
    return 1
  fi

  last_body="${response%$'\n'*}"
  printf 'HTTP %s\n' "$status"
}

cleanup() {
  if [[ -n "$trip_id" ]]; then
    if ! curl --silent --show-error --fail \
      --request DELETE \
      --url "${API_BASE_URL}/api/trips/${trip_id}" >/dev/null; then
      printf 'Cleanup warning: test trip %s could not be deleted.\n' "$trip_id" >&2
    fi
  fi
}
trap cleanup EXIT

assert_status 200 "$(request GET /api/health)"
assert_status 200 "$(request GET /openapi/v1.json)"
assert_status 200 "$(request GET /api/destinations)"
destination_count="$(json_length "$last_body")"
if (( destination_count < 1 )); then
  printf 'No destination records are available; apply the reviewed demo seed first.\n' >&2
  exit 1
fi

assert_status 200 "$(request GET '/api/destinations?search=Lisbon')"
destination_id="$(json_first_id "$last_body")"
assert_status 200 "$(request GET "/api/destinations/${destination_id}/activities")"
activity_count="$(json_length "$last_body")"
if (( activity_count < 1 )); then
  printf 'Destination %s has no activities; apply the reviewed demo seed first.\n' "$destination_id" >&2
  exit 1
fi
assert_status 404 "$(request GET /api/trips/0)"
assert_status 404 "$(request GET /api/trips/0/expenses)"
assert_status 200 "$(request GET /api/trips)"

invalid_trip='{"name":" ","startDate":null,"endDate":null}'
assert_status 400 "$(request POST /api/trips "$invalid_trip")"

trip_name="Teleport API smoke test $(date -u +%Y%m%dT%H%M%SZ)"
new_trip="$(node -e 'console.log(JSON.stringify({name:process.argv[1],startDate:null,endDate:null}))' "$trip_name")"
assert_status 201 "$(request POST /api/trips "$new_trip")"
trip_id="$(json_id "$last_body")"
assert_status 200 "$(request GET "/api/trips/${trip_id}")"
assert_status 200 "$(request GET /api/trips)"
if ! json_has_name "$last_body" "$trip_name"; then
  printf 'Created trip was not returned by the trip list.\n' >&2
  exit 1
fi
updated_trip="$(node -e 'console.log(JSON.stringify({name:process.argv[1],startDate:"2026-10-20",endDate:"2026-10-21"}))' \
  "${trip_name} updated")"
assert_status 200 "$(request PUT "/api/trips/${trip_id}" "$updated_trip")"

itinerary_item="{\"destinationId\":${destination_id},\"activityId\":null,\"sortOrder\":0}"
assert_status 201 "$(request POST "/api/trips/${trip_id}/itinerary-items" "$itinerary_item")"
itinerary_id="$(json_id "$last_body")"
assert_status 200 "$(request GET "/api/trips/${trip_id}/itinerary-items")"
assert_status 200 "$(request PUT "/api/trips/${trip_id}/itinerary-items/${itinerary_id}" \
  '{"sortOrder":1}')"
assert_status 204 "$(request DELETE "/api/trips/${trip_id}/itinerary-items/${itinerary_id}")"

expense='{"description":"Smoke test expense","amount":12.5,"expenseDate":null}'
invalid_expense='{"description":" ","amount":0,"expenseDate":null}'
assert_status 400 "$(request POST "/api/trips/${trip_id}/expenses" "$invalid_expense")"
assert_status 201 "$(request POST "/api/trips/${trip_id}/expenses" "$expense")"
expense_id="$(json_id "$last_body")"
assert_status 200 "$(request GET "/api/trips/${trip_id}/expenses")"
updated_expense='{"description":"Updated smoke test expense","amount":13.5,"expenseDate":null}'
assert_status 200 "$(request PUT "/api/trips/${trip_id}/expenses/${expense_id}" "$updated_expense")"
assert_status 204 "$(request DELETE "/api/trips/${trip_id}/expenses/${expense_id}")"
assert_status 204 "$(request DELETE "/api/trips/${trip_id}")"
trip_id=""

printf 'API smoke test passed; its uniquely named trip and child records were removed.\n'
