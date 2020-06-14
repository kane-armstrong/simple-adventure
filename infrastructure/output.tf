// Container Registry

output "container_registry_id" {
    value = "${azurerm_container_registry.acr.id}"
}

output "container_registry_login_server" {
    value = "${azurerm_container_registry.acr.login_server}"
}

// AKS

output "client_key" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.client_key}"
}

output "client_certificate" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.client_certificate}"
}

output "cluster_ca_certificate" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.cluster_ca_certificate}"
}

output "cluster_username" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.username}"
}

output "cluster_password" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.password}"
}

output "kube_config" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config_raw}"
}

output "host" {
    value = "${azurerm_kubernetes_cluster.aks.kube_config.0.host}"
}

// Application Insights

output "instrumentation_key" {
  value = "${azurerm_application_insights.appinsights.instrumentation_key}"
}

output "app_id" {
  value = "${azurerm_application_insights.appinsights.app_id}"
}

// KeyVault

output "vault_id" {
    value = "${azurerm_key_vault.keyvault.id}"
}

output "vault_uri" {
    value = "${azurerm_key_vault.keyvault.vault_uri}"
}

// SQL Server

output "sql_database_id" {
    value = "${azurerm_sql_database.sqlserver.id}"
}

output "sql_instance_id" {
    value = "${azurerm_sql_server.sqlserver.id}"
}

output "sql_instance_fqdn" {
    value = "${azurerm_sql_server.sqlserver.fully_qualified_domain_name}"
}
