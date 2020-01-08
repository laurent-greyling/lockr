using LockrFront.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
            //Only return the ID so we can know if object is available to disable or enable buttons
            //As we hash the api key, we do not want to retrieve the hash and display that, this is all for security 
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

                //Check if we already have a record for Api Key, if not create new
                if (string.IsNullOrEmpty(apiKeyModel.Id))
                {
                    apiKeyModel.ApiKey = $"{Guid.NewGuid().ToString()}.{userId}";
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
                apiKeyModel.ApiKey = $"{Guid.NewGuid().ToString()}.{userId}";

                _queryEntities.UpdateApiKey(apiKeyModel);

                return View(nameof(Index), apiKeyModel);
            }
            catch
            {
                return View();
            }
        }        
    }
}