using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CartController : Controller
    {
        public ActionResult Lines(string returnUrl) => View((object)returnUrl);
    }
}