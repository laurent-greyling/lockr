using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LockrFront.Services
{
    public class TokenProvider : ITokenProvider
    {
        public IConfiguration _config;

        public TokenProvider(IConfiguration config) 
        {
            _config = config;
        }

        public async Task<string> RequestAcccessTokenAsync()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_config.GetValue<string>("Authority"));

            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _config.GetValue<string>("ClientIdApi"),
                ClientSecret = "secret",
                Scope = "LockrApi"
            });

            return tokenResponse.AccessToken;
        }
    }
}
