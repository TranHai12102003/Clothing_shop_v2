using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly ClothingShopDbContext _context;
        public VoucherService(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseResult> ChangeStatus(int id, bool isActive = false)
        {
            var response = new ResponseResult();
            try
            {
                var entity = await _context.Vouchers.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy voucher");
                }
                entity.IsActive = isActive;
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thay đổi trạng thánh thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Create(VoucherCreateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var entity = VoucherMapping.VModelToEntity(model);
                await ValidateSavedVoucher(entity, null);
                _context.Vouchers.Add(entity);
                await _context.SaveChangesAsync();
                response = new SuccessResponseResult(entity, "Thêm voucher thành công");
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
                var entity = _context.Vouchers.FirstOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy voucher");
                }
                _context.Vouchers.Remove(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Xóa voucher thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ActionResult<PaginationModel<VoucherGetVModel>>> GetAll(VoucherFilterParams parameters)
        {
            IQueryable<Voucher> query = _context.Vouchers
                .Include(x => x.CustomerType)
                .Where(BuildQueryable(parameters));

            var vouchers = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => VoucherMapping.EntityToVModel(x))
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PaginationModel<VoucherGetVModel>
            {
                Records = vouchers,
                TotalRecords = totalRecords,
                PageSize = parameters.PageSize,
                CurrentPage = parameters.PageNumber
                // TotalPages tự tính nên không cần gán!
            };
        }

        public async Task<ActionResult<VoucherGetVModel>> GetById(int id)
        {
            var model = await _context.Vouchers.FirstOrDefaultAsync(x => x.Id == id);
            if (model == null)
            {
                return null;
            }
            var voucher = VoucherMapping.EntityToVModel(model);
            return voucher;
        }

        public async Task<ResponseResult> Update(VoucherUpdateVModel model)
        {
            var response = new ResponseResult();
            try
            {
                var entity = _context.Vouchers.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    return new ErrorResponseResult("Không tìm thấy voucher");
                }
                entity = VoucherMapping.VModelToEntity(model, entity);
                await ValidateSavedVoucher(entity, model.Id);
                _context.Vouchers.Update(entity);
                _context.SaveChanges();
                response = new SuccessResponseResult(entity, "Cập nhật voucher thành công");
                return response;
            }
            catch (ValidationException ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
        private async Task<Boolean> ValidateSavedVoucher(Voucher savedVoucher, long? id)
        {
            if (string.IsNullOrEmpty(savedVoucher.VoucherCode))
            {
                throw new ValidationException("Mã voucher là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.Description.ToString()))
            {
                throw new ValidationException("Mô tả là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.DiscountType.ToString()))
            {
                throw new ValidationException("Loại giảm giá là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.DiscountValue.ToString()))
            {
                throw new ValidationException("Giá trị voucher là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.EndDate.ToString()))
            {
                throw new ValidationException("End date is required");
            }
            if (string.IsNullOrEmpty(savedVoucher.StartDate.ToString()))
            {
                throw new ValidationException("Ngày bắt đầu là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.EndDate.ToString()))
            {
                throw new ValidationException("Ngày kết thúc là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.MaxUsage.ToString()))
            {
                throw new ValidationException("Số lần sử dụng tối đa là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.UsedCount.ToString()))
            {
                throw new ValidationException("Số lần đã sử dụng là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.IsActive.ToString()))
            {
                throw new ValidationException("Trạng thái là bắt buộc");
            }
            if (string.IsNullOrEmpty(savedVoucher.CustomerTypeId.ToString()))
            {
                throw new ValidationException("Loại khách hàng là bắt buộc");
            }
            return true;
        }
        private Expression<Func<Voucher, bool>> BuildQueryable(VoucherFilterParams fParams)
        {
            return x =>
                (fParams.VoucherCode == null || (x.VoucherCode != null && x.VoucherCode.Contains(fParams.VoucherCode))) &&
                (fParams.IsActive == null || x.IsActive == fParams.IsActive);
        }
    }
}
