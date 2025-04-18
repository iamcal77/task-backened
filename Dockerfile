# Use the official .NET 9 SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore
COPY . ./
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Set the entry point
ENTRYPOINT ["dotnet", "MyWebAPI.dll"]
