#!/bin/bash
set -e

# Install npm dependencies
if [ -d "web" ]; then
    pushd web
    npm install
    popd
else
    echo "Directory 'web' does not exist."
    exit 1
fi

# Restore .NET dependencies
if [ -d "src" ]; then
    pushd src
    dotnet restore
    popd
else
    echo "Directory 'src' does not exist."
    exit 1
fi

# Trust .NET development certificates
dotnet dev-certs https --trust