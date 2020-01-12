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
        $mvcDisplayName = "${registeredAppDisplayName}MVC"
        $apiDisplayName = "${registeredAppDisplayName}Api"

        Write-Host "Registering MVC App"  -ForegroundColor Yellow
        $aadRegisterAppMvc = az ad app create --display-name $mvcDisplayName --reply-urls $redirectUri

        Write-Host "Registering Api App"  -ForegroundColor Yellow
        $aadRegisterAppApi = az ad app create --display-name $apiDisplayName --reply-urls $redirectUri

        $appInfoMvc = $aadRegisterAppMvc | ConvertFrom-Json
        $appInfoApi = $aadRegisterAppApi | ConvertFrom-Json

        $tenantId = az account get-access-token --query tenant --output tsv
        

        #Print Instructions for the apps
        Write-Host "Setting up Environment after once this script is done" -ForegroundColor Gray
        Write-Host "1. Login to Azure portal" -ForegroundColor Green
        Write-Host "2. Go to ${resourceGroupName}sqlserver"  -ForegroundColor Green
        Write-Host "3. Under Security>firewalls add client IP"  -ForegroundColor Green
        Write-Host "4. Go to Azure Active Directory (Default should have registered apps)"  -ForegroundColor Green
        Write-Host "5. Under Manage > App Registration you should find $mvcDisplayName and $apiDisplayName"  -ForegroundColor Green
        Write-Host "6. For both go to API permissions and add permissions Application.Read.All and IdentityProvider.Read.All"  -ForegroundColor Green
        Write-Host "6.1. When you login with AAD later and get error you might have to come back to the apps and add your user as owner for project"  -ForegroundColor Green
        
        #Identity Server        
        Write-Host "IdentityServer Setup" -ForegroundColor Gray
        $pathToJson = "..\IdentityServer\IdentityServerLockr\appsettings.json"
        $identityServerSettings = Get-Content $pathToJson -Raw | ConvertFrom-Json
        $identityServerSettings.ClientIdMVC = $appInfoMvc.appId
        $identityServerSettings.ClientIdApi = $appInfoApi.appId
        $identityServerSettings.TenantId = $tenantId
        $identityServerSettings | ConvertTo-Json | set-content $pathToJson

        Write-Host "7. Open Identity Server solution" -ForegroundColor Green
        Write-Host "8. For local debugging, righ click on project>properties and go to debug" -ForegroundColor Green
        Write-Host "9. set app url to http://localhost:5000/ and disable SSL" -ForegroundColor Green
        Write-Host "10. Check that the appsettings.json for Identity server contains:" -ForegroundColor Green
        Write-Host "10.1. ClientIdMvc : "$appInfoMvc.appId -ForegroundColor Yellow
        Write-Host "10.2. ClientIdApi : "$appInfoApi.appId -ForegroundColor Yellow
        Write-Host "10.3. TenantId : "$tenantId -ForegroundColor Yellow

        $sqlCon = "Server=tcp:${resourceGroupName}sqlserver.database.windows.net,1433;Initial Catalog=LockrDb;Persist Security Info=False;User ID=sqlserveruser;Password=Sql@server12user;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
        #MVC App setup
        Write-Host "MVC App Setup" -ForegroundColor Gray
        $pathToMvcJson = "..\LockrFront\LockrFront\appsettings.json"
        $mvcSettings = Get-Content $pathToMvcJson -Raw | ConvertFrom-Json
        $mvcSettings.ClientId = $appInfoMvc.appId
        $mvcSettings.ClientIdApi = $appInfoApi.appId
        $mvcSettings.DbConnectionString = $sqlCon
        $mvcSettings | ConvertTo-Json | set-content $pathToMvcJson

        Write-Host "11. Open LockrFront solution" -ForegroundColor Green
        Write-Host "12. For local debugging, righ click on project>properties and go to debug" -ForegroundColor Green
        Write-Host "13. set app url to http://localhost:5001/ and disable SSL" -ForegroundColor Green
        Write-Host "14. Check that the appsettings.json for LockrFront (mvc) contains:" -ForegroundColor Green
        Write-Host "14.1. ClientId : "$appInfoMvc.appId -ForegroundColor Yellow
        Write-Host "14.2. ClientIdApi : "$appInfoApi.appId -ForegroundColor Yellow
        Write-Host "14.3. DbConnectionString : "$sqlCon -ForegroundColor Yellow

        #API App setup
        Write-Host "Api App Setup" -ForegroundColor Gray
        $pathToApiJson = "..\LockrApi\LockrApi\appsettings.json"
        $apiSettings = Get-Content $pathToApiJson -Raw | ConvertFrom-Json
        $apiSettings.ClientId = $appInfoApi.appId
        $apiSettings.DbConnectionString = $sqlCon
        $apiSettings | ConvertTo-Json | set-content $pathToApiJson

        Write-Host "15. Open LockrApi solution" -ForegroundColor Green
        Write-Host "16. For local debugging, righ click on project>properties and go to debug" -ForegroundColor Green
        Write-Host "17. set app url to http://localhost:5003 and disable SSL" -ForegroundColor Green
        Write-Host "18. Check that the appsettings.json for LockrApi contains:" -ForegroundColor Green
        Write-Host "18.1. ClientId : "$appInfoApi.appId -ForegroundColor Yellow
        Write-Host "18.2. DbConnectionString : "$sqlCon -ForegroundColor Yellow

        Write-Host "19. Run the apps" -ForegroundColor Green
        Write-Host "20. If you want to check api call with post man, for now you will need the bearer token as API keys do not work yet though it can be generated. To get Bearer token, do following: " -ForegroundColor Gray
        Write-Host "20.1. Open the project DiscoverAccessToken, and replace the ClientId value with "$appInfoApi.appId -ForegroundColor Green
        Write-Host "20.2. Run the project, copy the token displayed and use in Postman as Bearer token." -ForegroundColor Green

        Write-Host "If the above appsettings were not updated, please use values below to do so manually" -ForegroundColor Gray
        Write-Host "Make Note of the MVC Application (Client) ID: "$appInfoMvc.appId -ForegroundColor Yellow
        Write-Host "Make Note of the Api Application (Client) ID: "$appInfoApi.appId -ForegroundColor Yellow
        Write-Host "Make Note of Directory (Tenant) ID: $tenantId"  -ForegroundColor Yellow
        Write-Host "DbConnectionString : "$sqlCon -ForegroundColor Yellow
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