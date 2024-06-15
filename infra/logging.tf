resource "azurerm_log_analytics_workspace" "service_monitoring_workspace" {
  name                = "${var.resource_group_prefix}-workspace"
  location            = azurerm_resource_group.primary_resource_group.location
  resource_group_name = azurerm_resource_group.primary_resource_group.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "service_monitoring_app_insights" {
  name                = "${var.resource_group_prefix}-appinsights"
  location            = azurerm_resource_group.primary_resource_group.location
  resource_group_name = azurerm_resource_group.primary_resource_group.name
  workspace_id        = azurerm_log_analytics_workspace.service_monitoring_workspace.id
  application_type    = "web"
}

output "instrumentation_key" {
  value = azurerm_application_insights.service_monitoring_app_insights.instrumentation_key
  sensitive = true
}
