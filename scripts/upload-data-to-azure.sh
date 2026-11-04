#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/.." && pwd)"

: "${AZURE_STORAGE_ACCOUNT:?Set AZURE_STORAGE_ACCOUNT to an existing storage account name}"
: "${AZURE_STORAGE_CONTAINER:?Set AZURE_STORAGE_CONTAINER to the destination container name}"

azure_cli="$(command -v az || true)"
if [[ -z "$azure_cli" && -x "$HOME/.local/venvs/azure-cli/bin/az" ]]; then
  azure_cli="$HOME/.local/venvs/azure-cli/bin/az"
fi

if [[ -z "$azure_cli" ]]; then
  printf '%s\n' 'Azure CLI is required. Install it and sign in with az login.' >&2
  exit 1
fi

if ! "$azure_cli" account show --output none; then
  printf '%s\n' 'No active Azure login. Run az login before uploading.' >&2
  exit 1
fi

upload_args=(
  --account-name "$AZURE_STORAGE_ACCOUNT"
  --source "$repo_root/data"
  --destination "$AZURE_STORAGE_CONTAINER"
  --destination-path teleport
  --auth-mode login
  --only-show-errors
)

"$azure_cli" storage container create \
  --name "$AZURE_STORAGE_CONTAINER" \
  --account-name "$AZURE_STORAGE_ACCOUNT" \
  --auth-mode login \
  --output none

if [[ "${AZURE_BLOB_OVERWRITE:-false}" == "true" ]]; then
  upload_args+=(--overwrite)
fi

"$azure_cli" storage blob upload-batch "${upload_args[@]}"
printf 'Uploaded project data under teleport/ in container %s.\n' "$AZURE_STORAGE_CONTAINER"