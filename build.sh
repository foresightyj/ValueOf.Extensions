#!/usr/bin/env bash
dotnet build --configuration Release
dotnet pack --no-build --configuration Release
