using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IBannerService
    {
        Task<ActionResult<PaginationModel<BannerGetVModel>>> GetAll(BannerFilterParams parameters);
        Task<ActionResult<BannerGetVModel>> GetById(int id);
        Task<ResponseResult> Create(BannerCreateVModel vmodel, string image);
        Task<ResponseResult> Update(BannerUpdateVModel vmodel,string image);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> ToggleActive(int id, bool isActive = false);
    }
}
