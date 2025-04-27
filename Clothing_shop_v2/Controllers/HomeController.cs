using System.Diagnostics;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPizza_API_BackEnd.Service.IService;

namespace Clothing_shop_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClothingShopDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ClothingShopDbContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
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
                // Kiểm tra xem tên người dùng đã tồn tại chưa
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên người dùng hoặc email đã tồn tại.");
                    return View(user);
                }
                // Tạo người dùng mới
                var savedUser =RegisterMapping.Register(user);
                _context.Users.Add(savedUser);
                _context.SaveChanges();
                // Gửi email xác nhận
                var emailContent = $"Chào {user.FullName},\n\nCảm ơn bạn đã đăng ký tài khoản trên trang web của chúng tôi. Vui lòng xác nhận địa chỉ email của bạn để hoàn tất quá trình đăng ký.\n\nTrân trọng,\nĐội ngũ hỗ trợ khách hàng.";
                _emailService.SendEmailAsync(user.Email, "Xác nhận đăng ký tài khoản", emailContent);
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View(new LoginVModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password); // TODO: Use password hashing
                if (user != null)
                {
                    // TODO: Implement authentication (e.g., cookie or session)
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        public IActionResult Logout() {
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
