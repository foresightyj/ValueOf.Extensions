#!/usr/bin/env bash
set -x
find . -type f -name '*.nupkg' -exec rm {} \;
dotnet build --configuration Release
dotnet pack --no-build --configuration Release
