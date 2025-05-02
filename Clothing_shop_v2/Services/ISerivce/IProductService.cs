using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IProductService
    {
        Task<ActionResult<PaginationModel<ProductGetVModel>>> GetAll(ProductFilterParams parameters);
        Task<ActionResult<Product>> GetById(int id);
        Task<ResponseResult> Create(ProductCreateVModel product);
        Task<ResponseResult> Update(ProductUpdateVModel product);
        Task<ResponseResult> Delete(int id);
    }
}
