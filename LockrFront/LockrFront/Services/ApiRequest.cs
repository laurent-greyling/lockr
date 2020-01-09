using LockrFront.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LockrFront.Services
{
    public class ApiRequest : IApiRequest
    {
        public ITokenProvider _tokenProvider;
        public IConfiguration _config;

        public ApiRequest(
            ITokenProvider tokenProvider,
            IConfiguration config)
        {
            _tokenProvider = tokenProvider;
            _config = config;
        }

        public async Task<string> GetAsync(string controller)
        {
            var request = await RequestSetup();
            var response = await request.Item1.GetAsync($"{request.Item2}{controller}");

            return await response.Content.ReadAsStringAsync();
        }

        public async Task PostAsync(DomainModel model, string controller)
        {
            var request = await RequestSetup();
            await request.Item1.PostAsync($"{request.Item2}{controller}", model, new JsonMediaTypeFormatter());
        }

        /// <summary>
        /// Setup the client and get the base url for use in the requests to the api
        /// </summary>
        /// <returns></returns>
        private async Task<Tuple<HttpClient, string>> RequestSetup()
        {
            var baseUri = _config.GetValue<string>("ApiRequestUri");
            var accessToken = await _tokenProvider.RequestAcccessTokenAsync();

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return new Tuple<HttpClient, string>(client, baseUri);
        }
    }
}
