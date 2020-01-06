## Setup Identity Server and Login

For signin we preferably want to use `Azure Active Directory` to login as it allows us not to write our own authentication code to create secure user profiles. For this excersice we will not create new users, we assume you have an Azure Account with a `Default Active Directory` setup with yourself as an owner or already registered users. 

### 1. Run the `Initialize-AadApplication` script

The `Initialize-AadApplication` script will register your given app with Active Directory and display your `TenantId` and `AppId` on screen for use in `Identity Server` (`Tenant` or `ClientId` will also be needed for your WebApp). 

To run the script open `powershell` and navigate to ".\Setup". From here run `Import-Module .\New-Environment.psm1 -Force`

This will open up the script `Initialize-AadApplication` for execution. 

To execute the script run the following command:

``` 
Initialize-AAD -registeredAppDisplayName <DisplayName> -redirectUri <RedirectUri> 
```

- `registeredAppDisplayName` : Your display name for your registered app. This will be the name displayed in Azure Active Directory under `App registrations`
- `-redirectUri` : Redirect Uri for your application. Currently set to go back to IdentityServer and identity server will redirect to your registered app. (Can set this to `http://localhost:5000` while debugging). To this uri `/signin-aad` is added, as this is what identity server expect. If you change this value, you need to make sure that in identity server and this script it is the same, or go to the protal and under `App registrations\Authentication` update this value.

### 2. Setup Permissions for App in AAD

For now, you need to navigate to [Azure](https://portal.azure.com/) and set the permissions for your app at `App registration/Api permissions`.

- IdentityProvider.Read.All
- User.Read.All

### 3. Setup identity server to use AAD

In the `Identity Server` solution, change the configuration in the `appsettings.json` to be able to use AAD. Use the `TenantId` and `AppId` (`ClientID`) and substitute the values as below. 

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ClientId": "<Directory (tenant) ID>",
  "AppId": "<Your Application (client) ID>"
}
```

This should allow you to use `AAD` to login. You can also use the `TestUsers` Alice or Bob to login with local test users. 

### 4. Setup the MVC project

For the MVC solution we use the same `ClientId` as in Identity server. This can be setup in the `appsettings.json` file of `LockrFront` by substituting the clientid with your current clientid

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ClientId": "<Directory (tenant) ID>"
}
```