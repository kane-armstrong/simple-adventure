// Shared

variable location {
    default = "East US"
}

variable resource_group_name {
    default = "PetDoctor"
}

variable environment {
    default = "Development"
}

variable created_by {
    default = "Kane Armstrong"
}

// Container registry

variable registry_name {
    default = "kanearmstrongdev"
}

// AKS

variable "client_id" {}
variable "client_secret" {}

variable "agent_count" {
    default = 1
}

variable "ssh_public_key" {
    default = "~/.ssh/id_rsa.pub"
}

variable "dns_prefix" {
    default = "kanearmstrong-dev"
}

variable cluster_name {
    default = "kanearmstrong-dev"
}

variable log_analytics_workspace_name {
    default = "kanearmstrongDevTestLogAnalyticsWorkspaceName"
}

variable log_analytics_workspace_location {
    default = "eastus"
}

variable log_analytics_workspace_sku {
    default = "PerGB2018"
}

// Application Insights

variable app_insights_instance_name {
    default = "petdoctor-dev"
}

// KeyVault

variable vault_name {
    default = "petdoctor-dev"
}

variable tenant_id {
    default = "d862e1ae-d9d6-490a-87d4-ec497a618a1e"
}

variable managed_identity_tenant_id {
    default = "d862e1ae-d9d6-490a-87d4-ec497a618a1e"
}

variable managed_identity_object_id {
    default = "22e7e111-3757-4a53-95be-672c6bc6e21f"
}

// SQL Server

variable "administrator_login" {}
variable "administrator_login_password" {}

variable sql_server_instance_name {
    default = "petdoctor-dev"
}

variable database_name {
    default = "petdoctordb"
}