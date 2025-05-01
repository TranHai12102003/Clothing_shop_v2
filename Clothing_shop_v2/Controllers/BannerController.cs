using System.Threading.Tasks;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly ClothingShopDbContext _context;
        private readonly Cloudinary _cloudinary;
        public BannerController(IBannerService bannerService, Cloudinary cloudinary, ClothingShopDbContext context)
        {
            _bannerService = bannerService;
            _cloudinary = cloudinary;
            _context = context;
        }
        public async Task<ActionResult<PaginationModel<BannerGetVModel>>> Index([FromQuery] BannerFilterParams parameters)
        {
            var response = await _bannerService.GetAll(parameters);
            return View(response.Value);
        }
        public async Task<ActionResult<BannerGetVModel>> Details(int id)
        {
            var response = await _bannerService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            return View(response.Value);
        }

        private async Task LoadViewBagData()
        {
            ViewBag.Categories = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).ToListAsync();
            ViewBag.Products = await _context.Products.Select(p => new { p.Id, p.ProductName }).ToListAsync();
            ViewBag.Promotions = await _context.Promotions.Select(p => new { p.Id, p.PromotionName }).ToListAsync();
        }

        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerCreateVModel vmodel)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi ModelState để debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                await LoadViewBagData();
                return View(vmodel);
            }

            string imageUrl = string.Empty;
            if (vmodel.ImageFile != null && vmodel.ImageFile.Length > 0)
            {
                // Kiểm tra kích thước file
                if (vmodel.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh vượt quá kích thước tối đa 5MB.");
                    await LoadViewBagData();
                    return View(vmodel);
                }

                // Kiểm tra định dạng file
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(vmodel.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Ảnh không hợp lệ. Chỉ cho phép ảnh thuộc: .jpg, .jpeg, .png, .gif");
                    await LoadViewBagData();
                    return View(vmodel);
                }

                // Upload ảnh lên Cloudinary
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(vmodel.ImageFile.FileName, vmodel.ImageFile.OpenReadStream()),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                    Folder = "ClothingShop/Banner",
                    UseFilename = true,
                    UniqueFilename = false,
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError("ImageFile", "Đã xảy ra lỗi khi upload ảnh: " + uploadResult.Error.Message);
                    await LoadViewBagData();
                    return View(vmodel);
                }
                imageUrl = uploadResult.SecureUrl.ToString();
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Vui lòng chọn một ảnh.");
                await LoadViewBagData();
                return View(vmodel);
            }

            var response = await _bannerService.Create(vmodel, imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Thêm mới thành công";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Thêm banner thất bại.";
            await LoadViewBagData();
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, bool isActive = false)
        {
            var response = await _bannerService.ToggleActive(id, isActive);
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
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var response = await _bannerService.GetById(id);
            if (response.Value == null)
            {
                return NotFound();
            }
            await LoadViewBagData();
            return View(response.Value);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BannerUpdateVModel vmodel)
        {
            if (id != vmodel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);
                await LoadViewBagData();
                return View(vmodel);
            }

            var existingBannerResponse = await _bannerService.GetById(id);
            if (existingBannerResponse.Value == null)
            {
                return NotFound();
            }

            string imageUrl = existingBannerResponse.Value.ImageUrl; // Giữ URL ảnh hiện tại nếu không có ảnh mới
            if (vmodel.ImageFile != null && vmodel.ImageFile.Length > 0)
            {
                // Kiểm tra kích thước file
                if (vmodel.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Ảnh vượt quá kích thước tối đa 5MB.");
                    await LoadViewBagData();
                    return View(vmodel);
                }

                // Kiểm tra định dạng file
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(vmodel.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Ảnh không hợp lệ. Chỉ cho phép ảnh thuộc: .jpg, .jpeg, .png, .gif");
                    await LoadViewBagData();
                    return View(vmodel);
                }

                // Upload ảnh mới lên Cloudinary
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(vmodel.ImageFile.FileName, vmodel.ImageFile.OpenReadStream()),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                    Folder = "ClothingShop/Banner",
                    UseFilename = true,
                    UniqueFilename = false,
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError("ImageFile", "Đã xảy ra lỗi khi upload ảnh: " + uploadResult.Error.Message);
                    await LoadViewBagData();
                    return View(vmodel);
                }
                imageUrl = uploadResult.SecureUrl.ToString();

                // Xóa ảnh cũ trên Cloudinary nếu có
                if (!string.IsNullOrEmpty(existingBannerResponse.Value.ImageUrl))
                {
                    var uri = new Uri(existingBannerResponse.Value.ImageUrl);
                    var publicId = Path.GetFileNameWithoutExtension(uri.Segments.Last());
                    var folder = "clothingshop/banner";
                    var fullPublicId = $"{folder}/{publicId}";

                    var deletionParams = new DeletionParams(fullPublicId);
                    await _cloudinary.DestroyAsync(deletionParams);
                }
            }

            // Cập nhật thông tin banner
            var response = await _bannerService.Update(vmodel, imageUrl);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Cập nhật banner thành công.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Cập nhật banner thất bại.";
            await LoadViewBagData();
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _bannerService.Delete(id);
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
