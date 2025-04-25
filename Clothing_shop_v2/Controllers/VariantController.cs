using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Controllers
{
    public class VariantController : Controller
    {
        private readonly ILogger<VariantController> _logger;
        private readonly ClothingShopDbContext _context;
        public VariantController(ILogger<VariantController> logger, ClothingShopDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddVariant([FromForm] VariantCreateVModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }

            // Ràng buộc: Giá khuyến mãi phải nhỏ hơn giá gốc (nếu có giá khuyến mãi)
            if (model.SalePrice.HasValue && model.SalePrice >= model.Price)
            {
                return Json(new { success = false, message = "Giá khuyến mãi phải nhỏ hơn giá gốc." });
            }
            if(model.QuantityInStock < 0)
            {
                return Json(new { success = false, message = "Số lượng tồn không được âm." });
            }

            // Kiểm tra xem đã tồn tại biến thể với SizeId và ColorId này chưa
            var existingVariant = await _context.Variants
                .FirstOrDefaultAsync(v => v.ProductId == model.ProductId && v.SizeId == model.SizeId && v.ColorId == model.ColorId);
            if (existingVariant != null)
            {
                return Json(new { success = false, message = "Biến thể với kích thước và màu sắc này đã tồn tại." });
            }

            // Ánh xạ từ ViewModel sang Entity
            var variant = VariantMapping.VModelToEntity(model);
            _context.Variants.Add(variant);
            await _context.SaveChangesAsync();

            // Ánh xạ Entity sang ViewModel để trả về
            var variantGetVModel = VariantMapping.EntityGetVModel(variant);
            return Json(new
            {
                success = true,
                message = "Thêm biến thể thành công!",
                variant = new
                {
                    id = variantGetVModel.Id,
                    sizeId = variantGetVModel.SizeId,
                    colorId = variantGetVModel.ColorId,
                    price = variantGetVModel.Price,
                    salePrice = variantGetVModel.SalePrice,
                    quantityInStock = variantGetVModel.QuantityInStock
                }
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVariant([FromForm] VariantUpdateVModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }

            var variant = await _context.Variants.FindAsync(model.Id);
            if (variant == null)
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể." });
            }

            // Kiểm tra xem đã tồn tại biến thể khác với SizeId và ColorId này chưa
            var existingVariant = await _context.Variants
                .FirstOrDefaultAsync(v => v.ProductId == model.ProductId && v.SizeId == model.SizeId && v.ColorId == model.ColorId && v.Id != model.Id);
            if (existingVariant != null)
            {
                return Json(new { success = false, message = "Biến thể với kích thước và màu sắc này đã tồn tại." });
            }

            // Ánh xạ từ ViewModel sang Entity
            VariantMapping.VModelToEntity(model, variant);
            _context.Variants.Update(variant);
            await _context.SaveChangesAsync();

            // Ánh xạ Entity sang ViewModel để trả về
            var variantGetVModel = VariantMapping.EntityGetVModel(variant);
            return Json(new
            {
                success = true,
                message = "Cập nhật biến thể thành công!",
                variant = new
                {
                    id = variantGetVModel.Id,
                    sizeId = variantGetVModel.SizeId,
                    colorId = variantGetVModel.ColorId,
                    price = variantGetVModel.Price,
                    salePrice = variantGetVModel.SalePrice,
                    quantityInStock = variantGetVModel.QuantityInStock
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.Variants.FindAsync(id);
            if (variant == null)
            {
                return Json(new { success = false, message = "Không tìm thấy biến thể." });
            }

            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa biến thể thành công!" });
        }
    }
}
