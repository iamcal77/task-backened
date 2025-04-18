# Use the official .NET 9.0 runtime image as the base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK 9.0 to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyWebAPI/MyWebAPI.csproj", "MyWebAPI/"]
RUN dotnet restore "MyWebAPI/MyWebAPI.csproj"
COPY . .
WORKDIR "/src/MyWebAPI"
RUN dotnet build "MyWebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MyWebAPI.csproj" -c Release -o /app/publish

# Final stage: Use the runtime base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyWebAPI.dll"]
