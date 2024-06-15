variable "resource_group_prefix" {
    type = string
    description = "The unique name of the resource group to create"
}

variable "resource_group_postfix" {
    type = string
    default = "main"
    description = "The shared postfix applied to all created resource groups"
}

locals {
  resource_group_name = "${var.resource_group_prefix}_${var.resource_group_postfix}"
}

variable "resource_group_location" {
    type = string
    default = "eastus2"
}

variable "ARM_SUBSCRIPTION_ID" {
    type = string
}

variable "ARM_TENANT_ID" {
    type = string
}


