using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace IdentityServerLockr.Configuration
{
    public class Config
    {
        public static IConfiguration _config;

        public Config(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public IEnumerable<ApiResource> Apis => new List<ApiResource>
            {
                new ApiResource("api1", "My Api")
            };

        public IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = _config.GetValue<string>("ClientId"),
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    RequirePkce = true,
                    RequireClientSecret = false,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5001/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
    }
}
