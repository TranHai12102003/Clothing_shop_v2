using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IVoucherService
    {
        Task<ActionResult<PaginationModel<VoucherGetVModel>>> GetAll(VoucherFilterParams parameters);
        Task<ActionResult<VoucherGetVModel>> GetById(int id);
        Task<ResponseResult> Create(VoucherCreateVModel model);
        Task<ResponseResult> Update(VoucherUpdateVModel model);
        Task<ResponseResult> Delete(int id);
        Task<ResponseResult> ChangeStatus(int id, bool isActive = false);
    }
}
