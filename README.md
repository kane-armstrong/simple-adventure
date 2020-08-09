# Simple Event Sourced Application

[![CodeFactor](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure/badge)](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure)

This repository contains the source code for a simple toy project using CQRS and event sourcing. I've built it to stretch muscles I haven't used for a while, to try
out new stuff, and possibly generate some content for my own future reference. 

Some of the stuff demonstrated in this codebase:

* Event sourcing and CQRS (separate read/write stores)
* Entity Framework Core
* Rich(er than usual) domain models
* Integration (aka functional or component) testing
* Unit testing
* Running the API in docker and docker compose
* Running tests in docker and docker compose
* Using Pulumi to generate both Azure resources and to manage/deploy to a Kubernetes cluster

Some of the stuff I'd like to add to this codebase:

* Authentication (probably using Identity Server)
* Automated build/deploy of both infrastructure and code using Azure DevOps and/or GitHub actions
* Model more of what happens at a practice than just scheduling appointments (i.e. implement a microservices architecture)
* Create a couple of different user interfaces (a public facing one and an internal one, in different stacks e.g. React, Blazor)

## Local Development

This project targets .NET 5.0. At the time of writing, you'll need the [latest SDK](https://dotnet.microsoft.com/download/dotnet/5.0) 
installed (I'm using preview7) and you'll need to use [Visual Studio Preview](https://visualstudio.microsoft.com/vs/preview/).

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

This application can be deployed to a Kubernetes cluster hosted in Azure by running `pulumi up` after cd'ing to `deploy/PetDoctor.InfrastructureStack`. This takes care of:

* Provisioning shared Azure infrastructure
* Provisioning Azure services dedicated to the appointments API
* Deploying things to Kubernetes (both shared concerns and application-specific, with ingress/service/deployment/secrets/etc)

Steps to get this to work properly:

1. Open a terminal
2. cd to `PetDoctor.Infrastructure` 
3. Change config as appropriate (see `Pulumi.yaml` and `Pulumi.dev.yaml`)
4. Run `pulumi up`
5. If it falls over, running `pulumi up` again typically works
6. Run `kubectl --namespace ingress-nginx get services -o wide -w ingress-nginx-controller` and note down the value of `EXTERNAL-IP`
7. Create an A record pointing the domain specified in your `Pulumi.dev.yaml` file (in the `PetDoctor.Infrastructure` directory) to the IP address obtained in the previous step
8. Give DNS a some time to propagate
9. Browse to `<the domain you provided>/api` or `<the domain you provided>/api/swagger` - if the page loads, you're good

To tear the resources down run:

`pulumi destroy`
