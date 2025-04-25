using Clothing_shop_v2.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly ClothingShopDbContext _context;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ProductImageService> _logger;

        public ProductImageService(ClothingShopDbContext context, Cloudinary cloudinary, ILogger<ProductImageService> logger)
        {
            _context = context;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<string> AddImages(int productId, List<IFormFile> imageFiles, int? variantId = null)
        {
            // Kiểm tra productId
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return "Sản phẩm không tồn tại.";
            }

            // Kiểm tra variantId nếu có
            if (variantId.HasValue)
            {
                var variant = await _context.Variants.FindAsync(variantId);
                if (variant == null || variant.ProductId != productId)
                {
                    return "Biến thể không tồn tại hoặc không thuộc sản phẩm này.";
                }
            }

            // Kiểm tra hình ảnh
            if (imageFiles == null || !imageFiles.Any())
            {
                return "Vui lòng tải lên ít nhất một hình ảnh.";
            }

            // Validate kiểu file
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var file in imageFiles)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return "Định dạng file không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.";
                }

                if (file.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                {
                    return "Kích thước file không được vượt quá 5MB.";
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
                            return $"Cloudinary upload failed: {uploadResult.Error.Message}";
                        }

                        var imageUrl = uploadResult.SecureUrl.AbsoluteUri;

                        var productImage = new ProductImage
                        {
                            ProductId = productId,
                            VariantId = variantId,
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
                return "Thêm hình ảnh thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding product images: {ErrorMessage}", ex.Message);
                return "Đã có lỗi xảy ra: " + ex.Message;
            }
        }

        public async Task<string> DeleteImage(int imageId)
        {
            // Kiểm tra imageId
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (productImage == null)
            {
                return "Hình ảnh không tồn tại.";
            }

            try
            {
                // Xóa ảnh trên Cloudinary (nếu cần)
                if (!string.IsNullOrEmpty(productImage.ImageUrl))
                {
                    // Lấy PublicId từ ImageUrl
                    var uri = new Uri(productImage.ImageUrl);
                    var publicId = Path.GetFileNameWithoutExtension(uri.Segments.Last());
                    var folder = "upload_clothingshop/products";
                    var fullPublicId = $"{folder}/{publicId}";

                    var deletionParams = new DeletionParams(fullPublicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                    if (deletionResult.Error != null)
                    {
                        _logger.LogError("Cloudinary deletion error: {ErrorMessage}", deletionResult.Error.Message);
                        return $"Cloudinary deletion failed: {deletionResult.Error.Message}";
                    }
                }

                // Kiểm tra xem ảnh bị xóa có phải là ảnh chính không
                bool wasPrimary = productImage.IsPrimary;
                var productId = productImage.ProductId;
                var variantId = productImage.VariantId;

                // Xóa ảnh khỏi database
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();

                // Nếu ảnh bị xóa là ảnh chính, chọn ảnh khác làm ảnh chính (nếu còn ảnh)
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

                return "Xóa hình ảnh thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting product image: {ErrorMessage}", ex.Message);
                return "Đã có lỗi xảy ra: " + ex.Message;
            }
        }
        

        public async Task<string> UpdateImage(int imageId, IFormFile? imageFile, bool? isPrimary)
        {
            // Kiểm tra imageId
            var productImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.Id == imageId);
            if (productImage == null)
            {
                return "Hình ảnh không tồn tại.";
            }

            try
            {
                // Nếu có file ảnh mới, upload lên Cloudinary và cập nhật ImageUrl
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Validate kiểu file
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return "Định dạng file không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.";
                    }

                    if (imageFile.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                    {
                        return "Kích thước file không được vượt quá 5MB.";
                    }

                    // Upload ảnh mới lên Cloudinary
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
                        return $"Cloudinary upload failed: {uploadResult.Error.Message}";
                    }

                    // Cập nhật ImageUrl
                    productImage.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }

                // Cập nhật trạng thái IsPrimary nếu có giá trị
                if (isPrimary.HasValue)
                {
                    if (isPrimary.Value)
                    {
                        // Đặt tất cả ảnh khác của cùng sản phẩm/biến thể về IsPrimary = false
                        var otherImages = await _context.ProductImages
                            .Where(pi => pi.ProductId == productImage.ProductId &&
                                         pi.VariantId == productImage.VariantId &&
                                         pi.Id != imageId)
                            .ToListAsync();
                        foreach (var img in otherImages)
                        {
                            img.IsPrimary = false;
                        }
                    }
                    productImage.IsPrimary = isPrimary.Value;
                }

                // Cập nhật thời gian
                productImage.UpdatedDate = DateTime.Now;

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                return "Cập nhật hình ảnh thành công.";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating product image: {ErrorMessage}", ex.Message);
                return "Đã có lỗi xảy ra: " + ex.Message;
            }
        }
    }
}
