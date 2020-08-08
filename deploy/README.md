# Infrastructure/Software Deployment

Infrastructure is provisioned in Azure using Pulumi, and applications are deployed by Pulumi to Kubernetes. There are currently a few problems standing in the way of this being as reliable (and functional) as I'd like:

* `pulumi destroy` seems to get stuck on deleting the AAD pod identity resources; the only way to destroy the stack from here is to manually delete the resource group and delete and recreate the stack in Pulumi
* `pulumi up` needs to be run twice as the nginx ingress deployment fails when trying to run the `ingress-nginx-admission-create` job
* Attempting to browse to `https://petdoctor.kanearmstrong.com/api` should take you to one of the appointments API pods, but it times out instead. It doesn't seem like the nginx ingress is working

I'm thinking of switching from nginx to something else, HA Proxy maybe. That might help with problems 2 and 3