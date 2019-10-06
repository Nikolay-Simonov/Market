using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Market.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CartController : Controller
    {
        public async Task<ActionResult> Cart(string returnUrl) => View(returnUrl);
    }
}