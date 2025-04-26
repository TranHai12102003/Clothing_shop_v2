using Clothing_shop_v2.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Shopapp.Mappings;
using Clothing_shop_v2.VModels;
using Clothing_shop_v2.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly ILogger<ProductImageController> _logger;
        private readonly ClothingShopDbContext _context;
        private readonly Cloudinary _cloudinary;
        public ProductImageController(ILogger<ProductImageController> logger, ClothingShopDbContext context, Cloudinary cloudinary)
        {
            _logger = logger;
            _context = context;
            _cloudinary = cloudinary;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddImages(int productId, List<IFormFile> imageFiles, int? variantId = null)
        {
            // Kiểm tra productId
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return BadRequest("Sản phẩm không tồn tại.");
            }

            // Kiểm tra variantId nếu có
            if (variantId.HasValue)
            {
                var variant = await _context.Variants.FindAsync(variantId);
                if (variant == null || variant.ProductId != productId)
                {
                    return BadRequest("Biến thể không tồn tại hoặc không thuộc sản phẩm này.");
                }
            }

            // Kiểm tra hình ảnh
            if (imageFiles == null || !imageFiles.Any())
            {
                return BadRequest("Vui lòng tải lên ít nhất một hình ảnh.");
            }

            // Validate kiểu file
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var file in imageFiles)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Định dạng file không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");
                }

                if (file.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                {
                    return BadRequest("Kích thước file không được vượt quá 5MB.");
                }
            }

            try
            {
                // Kiểm tra xem sản phẩm/biến thể đã có hình ảnh chính chưa
                bool hasPrimaryImage = await _context.ProductImages.AnyAsync(pi =>
                    pi.ProductId == productId &&
                    pi.VariantId == variantId &&
                    pi.IsPrimary);
                bool firstImage = !hasPrimaryImage;

                // Tải hình ảnh lên Cloudinary và lưu URL
                foreach (var imageFile in imageFiles)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                            Folder = "upload_clothingshop/products",
                            UseFilename = true,
                            UniqueFilename = false
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                        if (uploadResult.Error != null)
                        {
                            _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                            return BadRequest($"Cloudinary upload failed: {uploadResult.Error.Message}");
                        }

                        var imageUrl = uploadResult.SecureUrl.AbsoluteUri;

                        var productImage = new ProductImage
                        {
                            ProductId = productId,
                            VariantId = variantId, // Có thể là null (cho ảnh của sản phẩm) hoặc có giá trị (cho ảnh của biến thể)
                            ImageUrl = imageUrl,
                            IsPrimary = firstImage,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now
                        };

                        _context.ProductImages.Add(productImage);
                        firstImage = false;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Thêm hình ảnh thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding product images: {ErrorMessage}", ex.Message);
                return BadRequest("Đã có lỗi xảy ra: " + ex.Message);
            }
        }
        [HttpPost("DeleteImage")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var productImage = await _context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (productImage == null)
            {
                return Json(new { success = false, message = "Hình ảnh không tồn tại." });
            }

            try
            {
                if (!string.IsNullOrEmpty(productImage.ImageUrl))
                {
                    var uri = new Uri(productImage.ImageUrl);
                    var publicId = Path.GetFileNameWithoutExtension(uri.Segments.Last());
                    var folder = "upload_clothingshop/products";
                    var fullPublicId = $"{folder}/{publicId}";

                    var deletionParams = new DeletionParams(fullPublicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                    if (deletionResult.Error != null)
                    {
                        _logger.LogError("Cloudinary deletion error: {ErrorMessage}", deletionResult.Error.Message);
                        return Json(new { success = false, message = $"Cloudinary deletion failed: {deletionResult.Error.Message}" });
                    }
                }

                bool wasPrimary = productImage.IsPrimary;
                var productId = productImage.ProductId;
                var variantId = productImage.VariantId;

                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();

                if (wasPrimary)
                {
                    var remainingImages = await _context.ProductImages
                        .Where(pi => pi.ProductId == productId && pi.VariantId == variantId)
                        .ToListAsync();
                    if (remainingImages.Any())
                    {
                        var newPrimaryImage = remainingImages.First();
                        newPrimaryImage.IsPrimary = true;
                        newPrimaryImage.UpdatedDate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Xóa hình ảnh thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting product image: {ErrorMessage}", ex.Message);
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost("UpdateImage")]
        public async Task<IActionResult> UpdateImage(int imageId, bool isPrimary)
        {
            var productImage = await _context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (productImage == null)
            {
                return Json(new { success = false, message = "Hình ảnh không tồn tại." });
            }

            try
            {
                if (isPrimary)
                {
                    var otherImages = await _context.ProductImages
                        .Where(pi => pi.ProductId == productImage.ProductId &&
                                     pi.VariantId == productImage.VariantId &&
                                     pi.Id != imageId)
                        .ToListAsync();
                    foreach (var img in otherImages)
                    {
                        img.IsPrimary = false;
                        img.UpdatedDate = DateTime.Now;
                    }
                }
                productImage.IsPrimary = isPrimary;
                productImage.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật hình ảnh thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating product image: {ErrorMessage}", ex.Message);
                return Json(new { success = false, message = "Đã có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
