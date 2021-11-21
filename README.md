# Simple Event Sourced Application

[![CodeFactor](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure/badge)](https://www.codefactor.io/repository/github/kane-armstrong/simple-adventure)

This repository contains the source code for a simple toy project using CQRS and event sourcing.

Features:

* [x] Event sourcing and CQRS (separate read/write stores) using `MsSqlStreamStore` and Entity Framework Core
* [x] DDD
* [x] Code-first migrations
* [x] Integration tests
* [x] Unit tests
* [x] Running tests and tests in docker and docker compose
* [x] Using Pulumi for infrastructure as code, deploying to an Azure Kubernetes Service cluster
* [] Authentication (probably using Identity Server)
* [] Automated build/deploy of both infrastructure and code using Azure DevOps and/or GitHub actions
* [] Model more of what happens at a practice than just scheduling appointments (i.e. implement a microservices architecture)
* [] Better/more complete event sourcing implementation (projections)
* [] Create user interfaces

## Infrastructure & Deployment

**This is currently broken**

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
