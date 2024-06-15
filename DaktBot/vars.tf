variable "resource_group_prefix" {
    type = string
    description = "The unique name of the resource group to create"
}

variable "resource_group_postfix" {
    type = string
    default = "main"
    description = "The shared postfix applied to all created resource groups"
}

variable "resource_group_location" {
    type = string
    default = "westus3"
}


