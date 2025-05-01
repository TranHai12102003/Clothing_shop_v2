using System.ComponentModel.DataAnnotations;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class RoleService : IRoleService
    {
        private readonly ClothingShopDbContext _context;
        public RoleService(ClothingShopDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<List<RoleGetVModel>>> GetAll()
        {
            var roles = await _context.Roles.ToListAsync();
            if (roles == null)
            {
                return null;
            }
            var roleVModels = roles.Select(x => RoleMapping.EntityToVModel(x)).ToList();
            return roleVModels;
        }

        public async Task<ActionResult<RoleGetVModel>> GetById(int id)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return null;
                }
                var roleVModel = RoleMapping.EntityToVModel(entity);
                return roleVModel;
            }
            catch (ValidationException ex)
            {
                return null;
            }
        }

        public async Task<ResponseResult> Create(RoleCreateVModel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = RoleMapping.VModelToEntity(vmodel);
                _context.Roles.Add(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Thêm quyền mới thành công");
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
                var entity = _context.Roles.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy quyền");
                }
                _context.Roles.Remove(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Xóa quyền thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }


        public async Task<ResponseResult> Update(RoleUpdateVModel vmodel)
        {
            var response = new ResponseResult();
            try
            {
                var entity = _context.Roles.FirstOrDefault(x => x.Id == vmodel.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy quyền");
                }
                entity.RoleName = vmodel.Name;
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Cập nhật quyền thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        } 
    }
}
