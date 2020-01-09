using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace DiscoverAccessToken
{
    /// <summary>
    /// This is a small little helper project to get the Bearer token from Identity server.
    /// With this you can query the api from other apps, this can be used till api kry validation works (at time of writing api key was not working)
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");

            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "<Api Client Id>", //Client id for the api
                ClientSecret = "secret",
                Scope = "LockrApi" //Name given to the api, if you change it in api it needs to be changed here as well
            });

            Console.WriteLine(tokenResponse.AccessToken);
            Console.ReadKey();
        }
    }
}
