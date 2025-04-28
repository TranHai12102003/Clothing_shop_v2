using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IPromotionService
    {
        Task<ActionResult<PaginationModel<PromotionGetVmodel>>> GetAll(PromotionFilterParams parameters);
        Task<ResponseResult> Create(PromotionCreateVmodel vmodel);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> Update(PromotionUpdateVmodel vmodel);
        Task<ResponseResult> ToggleActive(int id, bool isActive = false);
    }
}
