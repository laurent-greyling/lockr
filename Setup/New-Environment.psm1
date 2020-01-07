#This will initialize the lockr environment
# 1. Create Resource Group
# 2. Register app on AAD
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
        CreateSqlServer -resourceGroupName $resourceGroupName -location $location

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

        $user = az ad user list --display-name "Greyling"
        $userInfo = $user | ConvertFrom-Json

        az ad app owner add --id $appInfo.appId --owner-object-id $userInfo.objectId

        Write-Host "Make not of appId, password (ClientSecret) and tenant (TenantId), these are needed for the appsettings.json" -ForegroundColor Yellow
        az ad app credential reset --id $appInfo.appId --credential-description "ClientSecret" --append   
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

#Create SQL if Not Exist
function CreateSqlServer
{
    param(
        [string]
        $resourceGroupName,
        [string]
        $location
    ) 
    
    $sqlServerName = "${resourceGroupName}sqlserver"
    Write-Host "Checking sql server $sqlServerName" -ForegroundColor Yellow

    $sqlServerExists = az sql server show `
    --name $sqlServerName `
    --resource-group $resourceGroupName

    if(!$sqlServerExists)
    {

        Write-Host "Creating sql server $sqlServerName" -ForegroundColor Yellow
        #The password can also be user specified with small script change. Just easier this way and changing afterwards
        Write-Host "Note: Reset admin password after creation to somwthing more secure and unknown" -ForegroundColor Red
        az sql server create `
        --admin-password "Sql@server12user" `
        --admin-user "sqlserveruser" `
        --name $sqlServerName `
        --resource-group $resourceGroupName

        az sql db create `
        -g $resourceGroupName `
        -s $sqlServerName `
        -n "LockrDb"
    }
}

Export-ModuleMember Initialize-LockrEnvironment