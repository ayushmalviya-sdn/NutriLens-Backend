# Use the official .NET 8 SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file from the App.Api directory
COPY App.Api/App.Api.sln ./App.Api/

# Copy the .csproj files of all libraries and API
COPY App.Api/*.csproj ./App.Api/
COPY App.Application/*.csproj ./App.Application/
COPY App.Common/*.csproj ./App.Common/
COPY App.Domain/*.csproj ./App.Domain/
COPY App.Infrastructure/*.csproj ./App.Infrastructure/
COPY App.Services.Test/*.csproj ./App.Services.Test/

# Restore dependencies for the solution
RUN dotnet restore App.Api/App.Api.sln

# Copy the rest of the files
COPY . .

# Publish the API project
WORKDIR /src/App.Api
RUN dotnet publish -c Release -o /app/publish

# Final runtime image (lightweight)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set environment variable for dynamic port (Railway provides PORT)
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "App.Api.dll"]
