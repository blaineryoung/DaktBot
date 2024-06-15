resource "azurerm_cosmosdb_account" "primary_storage" {
  name                  = "${var.resource_group_prefix}-storage"
  location              = azurerm_resource_group.primary_resource_group.location
  resource_group_name   = azurerm_resource_group.primary_resource_group.name
  default_identity_type = join("=", ["UserAssignedIdentity", azurerm_user_assigned_identity.service_identity.id])
  offer_type            = "Standard"
  kind                  = "GlobalDocumentDB"

  consistency_policy {
    consistency_level = "Strong"
  }

  geo_location {
    location          = "${var.resource_group_location}"
    failover_priority = 0
  }

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.service_identity.id]
  }
}
