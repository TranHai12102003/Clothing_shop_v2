using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IColorService
    {
        Task<ActionResult<PaginationModel<ColorGetVModel>>> GetAll(ColorFilterParams parameters);
        Task<ActionResult<ColorGetVModel>> GetById(int id);
        Task<ResponseResult> Create(ColorCreateVModel model);
        Task<ResponseResult> Update(ColorUpdateVModel model);
        Task<ResponseResult> Delete(int id);
    }
}
