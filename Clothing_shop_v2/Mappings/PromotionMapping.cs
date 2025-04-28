using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class PromotionMapping
    {
        public static PromotionGetVmodel EntityToVModel(Promotion promotion)
        {
            return new PromotionGetVmodel
            {
                Id = promotion.Id,
                PromotionName = promotion.PromotionName,
                PercentDiscount = promotion.PercentDiscount,
                DiscountValue = promotion.DiscountValue,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                CreatedDate = promotion.CreatedDate,
                UpdatedDate = promotion.UpdatedDate
            };
        }
        public static Promotion VModelToEntity(this PromotionCreateVmodel vModel)
        {
            return new Promotion
            {
                PromotionName = vModel.PromotionName,
                PercentDiscount = vModel.PercentDiscount,
                DiscountValue = vModel.DiscountValue,
                StartDate = vModel.StartDate,
                EndDate = vModel.EndDate,
                IsActive = true,
            };
        }
        public static Promotion VModelToEntity(PromotionUpdateVmodel vModel, Promotion promotion)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion));
            }
            promotion.PromotionName = vModel.PromotionName;
            promotion.PercentDiscount = vModel.PercentDiscount;
            promotion.DiscountValue = vModel.DiscountValue;
            promotion.StartDate = vModel.StartDate;
            promotion.EndDate = vModel.EndDate;
            promotion.IsActive = vModel.IsActive;
            promotion.UpdatedDate = DateTime.Now;
            return promotion;
        }
    }
}
