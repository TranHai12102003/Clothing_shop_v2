using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
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
        private readonly IProductService _productService;
        public ProductController(ILogger<ProductController> logger, ClothingShopDbContext context, IProductImageService productImageService, IProductService productService)
        {
            _logger = logger;
            _context = context;
            _productImageService = productImageService;
            _productService = productService;
        }
        #region
        //public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        //{
        //    var query = _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.ProductImages)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        query = query.Where(s => s.ProductName.Contains(searchString));
        //    }

        //    var totalItems = await query.CountAsync();
        //    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    pageNumber = Math.Max(1, pageNumber);
        //    pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

        //    var products = await query
        //        .OrderBy(s => s.Id)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //    var productGetVModel = products.Select(p => ProductMapping.EntityToVModel(p));

        //    var viewModel = new ProductListViewModel
        //    {
        //        Products = productGetVModel,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalPages = totalPages,
        //        TotalItems = totalItems,
        //        SearchString = searchString
        //    };

        //    return View(viewModel);
        //}
        #endregion
        public async Task<ActionResult<PaginationModel<ProductGetVModel>>> Index([FromQuery] ProductFilterParams parameters)
        {
            var response = await _productService.GetAll(parameters);
            return View(response.Value);
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
                var response = await _productService.Create(model);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", response.Message);
                    ViewBag.Categories = _context.Categories.ToList();
                    return View(model);
                }
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
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            var productVModel = ProductMapping.EntityToVModel(product);
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.ProductImages = product.ProductImages;

            return View(productVModel);
        }

        #region
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Update(int id, ProductUpdateVModel product, List<IFormFile>? imageFiles)
        //{
        //    if (id != product.Id)
        //    {
        //        return BadRequest();
        //    }

        //    try
        //    {
        //        // Kiểm tra các trường bắt buộc
        //        if (string.IsNullOrWhiteSpace(product.ProductName))
        //        {
        //            ModelState.AddModelError("ProductName", "Tên sản phẩm không được để trống.");
        //        }

        //        if (string.IsNullOrWhiteSpace(product.Description))
        //        {
        //            ModelState.AddModelError("Description", "Mô tả không được để trống.");
        //        }

        //        if (product.CategoryId <= 0 || !await _context.Categories.AnyAsync(c => c.Id == product.CategoryId))
        //        {
        //            ModelState.AddModelError("CategoryId", "Danh mục không hợp lệ.");
        //        }

        //        var existingProduct = await _context.Products
        //            .Include(p => p.ProductImages)
        //            .FirstOrDefaultAsync(p => p.Id == id);
        //        if (existingProduct == null)
        //        {
        //            TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
        //            return RedirectToAction("Index");
        //        }

        //        // Nếu ModelState không hợp lệ, hiển thị lại form
        //        if (!ModelState.IsValid)
        //        {
        //            ViewBag.Categories = await _context.Categories.ToListAsync();
        //            ViewBag.ProductImages = await _context.ProductImages
        //                .Where(pi => pi.ProductId == id && pi.VariantId == null)
        //                .ToListAsync();
        //            return View(product);
        //        }

        //        // Cập nhật thông tin sản phẩm
        //        existingProduct = ProductMapping.VModelToEntity(product, existingProduct);
        //        existingProduct.UpdatedDate = DateTime.Now;
        //        _context.Update(existingProduct);
        //        await _context.SaveChangesAsync();

        //        // Xử lý ảnh mới nếu có
        //        if (imageFiles != null && imageFiles.Any())
        //        {
        //            // Kiểm tra số lượng ảnh tối đa
        //            const int maxImages = 5;
        //            int currentImageCount = existingProduct.ProductImages?.Count ?? 0;
        //            int newImageCount = imageFiles.Count;
        //            if (currentImageCount + newImageCount > maxImages)
        //            {
        //                ModelState.AddModelError("", $"Sản phẩm chỉ được phép có tối đa {maxImages} ảnh.");
        //                ViewBag.Categories = await _context.Categories.ToListAsync();
        //                ViewBag.ProductImages = await _context.ProductImages
        //                    .Where(pi => pi.ProductId == id && pi.VariantId == null)
        //                    .ToListAsync();
        //                return View(product);
        //            }

        //            // Thêm ảnh mới
        //            var result = await _productImageService.AddImages(id, imageFiles, null);
        //            if (result != "Thêm hình ảnh thành công.")
        //            {
        //                ModelState.AddModelError("", result);
        //                ViewBag.Categories = await _context.Categories.ToListAsync();
        //                ViewBag.ProductImages = await _context.ProductImages
        //                    .Where(pi => pi.ProductId == id && pi.VariantId == null)
        //                    .ToListAsync();
        //                return View(product);
        //            }
        //        }

        //        TempData["SuccessMessage"] = $"Sản phẩm '{product.ProductName}' đã được cập nhật.";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm Id {Id}", id);
        //        TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm.";
        //        ViewBag.Categories = await _context.Categories.ToListAsync();
        //        ViewBag.ProductImages = await _context.ProductImages
        //            .Where(pi => pi.ProductId == id && pi.VariantId == null)
        //            .ToListAsync();
        //        return View(product);
        //    }
        //}
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductUpdateVModel product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            try
            {
                var response = await _productService.Update(product);
                if (response.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", response.Message);
                    ViewBag.Categories = await _context.Categories.ToListAsync();
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi cập nhật sản phẩm: " + ex.Message);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response.Message; // Lưu thông báo lỗi
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetById(id);
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
            return View(product.Value);
        }
    }
}
