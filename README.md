# swapfiets-bikes-api

A lightweight REST API that exposes real-time bike availability and type data for the Swapfiets fleet. The service is hosted on Azure App Service and deployed via GitHub Actions.

This is a live code review exercise. Please share your screen and avoid using AI assistants.

---

## Overview

The API provides the following endpoints:

| Endpoint | Description |
|---|---|
| `GET /api/bikes` | Returns the full list of bikes with their availability status and type |
| `GET /api/bikes/{id}` | Returns a single bike, enriched with its last known location from the fleet telemetry service |
| `POST /api/bikes/{id}/reservations` | Reserves an available bike for a customer and pre-authorises the payment |

Bike types are either `classic` or `electric`. Status values are `available` or `in_use`.

---

## Local Setup

**Prerequisites**

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for container testing)

**Run the API locally**

```bash
cd src/SwapfietsApi
dotnet run
```

The API will be available at `http://localhost:5000`.

**Run with Docker**

```bash
docker build -t swapfiets-bikes-api .
docker run -p 8080:80 swapfiets-bikes-api
```

The API will be available at `http://localhost:8080`.

---

## Project Structure

```
src/
  SwapfietsApi/           # .NET 8 Web API
    Controllers/
      BikesController.cs  # Bike availability and reservation endpoints
    Services/
      FleetTelemetryClient.cs  # Client for the fleet telemetry service
    Program.cs            # App entry point
infra/
  main.bicep              # Azure infrastructure (App Service Plan + App Service)
Dockerfile                # Multi-stage container build
.github/
  workflows/
    pipeline.yml          # CI/CD pipeline
```

---

## Pipeline

The GitHub Actions pipeline runs on every push to `main` and on pull requests.

**Jobs**

| Job | Description |
|---|---|
| `build` | Restores and compiles the .NET application |
| `test` | Runs the test suite |
| `docker` | Builds the Docker image and pushes to Azure Container Registry |
| `deploy` | Deploys the new image to the production App Service |

**Infrastructure**

The pipeline deploys to Azure App Service (`swapfiets-bikes-api-prd`). The infrastructure is provisioned separately via the Bicep template in `infra/main.bicep`.

---

## Infrastructure

The Bicep template provisions:

- **App Service Plan** — Linux B2 plan
- **App Service** — Container-based deployment from ACR
- **Managed Identity** — System-assigned identity for Azure resource access

To deploy the infrastructure:

```bash
az deployment sub create \
  --location westeurope \
  --template-file infra/main.bicep
```

---

## Configuration

The following GitHub Actions secrets are required:

| Secret | Description |
|---|---|
| `ACR_USERNAME` | Azure Container Registry username |
| `ACR_PASSWORD` | Azure Container Registry password |
