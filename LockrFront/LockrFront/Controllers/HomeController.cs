using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LockrFront.Models;
using Microsoft.AspNetCore.Authorization;
using LockrFront.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace LockrFront.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IApiRequest _request;

        public HomeController(IApiRequest request)
        {
            _request = request;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _request.GetAsync("Domain");

            var domainModelList = JsonConvert.DeserializeObject<List<DomainModel>>(response);

            //Use a tuple as the index model expect a tuple as we are using multiple items for the models in the index
            var tuple = new Tuple<DomainModel, List<DomainModel>>(null, domainModelList);
            return View(tuple);
        }

        /// <summary>
        /// Send data to the Lockr api to verify and save domain info
        /// The Bind Prefix is because we use a tuple in the Index cshtml
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VerifyDomain([Bind(Prefix = "Item1")]DomainModel model)
        {
            await _request.PostAsync(model, "Domain");

            var response = await _request.GetAsync("Domain");
            var domainModelList = JsonConvert.DeserializeObject<List<DomainModel>>(response);

            var tuple = new Tuple<DomainModel, List<DomainModel>>(null, domainModelList);
            return View(nameof(Index), tuple);
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "aad");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
