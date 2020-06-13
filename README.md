# Simple Event Sourced Application

This repository contains the source code for a simple event sourced application. A simplified veterinary domain
is used to aid in breathing some life into the project.

The scope for this project is:

* Create an API-driven application for managing verterinary appointments for canine companions
* Use event sourcing as a persistence mechanism
* Provision a Kubernetes cluster in Azure
* Deploy the application to Kubernetes and access the application over the public internet
* Ensure adequate test coverage

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


### Database Migrations

This project uses code-first migrations. You'll need to install dotnet-ef:

```
dotnet tool update --global dotnet-ef
```

Example command to generate a migration:

```
dotnet ef migrations add InitialPetDoctorMigration -c PetDoctorContext -o Infrastructure/Migrations/PetDoctor/PetDoctorDb --startup-project src/PetDoctor.API
```
