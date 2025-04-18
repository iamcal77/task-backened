# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyWebAPI/MyWebAPI.csproj", "MyWebAPI/"]
RUN dotnet restore "MyWebAPI/MyWebAPI.csproj"
COPY . .
WORKDIR "/src/MyWebAPI"
RUN dotnet build "MyWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyWebAPI.csproj" -c Release -o /app/publish

# Final stage: Run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyWebAPI.dll"]
