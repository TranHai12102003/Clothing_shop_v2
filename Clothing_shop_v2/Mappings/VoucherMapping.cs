using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class VoucherMapping
    {
        public static VoucherGetVModel EntityToVModel(Voucher model)
        {
            var voucher = new VoucherGetVModel
            {
                Id = model.Id,
                VoucherCode = model.VoucherCode,
                Description = model.Description,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                MinOrderAmount = model.MinOrderAmount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MaxUsage = model.MaxUsage,
                UsedCount = model.UsedCount,
                IsActive = model.IsActive,
                CustomerTypeId = model.CustomerTypeId,
                CreatedDate = model.CreatedDate,
                UpdatedDate = model.UpdatedDate
            };
            var customerType = model?.CustomerType;
            if(customerType != null)
            {
                voucher.CustomerType = new IdNameVModel
                {
                    Id = customerType.Id,
                    Name = customerType.TypeName
                };
            }
            return voucher;
        }
        public static Voucher VModelToEntity(VoucherCreateVModel model)
        {
            return new Voucher
            {
                VoucherCode = model.VoucherCode,
                Description = model.Description,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                MinOrderAmount = model.MinOrderAmount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                MaxUsage = model.MaxUsage,
                UsedCount = model.UsedCount,
                IsActive = model.IsActive,
                CustomerTypeId = model.CustomerTypeId,
                CreatedDate = DateTime.Now,
            };
        }
        public static Voucher VModelToEntity(VoucherUpdateVModel vmodel, Voucher model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.VoucherCode = vmodel.VoucherCode;
            model.Description = vmodel.Description;
            model.DiscountType = vmodel.DiscountType;
            model.DiscountValue = vmodel.DiscountValue;
            model.MinOrderAmount = vmodel.MinOrderAmount;
            model.StartDate = vmodel.StartDate;
            model.EndDate = vmodel.EndDate;
            model.MaxUsage = vmodel.MaxUsage;
            model.UsedCount = vmodel.UsedCount;
            model.IsActive = vmodel.IsActive;
            model.CustomerTypeId = vmodel.CustomerTypeId;
            model.UpdatedDate = DateTime.Now;
            return model;
        }
    }
}
