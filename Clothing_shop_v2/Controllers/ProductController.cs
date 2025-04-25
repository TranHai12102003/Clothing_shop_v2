using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ClothingShopDbContext _context;
        private readonly IProductImageService _productImageService;
        public ProductController(ILogger<ProductController> logger, ClothingShopDbContext context, IProductImageService productImageService)
        {
            _logger = logger;
            _context = context;
            _productImageService = productImageService;
        }
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p=>p.ProductImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.ProductName.Contains(searchString));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = Math.Max(1, pageNumber);
            pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

            var products = await query
                .OrderBy(s => s.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var productGetVModel = products.Select(p => ProductMapping.EntityToVModel(p));

            var viewModel = new ProductListViewModel
            {
                Products = productGetVModel,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                SearchString = searchString
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }

            try
            {
                // Chuyển từ ViewModel sang Entity
                var product = ProductMapping.VModelToEntity(model);
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;

                // Lưu sản phẩm
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Xử lý hình ảnh (VariantId = null vì đây là ảnh của sản phẩm)
                if (model.imageFiles != null && model.imageFiles.Any())
                {
                    var imageResult = await _productImageService.AddImages(product.Id, model.imageFiles);
                    if (imageResult != "Thêm hình ảnh thành công.")
                    {
                        ModelState.AddModelError("imageFiles", imageResult);
                        ViewBag.Categories = _context.Categories.ToList();
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = $"Sản phẩm '{product.ProductName}' vừa được tạo.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi tạo sản phẩm: " + ex.Message);
                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }
            var productVModel = ProductMapping.EntityToVModel(product);

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(productVModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductUpdateVModel product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra Description không rỗng
                    if (string.IsNullOrWhiteSpace(product.Description))
                    {
                        ModelState.AddModelError("Description", "Mô tả không được để trống.");
                        ViewBag.Categories = await _context.Categories.ToListAsync();
                        return View(product);
                    }

                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                        return RedirectToAction("Index");
                    }

                    // Cập nhật các trường
                    existingProduct = ProductMapping.VModelToEntity(product, existingProduct);
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Sản phẩm '{product.ProductName}' đã được cập nhật.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm Id {Id}", id);
                    TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm.";
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Sản phẩm '{product.ProductName}' đã được xóa.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm Id {Id}", id);
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }
            // Lấy danh sách Sizes và Colors để sử dụng trong form thêm biến thể
            ViewBag.Sizes = await _context.Sizes
                .Select(s => new { s.Id, s.SizeName })
                .ToListAsync();
            ViewBag.Colors = await _context.Colors
                .Select(c => new { c.Id, c.ColorName })
                .ToListAsync();
            return View(product);
        }
    }
}
