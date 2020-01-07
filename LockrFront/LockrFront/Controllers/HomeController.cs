using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LockrFront.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace LockrFront.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {            
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var apiKey = string.Empty;

            var apiKeyModel = new ApiKeyModel 
            { 
                ApiKey = apiKey
            };

            return View(apiKeyModel);
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
