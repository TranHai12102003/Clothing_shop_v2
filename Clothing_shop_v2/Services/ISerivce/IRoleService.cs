using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IRoleService
    {
        Task<ActionResult<List<RoleGetVModel>>> GetAll();
        Task<ActionResult<RoleGetVModel>> GetById(int id);
        Task<ResponseResult> Create(RoleCreateVModel vmodel);
        Task<ResponseResult> Update(RoleUpdateVModel vmodel);
        Task<ResponseResult> Delete(int id);
    }
}
