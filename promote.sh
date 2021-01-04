#!/bin/bash

ORG="henifazzani"
SYSTEM_TEAMPROJECT="SynkerAPI"
VSTS_FEED_PAT="en2opsg4ouqijp4udrykiukq5tqdbvjjon6tsrdtbeeqa77pde7a"
Project="" ## TODO: gérer le cas des feeds scopés par projet

vsts_feed="packages"
packageName="GDrive.Anomalies.Library"
packageVersion="1.3.7"
viewId="Release"

api_version="?api-version=5.0-preview.1"
base_feeds_url="https://feeds.dev.azure.com/${ORG}/_apis/packaging/feeds"
list_feeds=$(curl -f -s -X GET -H 'Accept: application/json' -u ":$VSTS_FEED_PAT" "${base_feeds_url}${api_version}")
feedId=$(echo "$list_feeds" | jq --arg f $vsts_feed -c '[ .value[] | select( .name | contains($f)) ]' | jq -r '.[0].id')
# feedUrl="$base_feeds_url/${feedId}${api_version}"
# response=$(curl -H 'Accept: application/json' -u ":$VSTS_FEED_PAT" "$feedUrl")
# echo "$response" | jq .

## Get packages
# GET https://feeds.dev.azure.com/{organization}/{project}/_apis/packaging/Feeds/{feedId}/packages?api-version=6.0-preview.1
# base_packages_uri="https://feeds.dev.azure.com/${ORG}/_apis/packaging/Feeds/${feedId}/packages"
# list_pkg=$(curl -X GET -H 'Accept: application/json' -u ":$VSTS_FEED_PAT" "${base_packages_uri}${api_version}")
# pkgId=$(echo "$list_pkg" | jq --arg f $packageName -c '[ .value[] | select( .name | contains($f)) ]' | jq -r '.[0].id')
# pkg_info=$(curl -X GET -H 'Accept: application/json' -u ":$VSTS_FEED_PAT" "${base_packages_uri}/${pkgId}${api_version}")

# GET https://feeds.dev.azure.com/{organization}/_apis/packaging/Feeds/{feedId}/packages/{packageId}?api-version=6.0-preview.1
#   "https://pkgs.dev.azure.com/_apis/packaging/feeds/9a9327ae-8c62-4cc1-80a5-7365f97a5b87/nuget/packages/GDrive.Anomalies.Library/versions/1.3.8?api-version=5.0-preview.1"
url="https://pkgs.dev.azure.com/${ORG}/_apis/packaging/feeds/${feedId}/nuget/packages/${packageName}/versions/${packageVersion}$api_version"
exit_code=$(curl -f -s -H 'Accept: application/json' -H 'Content-Type: application/json' -u ":$VSTS_FEED_PAT" --data "{"views": { 'op': 0, 'path': '/views/-', 'value': '$viewId'}}" -X PATCH "$url")
return $exit_code