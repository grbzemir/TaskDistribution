# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Restore first so the dependency layer is cached between builds
COPY Directory.Build.props ./
COPY src/TaskDistribution.Domain/TaskDistribution.Domain.csproj src/TaskDistribution.Domain/
COPY src/TaskDistribution.Application/TaskDistribution.Application.csproj src/TaskDistribution.Application/
COPY src/TaskDistribution.Infrastructure/TaskDistribution.Infrastructure.csproj src/TaskDistribution.Infrastructure/
COPY src/TaskDistribution.API/TaskDistribution.API.csproj src/TaskDistribution.API/
RUN dotnet restore src/TaskDistribution.API/TaskDistribution.API.csproj

COPY src/ src/
RUN dotnet publish src/TaskDistribution.API/TaskDistribution.API.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# The SQLite database lives in /app/data, which is mounted as a volume
RUN mkdir -p /app/data && chown "$APP_UID" /app/data

COPY --from=build /app/publish .

USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__DefaultConnection="Data Source=data/taskdistribution.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskDistribution.API.dll"]
