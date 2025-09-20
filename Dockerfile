# syntax=docker/dockerfile:1

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080  # local/dev; en Render se usará $PORT

# --- Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG PROJECT_PATH=Api/Api.csproj  # cambia a la ruta real de tu .csproj
COPY . .
RUN dotnet restore "$PROJECT_PATH"
RUN dotnet publish "$PROJECT_PATH" -c Release -o /out \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true \
    /p:AssemblyName=app

# --- Final ---
FROM base AS final
WORKDIR /app
COPY --from=build /out .
# Expande $PORT en runtime (Render lo define). Por defecto 8080 localmente.
ENTRYPOINT ["sh","-c","ASPNETCORE_HTTP_PORTS=${PORT:-8080} dotnet app.dll"]

