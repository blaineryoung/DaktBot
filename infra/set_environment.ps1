Connect-AzAccount
$ct = Get-AzContext
$env:TF_VAR_ARM_SUBSCRIPTION_ID=$ct.Subscription.Id
$env:TF_VAR_ARM_TENANT_ID=$ct.Tenant.Id
