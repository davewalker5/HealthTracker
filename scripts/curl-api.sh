#!/bin/sh -f

API_SERVER="https://0.0.0.0:54468"
USERNAME=
PASSWORD=

# Authenticate to get an API token
URL="$API_SERVER/users/authenticate"
BODY=$(printf '{ "username": "%s", "password": "%s" }' "$USERNAME" "$PASSWORD")

echo "Authentication endpoint : $URL"
echo "Request Body            : $BODY"

TOKEN=$(curl -s --insecure -H "Accept: text/plain" -H "Content-Type: application/json" --data "$BODY" -X POST "$URL")
echo "API Token               : ${TOKEN:0:50}..."

# Send a GET request to the target endpoint. Expect a JSON response and pipe it into
# jq to format it into a more readable form:
#
# https://jqlang.org/

URL="$API_SERVER/$1"
echo "Target endpoint         : $URL"

curl -s --insecure -H "Authorization: Bearer $TOKEN" -H "Accept: application/json" -H "Content-Type: application/json" -X GET "$URL" | jq
