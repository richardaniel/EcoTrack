# syntax=docker/dockerfile:1

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# --- Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG PROJECT_PATH=Ecotrack_Api.csproj   # <--- en la raíz
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
ENTRYPOINT ["sh","-c","dotnet app.dll --urls http://0.0.0.0:${PORT:-8080}"]

