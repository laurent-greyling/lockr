using Microsoft.AspNetCore.Mvc;

namespace LockrFront.Controllers
{
    //Controller to generate and save the apikey 
    public class ApiKeyController : Controller
    {
        public IActionResult Index()
        {
            //Logic to create, save and returnapi key
            return View();
        }
    }
}