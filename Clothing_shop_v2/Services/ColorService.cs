using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Services
{
    public class ColorService : IColorService
    {
        private readonly ClothingShopDbContext _context;
        public ColorService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(ColorCreateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var color = ColorMapping.VModelToEntity(model);
                ValidateColor(color, null);
                _context.Colors.Add(color);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(color, "Thêm màu mới thành công");
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
                var color = _context.Colors.FirstOrDefault(x => x.Id == id);
                if (color == null)
                {
                    return new ErrorResponseResult("Không tìm thấy màu");
                }
                _context.Colors.Remove(color);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(color, "Xóa màu thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<ColorGetVModel>>> GetAll(ColorFilterParams parameters)
        {
            IQueryable<Color> query = _context.Colors
                .Where(BuildQueryable(parameters));
            var colors = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => ColorMapping.EntityToVModel(x))
                .ToListAsync();
            var totalItems = await query.CountAsync();
            return new PaginationModel<ColorGetVModel>
            {
                Records = colors,
                TotalRecords = totalItems,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<ColorGetVModel>> GetById(int id)
        {
            var color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (color == null)
            {
                return null;
            }
            var colorVModel = ColorMapping.EntityToVModel(color);
            return colorVModel;
        }

        public async Task<ResponseResult> Update(ColorUpdateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (color == null)
                {
                    return new ErrorResponseResult("Không tìm thấy màu");
                }
                color = ColorMapping.VModelToEntity(model,color);
                ValidateColor(color, model.Id);
                _context.Colors.Update(color);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(color, "Cập nhật màu thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private async Task<Boolean> ValidateColor(Color savedColor, int? id)
        {
            //validate for required fields
            if (savedColor.Id <= 0 || String.IsNullOrEmpty(savedColor.Id.ToString()))
            {
                throw new ValidationException("ID không hợp lệ");
            }
            if (String.IsNullOrEmpty(savedColor.ColorName.ToString()))
            {
                throw new ValidationException("Tên màu không hợp lệ");
            }
            if (String.IsNullOrEmpty(savedColor.ColorCode.ToString()))
            {
                throw new ValidationException("Mã màu không hợp lệ");
            }
            return true;
        }
        private Expression<Func<Color, bool>> BuildQueryable(ColorFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.ColorName != null && x.ColorName.Contains(fParams.SearchString)));

        }
    }
}
