# Simple Event Sourced Application

[![CodeFactor](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure/badge)](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure)

This repository contains the source code for a simple toy project using CQRS and event sourcing.

Features:

* [x] CQRS (separate read and write stores) with event sourcing
* [x] Code-first migrations, managed in separate console apps (not at application start of the dependent app(s))
* [x] A pretty rough first cut of DDD
* [x] Integration and unit tests
* [x] Everything runs in docker compose
* [x] Authentication/authorization (using a simple instance of Identity Server 4)

Broken, incomplete, or aspirational:

* [ ] Working AKS stack in Azure
* [ ] Infrastructure as Code using ~~Pulumi~~ bicep
* [ ] helm charts for app deployments
* [ ] CI/CD pipeline in GitHub Actions
* [ ] A richer domain implementation
* [ ] UI

## Running locally

**In docker compose**:

```
// change directory to the build folder
docker-compose build
docker-compose up api
```

**In an IDE like Visual Studio**:

1. Add configuration to user secrets for the read migrations project (sample below)
2. Run the read migrations project
3. Add configuration to user secrets for the write migrations project (sample below)
4. Make sure the identity project is running
5. Run the API project

**Tests**:

Docker compose happily runs using either of these:

```
docker-compose up integration-tests
docker-compose up unit-tests
```

Tests will also run in Visual Studio or similar, and don't require migrations to be run first
(databases are migrated on an independent connection).
