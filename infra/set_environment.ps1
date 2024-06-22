Connect-AzAccount -TenantId d474231d-df87-44eb-ae2e-5f23fae84e4a
$ct = Get-AzContext
$env:TF_VAR_ARM_SUBSCRIPTION_ID=$ct.Subscription.Id
$env:TF_VAR_ARM_TENANT_ID=$ct.Tenant.Id
