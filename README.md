## Setup Identity Server and Login

For signin we preferably want to use `Azure Active Directory` to login as it allows us not to write our own authentication code to create secure user profiles. For this excersice we will not create new users, we assume you have an Azure Account with a `Default Active Directory` setup with yourself as an owner or already registered users. 

### 1. Run the `Initialize-LockrEnvironment` script

The `Initialize-LockrEnvironment` script will:

- Create a resourcegroup 
- Create SQLServer called `{resourceGroupName}sqlserver` and a Database called `LockrDb`
    - __Please Note__ You need to go into Azure Portal to set the Firewall rules for SQL Server as well as change/reset default password for security
- Register your given app with Active Directory and display your `TenantId (tenant)` and `AppId (appId)` on screen for use in `Identity Server` (`AppId`, `ClientId`), and the `MVC app` (`ClientId`) in appsettings.

To run the script open `powershell` and navigate to ".\Setup". From here run `Import-Module .\New-Environment.psm1 -Force`

This will open up the script `Initialize-LockrEnvironment` for execution. 

To execute the script run the following command:

``` 
Initialize-LockrEnvironment -resourceGroupName <your resourcegroupname> -location <location> -registeredAppDisplayName <DisplayName> -redirectUri <RedirectUri>
```

- `resourceGroupName` : Name of your resource group
- `location` : location of your resources, can tab through a few preset ones
- `registeredAppDisplayName` : Your display name for your registered app. This will be the name displayed in Azure Active Directory under `App registrations`
- `-redirectUri` : Redirect Uri for your application. Currently set to go back to IdentityServer and identity server will redirect to your registered app. (Can set this to `http://localhost:5000` while debugging). To this uri `/signin-aad` is added, as this is what identity server expect. If you change this value, you need to make sure that in identity server and this script it is the same, or go to the protal and under `App registrations\Authentication` update this value.

### 2. Setup Permissions for App in AAD

For now, you need to navigate to [Azure](https://portal.azure.com/) and set the permissions for your app at `App registration/Api permissions`.

Graph:
`Azure Active Directory > AppRegistration > Your App > Api Permissions > Add Permissions > Microsoft Graph > Delegated permissions`
- Application.Read.All
- IdentityProvider.Read.All
Then once consent is prepared:
`Grant admin consent for Directory ` 

### 3. Setup identity server to use AAD

In the `Identity Server` solution, change the configuration in the `appsettings.json` to be able to use AAD. Use the `TenantId` (`ClientID`) and `AppId` and substitute the values as below. 

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

For the MVC solution we use the same `ClientId` as in Identity server. This can be setup in the `appsettings.json` file of `LockrFront` by substituting the clientid with your current clientid. Also use sql connection from portal for `DbConnectionString`

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
  "DbConnectionString": "<Sql Connection string>"
}
```

### 5. Run EntityFramework Migrations
Run `update-database –verbose` or `dotnet ef database update` to run Migrations and create your SQL tables.

Tables:
- ApiKeys

## MVC App (Front End)
Once you run the Identity server and the MVC app, you can signin with either the test users Alice or Bob, but AAD should be setup in such a way that you can use your AAD credentials to signin as well to this app.

Once you are signed in, you can:

- Generate and regenerate an `API Key` under the `Generate Api Key` tab.
  - Please note that this api key is hashed and saved into sql and if you navigate away from this page and did not copy the key you need to regenerate it as it will not be visible anymore.
  - Copy it and keep it safe. If you use it in an app or share with someone for use and forget what it was, you will have to regenerate the code break any app currently using this api key