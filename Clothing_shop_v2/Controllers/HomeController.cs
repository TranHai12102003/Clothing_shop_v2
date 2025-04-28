using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClothingShopDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, ClothingShopDbContext context, IEmailService emailService,IUserService userService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Products)
            .ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVModel user)
        {
            if (ModelState.IsValid)
            {
                //// Kiểm tra xem tên người dùng đã tồn tại chưa
                //var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);
                //if (existingUser != null)
                //{
                //    ModelState.AddModelError("", "Tên người dùng hoặc email đã tồn tại.");
                //    return View(user);
                //}
                //// Tạo người dùng mới
                //var savedUser =RegisterMapping.Register(user);
                //_context.Users.Add(savedUser);
                //_context.SaveChanges();
                //// Gửi email xác nhận
                //var emailContent = $"Chào {user.FullName},\n\nCảm ơn bạn đã đăng ký tài khoản trên trang web của chúng tôi. Vui lòng xác nhận địa chỉ email của bạn để hoàn tất quá trình đăng ký.\n\nTrân trọng,\nĐội ngũ hỗ trợ khách hàng.";
                //_emailService.SendEmailAsync(user.Email, "Xác nhận đăng ký tài khoản", emailContent);
                var result = _userService.RegisterUser(user);
                if (result.Result.Success)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", result.Result.Message);
                }
                //return RedirectToAction("Login");
            }
            return View(user);
        }

        // API kích hoạt tài khoản
        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount([FromQuery] string token)
        {
            var result = await _userService.ActivateAccount(token);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        public IActionResult Login()
        {
            return View(new LoginVModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model);
                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                if (result.Token != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserID.ToString()),
                        new Claim(ClaimTypes.Name, result.FullName),
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim(ClaimTypes.Role, result.Role) // Role: Admin, Customer...
                    };

                    var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyCookieAuth", principal);

                    // Điều hướng sau khi login
                    if (result.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Đây là cái scheme mà bạn đặt lúc SignInAsync
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccountManagement() 
        { 
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
