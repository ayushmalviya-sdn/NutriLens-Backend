#!/bin/bash

# Start the .NET app using the port provided by Railway
dotnet App.Api.dll --urls "http://0.0.0.0:$PORT"
