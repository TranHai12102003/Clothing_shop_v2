using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shopapp.Mappings;
using Microsoft.EntityFrameworkCore;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.Services;

namespace Clothing_shop_v2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ClothingShopDbContext _context;
        private readonly ILogger<CategoryController> _logger;
        private readonly Cloudinary _cloudinary;
        private readonly ICategoryService _categoryService;
        public CategoryController(ClothingShopDbContext context, ILogger<CategoryController> logger, Cloudinary cloudinary, ICategoryService categoryService)
        {
            _context = context;
            _logger = logger;
            _cloudinary = cloudinary;
            _categoryService = categoryService;
        }
        //public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        //{
        //    // Tạo query cơ bản
        //    var query = _context.Categories
        //        .Include(c => c.ParentCategory)
        //        .AsQueryable();

        //    // Lọc theo từ khóa tìm kiếm nếu có
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        query = query.Where(c => c.CategoryName.Contains(searchString));
        //    }

        //    // Lấy tổng số danh mục sau khi lọc
        //    var totalItems = await query.CountAsync();

        //    // Tính toán số lượng trang
        //    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    // Đảm bảo pageNumber hợp lệ
        //    pageNumber = Math.Max(1, pageNumber);
        //    pageNumber = Math.Min(pageNumber, totalPages > 0 ? totalPages : 1);

        //    // Lấy danh sách danh mục theo trang
        //    var categories = await query
        //        .OrderBy(c => c.Id) // Sắp xếp theo Id (hoặc tiêu chí khác)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    // Tạo ViewModel
        //    var viewModel = new CategoryListViewModel
        //    {
        //        Categories = categories,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalPages = totalPages,
        //        TotalItems = totalItems,
        //        SearchString = searchString // Lưu từ khóa tìm kiếm
        //    };

        //    return View(viewModel);
        //}
        public async Task<ActionResult<PaginationModel<CategoryGetVModel>>> Index([FromQuery] CategoryFilterParams parameters)
        {
            var response = await _categoryService.GetAll(parameters);
            // Lấy tất cả danh mục để tra cứu danh mục cha
            var allCategories = await _context.Categories
                .Select(c => new { c.Id, c.CategoryName })
                .ToDictionaryAsync(c => c.Id, c => c.CategoryName);

            ViewBag.ParentCategoryNames = allCategories; // Dictionary<int, string>
            return View(response.Value);
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
        public async Task<IActionResult> Create(CategoryCreateVModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi ModelState để debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                return View(vmodel);
            }

            string imageUrl = string.Empty;
            if (vmodel.ImageFile != null && vmodel.ImageFile.Length > 0)
            {
                // Kiểm tra kích thước file
                if (vmodel.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh vượt quá kích thước tối đa 5MB.");
                    return View(vmodel);
                }

                // Kiểm tra định dạng file
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(vmodel.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Ảnh không hợp lệ. Chỉ cho phép ảnh thuộc: .jpg, .jpeg, .png, .gif");
                    return View(vmodel);
                }

                // Upload ảnh lên Cloudinary
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(vmodel.ImageFile.FileName, vmodel.ImageFile.OpenReadStream()),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                    Folder = "ClothingShop/Category",
                    UseFilename = true,
                    UniqueFilename = false,
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError("ImageFile", "Đã xảy ra lỗi khi upload ảnh: " + uploadResult.Error.Message);
                    return View(vmodel);
                }
                imageUrl = uploadResult.SecureUrl.ToString();
            }
            //else
            //{
            //    ModelState.AddModelError("ImageFile", "Vui lòng chọn một ảnh.");
            //    return View(vmodel);
            //}

            var response = await _categoryService.Create(vmodel, imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Thêm danh mục thất bại.";
            return View(vmodel);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryService.GetById(id);
            if (category.Value == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategories = new SelectList(_context.Categories, "Id", "CategoryName", category.Value.ParentCategoryId);
            return View(category.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CategoryUpdateVModel vModel)
        {
            if(id != vModel.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                // Log lỗi ModelState để debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                return View(vModel);
            }
            var existingCategory = await _categoryService.GetById(id);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }
            string imageUrl = existingCategory.Value.ImageUrl;// Gữi lại ảnh cũ
            if (vModel.ImageFile != null && vModel.ImageFile.Length > 0)
            {
                // Kiểm tra kích thước file
                if (vModel.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh vượt quá kích thước tối đa 5MB.");
                    return View(vModel);
                }

                // Kiểm tra định dạng file
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(vModel.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Ảnh không hợp lệ. Chỉ cho phép ảnh thuộc: .jpg, .jpeg, .png, .gif");
                    return View(vModel);
                }

                // Upload ảnh mới lên Cloudinary
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(vModel.ImageFile.FileName, vModel.ImageFile.OpenReadStream()),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                    Folder = "ClothingShop/Category",
                    UseFilename = true,
                    UniqueFilename = false,
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError("ImageFile", "Đã xảy ra lỗi khi upload ảnh: " + uploadResult.Error.Message);
                    return View(vModel);
                }
                imageUrl = uploadResult.SecureUrl.ToString();

                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingCategory.Value.ImageUrl))
                {
                    var uri = new Uri(existingCategory.Value.ImageUrl);
                    var publicId = Path.GetFileNameWithoutExtension(uri.Segments.Last());
                    var folder = "clothingshop/category";
                    var fullPublicId = $"{folder}/{publicId}";

                    var deletionParams = new DeletionParams(fullPublicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }
            }
            var response = await _categoryService.Update(vModel,imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Cập nhật danh mục thất bại.";
            return View(vModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryService.Delete(id);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive = false)
        {
            var response = await _categoryService.ToggleActive(id, isActive);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message; // Lưu thông báo thành công
            }
            else
            {
                TempData["ErrorMessage"] = response.Message; // Lưu thông báo lỗi
            }
            return RedirectToAction("Index");
        }
    }
}
