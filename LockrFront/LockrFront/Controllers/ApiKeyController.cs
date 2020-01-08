using LockrFront.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LockrFront.Controllers
{
    public class ApiKeyController : Controller
    {
        public IApiKeyQueries _queryEntities;

        public ApiKeyController(IApiKeyQueries queryEntities) 
        {
            _queryEntities = queryEntities;
        }

        // GET: ApiKey
        public IActionResult Index()
        {
            var apiKeyModel = _queryEntities
                .RetrieveApiKey(User.Claims.FirstOrDefault(x => x.Type == "sub").Value);

            return View(apiKeyModel);
        }

        // POST: ApiKey/Create
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == "sub").Value;
                var apiKeyModel = _queryEntities.RetrieveApiKey(userId);

                if (string.IsNullOrEmpty(apiKeyModel.ApiKey))
                {
                    apiKeyModel.ApiKey = GenerateApiKey(userId);
                    apiKeyModel.Id = userId;
                    _queryEntities.SaveApiKeyAsync(apiKeyModel);
                }

                return View(nameof(Index), apiKeyModel);
            }
            catch
            {
                return View();
            }
        }

        // PUT: ApiKey/Update
        public IActionResult Update(IFormCollection collection)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == "sub").Value;
                var apiKeyModel = _queryEntities.RetrieveApiKey(userId);
                apiKeyModel.ApiKey = GenerateApiKey(userId);

                _queryEntities.UpdateApiKey(apiKeyModel);

                return View(nameof(Index), apiKeyModel);
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Generate a hashed key with userid as identifiable component in key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateApiKey(string userId)
        {
            var key = Guid.NewGuid().ToString();
            var hash = SHA256.Create();
            var hashByte = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashByte)
                sb.Append(b.ToString("X2"));

            return $"{sb}.{userId}";
        }
    }
}