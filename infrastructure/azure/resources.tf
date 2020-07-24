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

// SQL Database

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