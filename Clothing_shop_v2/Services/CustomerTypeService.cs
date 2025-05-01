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
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ClothingShopDbContext _context;
        public CustomerTypeService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(CustomerTypeCreateVModel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = CustomerTypeMapping.VModelToModel(vmodel);
                _context.CustomerTypes.Add(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thêm loại khách hàng mới thành công");
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
                var entity = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy loại khách hàng");
                }
                _context.CustomerTypes.Remove(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Xóa loại khách hàng thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<CustomerTypeGetVModel>>> GetAll(CustomerTypeFilterParams parameters)
        {
            IQueryable<CustomerType> query = _context.CustomerTypes.Where(BuildQueryable(parameters));

            var customerTypes = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => CustomerTypeMapping.ModelToVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<CustomerTypeGetVModel>
            {
                Records = customerTypes,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<CustomerTypeGetVModel>> GetById(int id)
        {
            var customerType = await _context.CustomerTypes
                .FirstOrDefaultAsync(x => x.Id == id);
            if (customerType == null)
            {
                return null;
            }
            return CustomerTypeMapping.ModelToVModel(customerType);
        }

        public async Task<ResponseResult> Update(CustomerTypeUpdateVModel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.CustomerTypes.FirstOrDefaultAsync(x => x.Id == vmodel.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy loại khách hàng");
                }
                entity = CustomerTypeMapping.VModelToModel(vmodel, entity);
                _context.CustomerTypes.Update(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Cập nhật loại khách hàng thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
        private Expression<Func<CustomerType, bool>> BuildQueryable(CustomerTypeFilterParams fParams)
        {
            return x =>
                (fParams.SearchString == null || (x.TypeName != null && x.TypeName.Contains(fParams.SearchString)));
        }
    }
}
