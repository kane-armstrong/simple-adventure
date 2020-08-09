# Infrastructure/Software Deployment

Infrastructure is provisioned in Azure using Pulumi, and applications are deployed by Pulumi to Kubernetes. There are currently a few problems standing in the way of this being as reliable (and functional) as I'd like:

* `pulumi destroy` seems to get stuck on deleting the AAD pod identity resources; the only way to destroy the stack from here is to manually delete the resource group and delete and recreate the stack in Pulumi
* `pulumi up` provisions resources correctly but configuring DNS needs to be done manually. Is this worth automating? (I'm leaning toward no, given portability concerns)
* Certificates aren't working properly