using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shopapp.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ClothingShopDbContext _context;
        private readonly ILogger<CategoryController> _logger;
        private readonly Cloudinary _cloudinary;
        public CategoryController(ClothingShopDbContext context, ILogger<CategoryController> logger, Cloudinary cloudinary)
        {
            _context = context;
            _logger = logger;
            _cloudinary = cloudinary;
        }
        //public IActionResult Index()
        //{
        //    var categories = _context.Categories
        //        .Include(c => c.ParentCategory)
        //        .ToList();
        //    return View(categories);
        //}
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            // Tạo query cơ bản
            var query = _context.Categories
                .Include(c => c.ParentCategory)
                .AsQueryable();

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.CategoryName.Contains(searchString));
            }

            // Lấy tổng số danh mục sau khi lọc
            var totalItems = await query.CountAsync();

            // Tính toán số lượng trang
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Đảm bảo pageNumber hợp lệ
            pageNumber = Math.Max(1, pageNumber);
            pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

            // Lấy danh sách danh mục theo trang
            var categories = await query
                .OrderBy(c => c.Id) // Sắp xếp theo Id (hoặc tiêu chí khác)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tạo ViewModel
            var viewModel = new CategoryListViewModel
            {
                Categories = categories,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems,
                SearchString = searchString // Lưu từ khóa tìm kiếm
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var parentCategories = _context.Categories
                .Where(c => c.ParentCategoryId == null) // hoặc bỏ dòng này nếu muốn hiển thị tất cả
                .ToList();

            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "CategoryName");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVModel category, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = null;

                // Xử lý upload ảnh
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                        return BadRequest("Unsupported file type");

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                        Folder = "upload_clothingshop",
                        UseFilename = true,
                        UniqueFilename = false
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload failed: {uploadResult.Error.Message}");
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // Gán ảnh vào ViewModel
                category.ImageUrl = imageUrl;

                // Chuyển từ ViewModel sang Entity
                var categoryEntity = CategoryMapping.VModelToEntity(category);

                _context.Categories.Add(categoryEntity);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var parentCategories = _context.Categories
                .Where(c => c.Id != id)
                .ToList();
            ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "CategoryName");
            var categoryVModel = CategoryMapping.EntityToVModel(category);
            return View(categoryVModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CategoryUpdateVModel vModel, IFormFile ImageFile)
        {
            if (id != vModel.Id)
            {
                return NotFound();
            }

            // Bỏ validation cho ImageFile vì nó không bắt buộc
            ModelState.Remove("ImageFile");

            if (ModelState.IsValid)
            {
                // Giữ ImageUrl cũ
                string imageUrl = vModel.ImageUrl;

                // Xử lý upload ảnh mới nếu có
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Định dạng file không được hỗ trợ.");
                        var parentCategories = _context.Categories
                            .Where(c => c.Id != id)
                            .ToList();
                        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "CategoryName");
                        return View(vModel);
                    }

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                        Folder = "upload_clothingshop",
                        UseFilename = true,
                        UniqueFilename = false
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        ModelState.AddModelError("ImageFile", $"Tải ảnh thất bại: {uploadResult.Error.Message}");
                        var parentCategories = _context.Categories
                            .Where(c => c.Id != id)
                            .ToList();
                        ViewBag.ParentCategories = new SelectList(parentCategories, "Id", "CategoryName");
                        return View(vModel);
                    }

                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // Cập nhật ImageUrl và UpdatedDate vào ViewModel
                vModel.ImageUrl = imageUrl ?? vModel.ImageUrl; // Đảm bảo không ghi đè thành null
                vModel.UpdatedDate = DateTime.Now;

                // Chuyển từ ViewModel sang Entity
                var categoryEntity = CategoryMapping.VModelToEntity(vModel);

                _context.Categories.Update(categoryEntity);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Debug lỗi ModelState
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning("ModelState errors: {Errors}", string.Join(", ", errors));

            // Nếu ModelState không hợp lệ, trả lại danh sách danh mục cha
            var parentCategoriesError = _context.Categories
                .Where(c => c.Id != id)
                .ToList();
            ViewBag.ParentCategories = new SelectList(parentCategoriesError, "Id", "CategoryName");
            return View(vModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm danh mục theo id
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                // Nếu không tìm thấy danh mục, trả về NotFound hoặc thông báo lỗi
                TempData["ErrorMessage"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa danh mục
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                // Lưu thông báo thành công vào TempData
                TempData["SuccessMessage"] = $"Danh mục '{category.CategoryName}' đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có (ví dụ: danh mục đang được sử dụng)
                _logger.LogError(ex, "Lỗi khi xóa danh mục với Id {Id}", id);
                TempData["ErrorMessage"] = "Không thể xóa danh mục. Vui lòng thử lại hoặc kiểm tra xem danh mục có đang được sử dụng không.";
            }

            // Chuyển hướng về danh sách danh mục
            return RedirectToAction("Index");
        }
    }
}
