#!/bin/sh
set -euo pipefail

# Setup cookie jar to store and use session cookie
COOKIE_JAR=$(mktemp)
trap 'rm -f "$COOKIE_JAR"' EXIT


# First time signup for admin user, save access token in cookie jar
req_body=$(jq -n --arg email "${ADMIN_EMAIL}" '{ email: $email, username: "admin", firstName: "admin" }')
admin_user_id=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/signup/setup" \
  -H 'Content-Type: application/json' \
  -c "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.id')


# Create user groups: admins, grafana
req_body=$(jq -n '{ name: "admins", friendlyName: "Admins"}')
admin_group_id=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/user-groups" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.id')

req_body=$(jq -n '{ name: "grafana", friendlyName: "Grafana"}')
grafana_group_id=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/user-groups" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.id')


# Add admin user to all groups
req_body=$(jq -n \
  --arg adminsId "${admin_group_id}" \
  --arg grafanaId "${grafana_group_id}" \
  '{ userGroupIds: [ $adminsId, $grafanaId ] }')
curl -fsS \
  -X PUT "$POCKET_ID_BASE_URL/api/users/$admin_user_id/user-groups" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" >/dev/null


# Create webapp OIDC client + generate client secret
req_body=$(jq -n \
  --arg appCallback "${APP_CALLBACK}" \
  --arg appLogoutCallback "${APP_LOGOUT_CALLBACK}" \
  '{
     name: "AoS Adjutant"
    ,isGroupRestricted: false
    ,pkceEnabled: true
    ,isPublic: false
    ,callbackURLs: [ $appCallback ]
    ,logoutCallbackURLs: [ $appLogoutCallback ]
  }'
)
webapp_client_id=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/oidc/clients" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.id')

webapp_client_secret=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/oidc/clients/$webapp_client_id/secret" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  | jq -r '.secret')


# Create Grafana OIDC client + generate client secret
req_body=$(jq -n \
  --arg grafanaCallback "${GRAFANA_CALLBACK}" \
  '{
     name: "Grafana"
    ,isGroupRestricted: true
    ,pkceEnabled: true
    ,isPublic: false
    ,callbackURLs: [ $grafanaCallback ]
  }'
)
grafana_client_id=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/oidc/clients" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.id')

grafana_client_secret=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/oidc/clients/$grafana_client_id/secret" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  | jq -r '.secret')


# Add grafana group to allowed user groups of Grafana client
req_body=$(jq -n \
  --arg grafanaGroupId "${grafana_group_id}" \
  '{ userGroupIds: [ $grafanaGroupId ] }')
curl -fsS \
  -X PUT "$POCKET_ID_BASE_URL/api/oidc/clients/$grafana_client_id/allowed-user-groups" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" >/dev/null


# Generate login code
req_body=$(jq -n '{ ttl: 3600 }')
token=$(curl -fsS \
  -X POST "$POCKET_ID_BASE_URL/api/users/$admin_user_id/one-time-access-token" \
  -H 'Content-Type: application/json' \
  -b "$COOKIE_JAR" \
  -d "$req_body" \
  | jq -r '.token')

echo "Pocket ID config complete.
Login and add a passkey with the following link to complete the setup: ${POCKET_ID_BASE_URL}/lc/${token}
"

umask 077
cat > /secrets/pocket-id.env <<EOF
WEBAPP_CLIENT_ID=${webapp_client_id}
WEBAPP_CLIENT_SECRET=${webapp_client_secret}
GRAFANA_CLIENT_ID=${grafana_client_id}
GRAFANA_CLIENT_SECRET=${grafana_client_secret}
EOF

echo "Pocket ID env vars written to .secrets/pocket-id.env. Move them to your secrets store and remove the file."
