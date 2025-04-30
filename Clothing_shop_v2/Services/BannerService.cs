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
    public class BannerService : IBannerService
    {
        private readonly ClothingShopDbContext _context;
        public BannerService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(BannerCreateVModel vmodel,string image)
        {
            var response = new ResponseResult();
            try
            {
                var entity = BannerMapping.VModelToModel(vmodel);
                if (image != null)
                {
                    entity.ImageUrl = image;
                }
                _context.Banners.Add(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thêm banner mới thành công");
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
                var entity = await _context.Banners.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy banner");
                }
                _context.Banners.Remove(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Xóa banner thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<BannerGetVModel>>> GetAll(BannerFilterParams parameters)
        {
            IQueryable<Banner> query = _context.Banners.Where(BuildQueryable(parameters));

            var promotions = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => BannerMapping.EntityToVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<BannerGetVModel>
            {
                Records = promotions,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<BannerGetVModel>> GetById(int id)
        {
            var banner = await _context.Banners
                .FirstOrDefaultAsync(x=>x.Id == id);
            if (banner == null)
            {
                return null;
            }
            return BannerMapping.EntityToVModel(banner);
        }

        public async Task<ResponseResult> Update(BannerUpdateVModel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Banners.FirstOrDefaultAsync(x => x.Id == vmodel.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy banner");
                }
                var savedEntity = BannerMapping.VModelToModel(vmodel, entity);
                _context.Banners.Update(savedEntity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Cập nhật banner thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        private Expression<Func<Banner, bool>> BuildQueryable(BannerFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.BannerName != null && x.BannerName.Contains(fParams.SearchString))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive);
        }
    }
}
