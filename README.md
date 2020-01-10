## Setup Identity Server and Login

For signin we preferably want to use `Azure Active Directory` to login as it allows us not to write our own authentication code to create secure user profiles. For this excersice we will not create new users, we assume you have an Azure Account with a `Default Active Directory` setup with yourself as an owner or already registered users. 

### 1. Run the `Initialize-LockrEnvironment` script

The `Initialize-LockrEnvironment` script will:

- Create a resourcegroup 
- Create SQLServer called `{resourceGroupName}sqlserver` and a Database called `LockrDb`
    - __Please Note__ You need to go into Azure Portal to set the Firewall rules for SQL Server as well as change/reset default password for security
- Register a MVC and API app with Active Directory and display your `TenantId (tenant)` and `AppId (appId)` on screen for use in `Identity Server` (`AppId`, `ClientId`), the `MVC app` (`ClientId for MVC`) and `Api App` (`ClientId for API`) in appsettings.

To run the script open `powershell` and navigate to ".\Setup". From here run `Import-Module .\New-Environment.psm1 -Force`

This will open up the script `Initialize-LockrEnvironment` for execution. 

To execute the script run the following command:

``` 
Initialize-LockrEnvironment -resourceGroupName <your resourcegroupname> -location <location> -registeredAppDisplayName <DisplayName> -redirectUri <RedirectUri>
```

- `resourceGroupName` : Name of your resource group
- `location` : location of your resources, can tab through a few preset ones
- `registeredAppDisplayName` : Your display name for your registered app. This will be the name displayed in Azure Active Directory under `App registrations` with `Api` and `MVC` appended to the end.
- `-redirectUri` : Redirect Uri for your application. Currently set to go back to IdentityServer and identity server will redirect to your registered app. (Can set this to `http://localhost:5000` while debugging). To this uri `/signin-aad` is added, as this is what identity server expect. If you change this value, you need to make sure that in identity server and this script it is the same, or go to the protal and under `App registrations\Authentication` update this value.

### 2. Setup Permissions for App in AAD

For now, you need to navigate to [Azure](https://portal.azure.com/) and set the permissions for your apps (Both apps need this setup) at `App registration/Api permissions`.

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
   "ClientIdMvc": "Application (Client) ID MVC",
  "ClientIdApi": "Application (Client) ID Api",
  "AppId": "<Your Application (client) ID>"
}
```

This should allow you to use `AAD` to login. You can also use the `TestUsers` Alice or Bob to login with local test users. 

### 4. Setup the MVC and Api projects

For the solutions we use the same `ClientIdMVC` for MVC project and `ClientIdApi` for Api project as in Identity server. This can be setup in the `appsettings.json` file of `LockrFront` and `LockrApi` by substituting the clientid with your current clientids. Also use sql connection from portal for `DbConnectionString`

MVC Settings
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
  "ClientIdApi": "<Application (Client) ID Api>",
  "ApiRequestUri": "<Api URI>",
  "DbConnectionString": "<Sql Connection string>"
}
```

API Settings
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

#### As a side note:
The above appsettings should preferable not be set in `appsettings.json` as this could be a potential security issue. Usually for development you will use the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) for Asp.net core and when app is published use something like `Azure Key Vault` to retrieve sensitive information.

### 5. Run EntityFramework Migrations

__Note__ make sure you set the firewall rules in the portal to allow your current IP.

Run `update-database –verbose` or `dotnet ef database update` to run Migrations and create your SQL tables.

Tables:
- ApiKeys

## MVC App (Front End)
Once you run the Identity server and the MVC app, you can signin with either the test users Alice or Bob, but AAD should be setup in such a way that you can use your AAD credentials to signin as well to this app.

Once you are signed in, you can:

- Generate and regenerate an `API Key` under the `Generate Api Key` tab.
  - Please note that this api key is hashed and saved into sql and if you navigate away from this page and did not copy the key you need to regenerate it as it will not be visible anymore.
  - Copy it and keep it safe. If you use it in an app or share with someone for use and forget what it was, you will have to regenerate the code break any app currently using this api key

- Send `Email address` or `domain` to Api. When on the home page you can enter an email address or domain name and press the validate button. This will send the information for validation to the api where it will be saved into the table for domains.
  - Once a domain is sent, if successfully checked and saved it will dispaly underneath the validation button in a table, this will show if domain is valid or not.

  ## API App
  The api will receive an `email address` or  `domain name` and check:

  1. Is it a valid email format
  2. Is it a valid domain format if it was not an email
  3. Validate if the spf record is valid or not

  Once this validation is done, a domain will be saved with status of `valid` or `invalid`, and return the saved records.

  ### To Run This App
  1. Do the setup as mentioned above
  2. Start the Identity Server
  3. Start the MVC app
  4. Start the API app
  5. Signin to the MVC App and send your domain to be checked or create api Key

  __Note__ at point of writing this, the generated api key will not allow access to the api yet. If you want access to api, either run the MVC app, or use the `Discover Access Token` console app to get the `Bearer` token and use that for now.

Although the MVC app can generate and regenerate an API key, my focus was more on `Access Token` authentication as I see this as a more secure route. With a `Bearer` token I can set a proper expiry time before it needs to be refreshed (not currently in this app), or kick an inactive user back to login screen. Users who also logout, will then expire the `Access Token`, unauthorizing the person who might be using the token in a malicious manner. This makes it more secure, even if you get hold of the token, you will have a limited time to execute any malicious intent. When it comes to highly personal information, I do not want a token access policy that could potentially see me logged in for life. Also, if an employee leaves a company and his/her access rights are revoked, he or she has no more access to the data.

With an API key, the opposite is true. Very few people put an expiry date/time on an API key, and if they do it is usually valid for a year. In this case, if you do get access to an api key for malicious intent, you have much longer window to access data. If the company does find out the api key was compromised, they can regenerate the key, having potential breaking changes for their customers who implemented this key. 