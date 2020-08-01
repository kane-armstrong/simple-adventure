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