using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
