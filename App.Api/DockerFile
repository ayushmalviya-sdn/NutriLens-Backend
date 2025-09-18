# Use the official .NET 8 SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file (if you have one). If not, you can restore via csproj of App.Api
COPY *.sln ./

# Copy project files of all libraries and API
COPY App.Api/*.csproj ./App.Api/
COPY App.Application/*.csproj ./App.Application/
COPY App.Common/*.csproj ./App.Common/
COPY App.Domain/*.csproj ./App.Domain/
COPY App.Infrastructure/*.csproj ./App.Infrastructure/
# If you want tests built (optional), you can include:
COPY App.Services.Test/*.csproj ./App.Services.Test/

# Restore all
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the API project
WORKDIR /src/App.Api
RUN dotnet publish -c Release -o /app/publish

# Build final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Set environment variable for dynamic port (Railway provides PORT)
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "App.Api.dll"]
