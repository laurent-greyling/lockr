#This is to setup the Azure Active Directory for use
#If you forgot to make not of tenant and/or AppId, rerun this. Will give error that resource already exist, but will find and display needed info
function Initialize-AadApplication
{
    param(
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
        Write-Host "If you forget to make not of tenant and/or AppId, rerun this. Will give error that resource already exist, but will find and display needed info" -ForegroundColor Gray
        RegisterAppOnAad -registeredAppDisplayName $registeredAppDisplayName -redirectUri $redirectUri"/signin-aad"
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
            $redirectUri
        )

        $aadRegisterApp = az ad app create --display-name $registeredAppDisplayName --reply-urls $redirectUri

        $appInfo = $aadRegisterApp | ConvertFrom-Json

        Write-Host "Make Note of Application (Client) ID: "$appInfo.appId -ForegroundColor Yellow

        $tenantId = az account get-access-token --query tenant --output tsv
        Write-Host "Make Note of Directory (Tenant) ID: $tenantId"  -ForegroundColor Yellow
}

Export-ModuleMember Initialize-AadApplication