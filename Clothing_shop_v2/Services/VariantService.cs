using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class VariantService : IVariantService
    {
        private readonly ClothingShopDbContext _context;
        public VariantService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(VariantCreateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var variant = VariantMapping.VModelToEntity(model);
                await ValidateVariant(variant, null);
                await _context.Variants.AddAsync(variant);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(variant, "Thêm biến thể mới thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Delete(int id)
        {
            var response = new ResponseResult();
            try
            {
                var variant = _context.Variants.FirstOrDefault(x => x.Id == id);
                if (variant == null)
                {
                    return new ErrorResponseResult("Không tìm thấy biến thể");
                }
                _context.Variants.Remove(variant);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(variant, "Xóa biến thể thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<VariantGetVModel>>> GetAll(VariantFilterParams parameters)
        {
            IQueryable<Variant> query = _context.Variants.Where(BuildQueryable(parameters));

            var variants = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => VariantMapping.EntityGetVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<VariantGetVModel>
            {
                Records = variants,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<VariantGetVModel>> GetById(int id)
        {
            var variant = await _context.Variants
                .FirstOrDefaultAsync(x => x.Id == id);
            if (variant == null)
            {
                return null;
            }
            return VariantMapping.EntityGetVModel(variant);
        }

        public async Task<ResponseResult> Update(VariantUpdateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var variant = _context.Variants.FirstOrDefault(x => x.Id == model.Id);
                if (variant == null)
                {
                    return new ErrorResponseResult("Không tìm thấy biến thể");
                }
                variant = VariantMapping.VModelToEntity(model, variant);
                await ValidateVariant(variant, model.Id);
                _context.Variants.Update(variant);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(variant, "Cập nhật biến thể thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private async Task<Boolean> ValidateVariant(Variant savedVariant, int? id)
        {
            //validate for required fields
            if (savedVariant.ProductId <= 0 || String.IsNullOrEmpty(savedVariant.ProductId.ToString()))
            {
                throw new ValidationException("Mã sản phẩm không hợp lệ");
            }
            if (savedVariant.SizeId <= 0 || String.IsNullOrEmpty(savedVariant.SizeId.ToString()))
            {
                throw new ValidationException("Size không hợp lệ");
            }
            if (savedVariant.ColorId <= 0 || String.IsNullOrEmpty(savedVariant.ColorId.ToString()))
            {
                throw new ValidationException("Màu không hợp lệ");
            }
            if(savedVariant.Price <= 0 || String.IsNullOrEmpty(savedVariant.Price.ToString()))
            {
                throw new ValidationException("Giá không hợp lệ");
            }
            if (savedVariant.QuantityInStock < 0 || String.IsNullOrEmpty(savedVariant.QuantityInStock.ToString()))
            {
                throw new ValidationException("Số lượng không hợp lệ");
            }
            if(savedVariant.SalePrice < 0 || String.IsNullOrEmpty(savedVariant.SalePrice.ToString()))
            {
                throw new ValidationException("Giá khuyến mãi không hợp lệ");
            }
            if(savedVariant.SalePrice > savedVariant.Price)
            {
                throw new ValidationException("Giá khuyến mãi không được lớn hơn giá gốc");
            }
            return true;
        }

        private Expression<Func<Variant, bool>> BuildQueryable(VariantFilterParams fParams)
        {
            return x =>
                (fParams.ProductId == null || (x.ProductId == fParams.ProductId)) &&
                (fParams.SizeId == null || (x.SizeId == fParams.SizeId)) &&
                (fParams.ColorId == null || (x.ColorId == fParams.ColorId));

        }
    }
}
