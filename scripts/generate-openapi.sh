#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/.."
REPO_ROOT=$(pwd)
API_DIR="$REPO_ROOT/backend/src/AosAdjutant.Api"

dotnet build "$API_DIR/AosAdjutant.Api.csproj" --no-restore -v quiet
cp "$API_DIR/obj/AosAdjutant.Api.json" "$REPO_ROOT/docs/openapi.json" 
echo "OpenApi document written to $REPO_ROOT/docs/openapi.json"
