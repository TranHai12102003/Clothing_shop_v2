using System.ComponentModel.DataAnnotations;
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
    public class PromotionService : IPromotionService
    {
        private readonly ClothingShopDbContext _context;
        public PromotionService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(PromotionCreateVmodel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = PromotionMapping.VModelToEntity(vmodel);
                _context.Promotions.Add(entity);
                await _context.SaveChangesAsync();

                response = new SuccessResponseResult(entity, "Thêm khuyến mãi mơi thành công");
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
                var entity = await _context.Promotions.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy khuyến mãi");
                }
                _context.Promotions.Remove(entity);
                await _context.SaveChangesAsync();

                response = new SuccessResponseResult(entity, "Xóa khuyến mãi thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<PromotionGetVmodel>>> GetAll(PromotionFilterParams parameters)
        {
            IQueryable<Promotion> query = _context.Promotions.Where(BuildQueryable(parameters));

            var promotions = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => PromotionMapping.EntityToVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<PromotionGetVmodel>
            {
                Records = promotions,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ResponseResult> ToggleActive(int id, bool isActive = false)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Promotions.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy khuyến mãi");
                }
                entity.IsActive = isActive;
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thay đổi trạng thánh thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Update(PromotionUpdateVmodel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = _context.Promotions.FirstOrDefault(x => x.Id == vmodel.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy khuyến mãi");
                }
                entity = PromotionMapping.VModelToEntity(vmodel, entity);
                _context.Promotions.Update(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Cập nhật khuyến mãi thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private Expression<Func<Promotion, bool>> BuildQueryable(PromotionFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.PromotionName != null && x.PromotionName.Contains(fParams.SearchString))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive);
        }
    }
}
