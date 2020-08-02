# Simple Event Sourced Application

[![CodeFactor](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure/badge)](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure)

This repository contains the source code for a simple toy project using CQRS and event sourcing.

The scope for this project is:

* Create an API-driven application for managing verterinary appointments for canine companions
* Use event sourcing and CQRS for persistence
* Provision a Kubernetes cluster in Azure
* Package and deploy the application to the cluster
* Include both unit and integration tests

Additional work that isn't in scope but may be done anyway:

* Authentication
* Automated build/deploy of both infrastructure and code using Azure DevOps
* Switch from Azure to AWS

## Motivation

It has been a while since I got to use all of the muscles required by this project, and this is an opportunity to 
work them.

## Local Development

Install the latest .NET Core 3.1 SDK

### Using Docker

Build using `docker build -t petdoctor-api .`

Run using `docker run -d -p 5640:80 petdoctor-api`

Omit `-d` if you don't want the container to run past the scope of your terminal session

You'll want to point `ConnectionStrings__PetDoctorContext` at a local SQL instance (host.docker.internal)

### Using Compose

Docker compose is configured to run a SQL image alongside the api image as a dependency, on port 1400.

To build: in a terminal with the working directory set to the repository root, `docker-compose build`

To run: same deal, but `docker-compose run`

To connect to the database from SSMS, use `.,1400` as the server name and SQL Server Authentication as `sa` (the 
password is in docker-compose.yml)

### Database Migrations

This project uses code-first migrations. You'll need to install dotnet-ef:

```
dotnet tool update --global dotnet-ef
```

Example command to generate a migration:

```
dotnet ef migrations add InitialPetDoctorMigration -c PetDoctorContext -o Infrastructure/Migrations/PetDoctor/PetDoctorDb --startup-project src/PetDoctor.API
```

### Tests

Tests can be run as usual using Visual Studio//ReSharper. They can also be run in Docker using compose:

To build the integration tests, use `docker-compose build integrationtests` and then `docker-compose up integrationtests` to run them

To build the unit tests, use `docker-compose build unittests` and then `docker-compose up unittests` to run them

## Infrastructure & Deployment

Infrastructure is provisioned using pulumi. cd to the `Petdoctor.InfrastructureStack` csproj and run:

`pulumi up`

To tear the resources down run:

`pulumi destroy`