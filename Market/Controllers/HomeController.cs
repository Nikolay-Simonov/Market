using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}