# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore
COPY *.sln ./
COPY SeMTG.API/*.csproj ./SeMTG.API/
RUN dotnet restore

# Copy the rest and publish
COPY SeMTG.API/. ./SeMTG.API/
WORKDIR /app/SeMTG.API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (adjust if your app uses a different port)
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "SeMTG.API.dll"]
