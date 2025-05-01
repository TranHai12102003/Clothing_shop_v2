using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface ICustomerTypeService
    {
        Task<ResponseResult> Create(CustomerTypeCreateVModel vmodel);
        Task<ResponseResult> Update(CustomerTypeUpdateVModel vmodel);
        Task<ResponseResult> Delete(int id);
        Task<ActionResult<PaginationModel<CustomerTypeGetVModel>>> GetAll(CustomerTypeFilterParams parameters);
        Task<ActionResult<CustomerTypeGetVModel>> GetById(int id);
    }
}
