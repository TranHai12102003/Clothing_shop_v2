using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface ICategoryService
    {
        Task<ActionResult<PaginationModel<CategoryGetVModel>>> GetAll(CategoryFilterParams parameters);
        Task<ActionResult<CategoryGetVModel>> GetById(int id);
        Task<ResponseResult> Create(CategoryCreateVModel model, string image);
        Task<ResponseResult> Update(CategoryUpdateVModel model,string image);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> ToggleActive(int id, bool isActive = false);

    }
}
