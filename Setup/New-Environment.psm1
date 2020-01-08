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
    #Are we already signed in or not, if not display to user what to do
    if(!$signedIn)
    {
        az login --use-device-code
        $signedIn = az account show
    }

    Return $signedIn
}

#Register app on AAD and display the tenantId and clientId for us to use in appsettings.json of our projects
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

        Write-Host "Make Note of Application (Client) ID: "$appInfo.appId -ForegroundColor Yellow

        $tenantId = az account get-access-token --query tenant --output tsv
        Write-Host "Make Note of Directory (Tenant) ID: $tenantId"  -ForegroundColor Yellow
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
        #This is easier as we want to create table as well
        #If want to specify own password and not admin one first, change script and pass param for password
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