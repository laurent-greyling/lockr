#This will initialize the lockr environment
# 1. Create Resource Group
# 2. Create Keyvault resources
# 3. Register app on AAD
function Initialize-LockrEnvironment
{
    param(
            [Parameter(Mandatory)]
            [string]
            $resourceGroupName,
            [Parameter(Mandatory)]
            [ValidateSet(
            "northeurope",
            "westeurope",
            "centralus",
            "eastus", 
            "westus")]
            [string]
            $location,
            [Parameter(Mandatory)]
            [string]
            $registeredAppDisplayName,
            [Parameter(Mandatory)]
            [string]
            $redirectUri
        ) 

    $signedIn = SignIn

    if($signedIn)
    {
        CreateResourceGroup -resourceGroupName $resourceGroupName -location $location

        Write-Host "If you forget to make not of tenant and/or AppId, rerun this. Will give error that resource already exist, but will find and display needed info" -ForegroundColor Gray
        RegisterAppOnAad -registeredAppDisplayName $registeredAppDisplayName -redirectUri $redirectUri"/signin-aad" -resourceGroupName $resourceGroupName
    }
}

#SignIn to Azure
function SignIn
{    
    $signedIn = az account show
    if(!$signedIn)
    {
        az login --use-device-code
        $signedIn = az account show
    }

    Return $signedIn
}

#Register app on AAD
function RegisterAppOnAad
{
        param(
            [string]
            $registeredAppDisplayName,
            [string]
            $redirectUri,
            [string]
            $resourceGroupName
        )

        Write-Host "Registering app with AAD"  -ForegroundColor Yellow
        $aadRegisterApp = az ad app create --display-name $registeredAppDisplayName --reply-urls $redirectUri        

        $appInfo = $aadRegisterApp | ConvertFrom-Json

        Write-Host "Make not of appId, password (ClientSecret) and tenant (TenantId), these are needed for the appsettings.json" -ForegroundColor Yellow
        az ad app credential reset --id $appInfo.appId --credential-description "ClientSecret" --append         
        
        CreateAzureKeyVault -resourceGroupName $resourceGroupName -location $location -objectId $appInfo.appId # $appInfo.objectId      
}

#Create Resource Group if Not Exist
function CreateResourceGroup
{
    param(
        [string]
        $resourceGroupName,
        [string]
        $location
        )

    Write-Host "Checking Resource Group $resourceGroupName" -ForegroundColor Yellow
    $groupExists = az group exists -n $resourceGroupName

    if($groupExists -eq $false)
    {
       Write-Host "Creating Resource Group $resourceGroupName" -ForegroundColor Green
       az group create `
       -n $resourceGroupName `
       -l $location
    }
}

#Create 2 keyvaults, one is for Lockr app secrets and the other to store clientAPIKeys
function CreateAzureKeyVault
{
    param(
        [string]
        $resourceGroupName,
        [string]
        $location,
        [string]
        $objectId
        )

        $keyvaultLokr = "${resourceGroupName}LockrKv"
        $keyvaultApiKey = "${resourceGroupName}ApiKeysKv"

        Write-Host "Check if ${keyvaultLokr} exists" -ForegroundColor Yellow
        $keyvaultLockrExists = az keyvault show --name $keyvaultLokr --resource-group $resourceGroupName

        if(!$keyvaultLockrExists)
        {
            Write-Host "Creating ${keyvaultLokr}" -ForegroundColor Green
            az keyvault create --name $keyvaultLokr --resource-group $resourceGroupName --location $location
            az keyvault set-policy --name $keyvaultLokr --spn $objectId --secret-permissions backup delete get list recover restore set
        }

        Write-Host "Check if ${keyvaultApiKey} exists" -ForegroundColor Yellow
        $keyvaultApiKeyExists = az keyvault show --name $keyvaultApiKey --resource-group $resourceGroupName

        if(!$keyvaultApiKeyExists)
        {
            Write-Host "Creating ${keyvaultApiKey}" -ForegroundColor Green
            az keyvault create --name $keyvaultApiKey --resource-group $resourceGroupName --location $location
            az keyvault set-policy --name $keyvaultApiKey --spn $objectId --secret-permissions backup delete get list recover restore set
        }
}

Export-ModuleMember Initialize-LockrEnvironment