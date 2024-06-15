terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~>3.78"
    }
  }
}

data "azurerm_client_config" "current" {}

provider "azurerm" {
  features {}

  subscription_id   = "${var.ARM_SUBSCRIPTION_ID}"
  tenant_id         = "${var.ARM_TENANT_ID}"
}

resource "azurerm_resource_group" "primary_resource_group" {
  name     = "${local.resource_group_name}"
  location = "${var.resource_group_location}"
}

resource "azurerm_user_assigned_identity" "service_identity" {
  resource_group_name = azurerm_resource_group.primary_resource_group.name
  location            = azurerm_resource_group.primary_resource_group.location
  name                = "${var.resource_group_prefix}-service-identity"
}