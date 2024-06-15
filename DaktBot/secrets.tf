
resource "azurerm_key_vault" "app_secrets" {
  name                = "${var.resource_group_prefix}-secrets"
  location            = azurerm_resource_group.primary_resource_group.location
  resource_group_name = azurerm_resource_group.primary_resource_group.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "premium"
}

resource "azurerm_key_vault_access_policy" "creater_policy" {
  key_vault_id = azurerm_key_vault.app_secrets.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  key_permissions = [
    "List", "Create", "Delete", "Get", "Purge", "Recover", "Update", "GetRotationPolicy", "SetRotationPolicy"
  ]

  secret_permissions = [
    "List", "Delete", "Get", "Purge", "Recover", "Set"
  ]
}

resource "azurerm_key_vault_access_policy" "app_policy" {
  key_vault_id = azurerm_key_vault.app_secrets.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_user_assigned_identity.service_identity.principal_id

  key_permissions = [
    "List",  "Get"
  ]

  secret_permissions = [
    "List", "Get"
  ]
}

resource "azurerm_key_vault_secret" "discord_token" {
  name         = "Discord--Token"
  value        = "Insert token here"
  key_vault_id = azurerm_key_vault.app_secrets.id
}

resource "azurerm_key_vault_secret" "cosmosdb_connection_string" {
  name         = "CosmosDb--ConnectionString"
  value        = azurerm_cosmosdb_account.primary_storage.primary_sql_connection_string
  key_vault_id = azurerm_key_vault.app_secrets.id
}
