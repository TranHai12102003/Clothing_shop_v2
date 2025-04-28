using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
