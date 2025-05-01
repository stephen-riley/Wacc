#!/usr/bin/env bash
pushd $(dirname "$0") > /dev/null
dotnet run --project src $@
popd > /dev/null