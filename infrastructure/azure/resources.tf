// Container registry

resource "azurerm_resource_group" "acr" {
  name     = "${var.resource_group_name}"
  location = "${var.location}"
}

resource "azurerm_container_registry" "acr" {
  name                     = "${var.registry_name}"
  resource_group_name      = "${azurerm_resource_group.acr.name}"
  location                 = "${azurerm_resource_group.acr.location}"
  sku                      = "Standard"
  admin_enabled            = false
  
  tags = {
    Environment = "${var.environment}",
    CreatedBy = "${var.created_by}"
  }
}

// AKS

resource "azurerm_resource_group" "aks" {
    name     = "${var.resource_group_name}"
    location = "${var.location}"
}

resource "azurerm_log_analytics_workspace" "test" {
    name                = "${var.log_analytics_workspace_name}"
    location            = "${var.log_analytics_workspace_location}"
    resource_group_name = "${azurerm_resource_group.aks.name}"
    sku                 = "${var.log_analytics_workspace_sku}"
}

resource "azurerm_log_analytics_solution" "test" {
    solution_name         = "ContainerInsights"
    location              = "${azurerm_log_analytics_workspace.test.location}"
    resource_group_name   = "${azurerm_resource_group.aks.name}"
    workspace_resource_id = "${azurerm_log_analytics_workspace.test.id}"
    workspace_name        = "${azurerm_log_analytics_workspace.test.name}"

    plan {
        publisher = "Microsoft"
        product   = "OMSGallery/ContainerInsights"
    }
}

resource "azurerm_kubernetes_cluster" "aks" {
    name                = "${var.cluster_name}"
    location            = "${azurerm_resource_group.aks.location}"
    resource_group_name = "${azurerm_resource_group.aks.name}"
    dns_prefix          = "${var.dns_prefix}"

    linux_profile {
        admin_username = "ubuntu"

        ssh_key {
            key_data = "${file("${var.ssh_public_key}")}"
        }
    }

    agent_pool_profile {
        name            = "agentpool"
        count           = "${var.agent_count}"
        vm_size         = "Standard_DS1_v2"
        os_type         = "Linux"
        os_disk_size_gb = 30
    }

    service_principal {
        client_id     = "${var.client_id}"
        client_secret = "${var.client_secret}"
    }

    addon_profile {
        oms_agent {
        enabled                    = true
        log_analytics_workspace_id = "${azurerm_log_analytics_workspace.test.id}"
        }
    }

    tags = {
        Environment = "${var.environment}",
        CreatedBy = "${var.created_by}"
    }
}

// Application Insights

resource "azurerm_resource_group" "appinsights" {
  name     = "${var.resource_group_name}"
  location = "${var.location}"
}

resource "azurerm_application_insights" "appinsights" {
  name                = "${var.app_insights_instance_name}"
  location            = "${azurerm_resource_group.appinsights.location}"
  resource_group_name = "${azurerm_resource_group.appinsights.name}"
  application_type    = "Web"

  tags = {
    Environment = "${var.environment}",
    CreatedBy = "${var.created_by}"
  }
}

// KeyVault

resource "azurerm_resource_group" "keyvault" {
    name     = "${var.resource_group_name}"
    location = "${var.location}"
}

resource "azurerm_key_vault" "keyvault" {
  name                        = "${var.vault_name}"
  location                    = "${azurerm_resource_group.keyvault.location}"
  resource_group_name         = "${azurerm_resource_group.keyvault.name}"
  enabled_for_disk_encryption = true
  tenant_id                   = "${var.tenant_id}"

  sku_name = "standard"

  access_policy {
    tenant_id = "${var.managed_identity_tenant_id}"
    object_id = "${var.managed_identity_object_id}"

    secret_permissions = [
      "get",
      "list"
    ]

    key_permissions = [
      "wrapKey",
      "unwrapKey"
    ]
  }

  network_acls {
    default_action = "Allow"
    bypass         = "AzureServices"
  }
  
  tags = {
    Environment = "${var.environment}",
    CreatedBy = "${var.created_by}"
  }
}

// SQL Server

resource "azurerm_resource_group" "sqlserver" {
    name     = "${var.resource_group_name}"
    location = "${var.location}"
}

resource "azurerm_sql_server" "sqlserver" {
  name                         = "${var.sql_server_instance_name}"
  location                     = "${azurerm_resource_group.sqlserver.location}"
  resource_group_name          = "${azurerm_resource_group.sqlserver.name}"
  version                      = "12.0"
  administrator_login          = "${var.administrator_login}"
  administrator_login_password = "${var.administrator_login_password}"

  tags = {
    Environment = "Development",
    CreatedBy = "Kane Armstrong"
  }
}

resource "azurerm_sql_database" "sqlserver" {
  name                             = "${var.database_name}"
  location                         = "${azurerm_resource_group.sqlserver.location}"
  resource_group_name              = "${azurerm_resource_group.sqlserver.name}"
  server_name                      = "${azurerm_sql_server.sqlserver.name}"
  requested_service_objective_name = "S0"

  tags = {
    Environment = "${var.environment}",
    CreatedBy = "${var.created_by}"
  }
}