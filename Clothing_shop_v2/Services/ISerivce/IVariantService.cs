using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IVariantService
    {
        Task<ActionResult<PaginationModel<VariantGetVModel>>> GetAll(VariantFilterParams parameters);
        Task<ActionResult<VariantGetVModel>> GetById(int id);
        Task<ResponseResult> Create(VariantCreateVModel model);
        Task<ResponseResult> Update(VariantUpdateVModel model);
        Task<ResponseResult> Delete(int id);
    }
}
