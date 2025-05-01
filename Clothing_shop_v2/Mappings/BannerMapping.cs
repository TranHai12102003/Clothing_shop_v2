using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class BannerMapping
    {
        public static BannerGetVModel EntityToVModel(Banner banner)
        {
            var vModel = new BannerGetVModel
            {
                Id = banner.Id,
                BannerName = banner.BannerName,
                ImageUrl = banner.ImageUrl,
                Description = banner.Description,
                LinkUrl = banner.LinkUrl,
                ProductId = banner.ProductId,
                CategoryId = banner.CategoryId,
                PromotionId = banner.PromotionId,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                IsActive = banner.IsActive,
                DisplayOrder = banner.DisplayOrder,
                CreatedDate = banner.CreatedDate,
                UpdatedDate = banner.UpdatedDate
            };
            var category = banner?.Category;
            if (category != null)
            {
                vModel.Category = new IdNameVModel
                {
                    Id = category.Id,
                    Name = category.CategoryName
                };
            }
            var product = banner?.Product;
            if (product != null)
            {
                vModel.Product = new IdNameVModel
                {
                    Id = product.Id,
                    Name = product.ProductName
                };
            }
            var promotion = banner?.Promotion;
            if (promotion != null)
            {
                vModel.Promotion = new IdNameVModel
                {
                    Id = promotion.Id,
                    Name = promotion.PromotionName
                };
            }
            return vModel;
        }
        public static Banner VModelToModel(BannerCreateVModel bannerCreateVModel)
        {
            return new Banner
            {
                BannerName = bannerCreateVModel.BannerName,
                ImageUrl = bannerCreateVModel.ImageUrl,
                Description = bannerCreateVModel.Description,
                LinkUrl = bannerCreateVModel.LinkUrl,
                ProductId = bannerCreateVModel.ProductId,
                CategoryId = bannerCreateVModel.CategoryId,
                PromotionId = bannerCreateVModel.PromotionId,
                StartDate = bannerCreateVModel.StartDate,
                EndDate = bannerCreateVModel.EndDate,
                IsActive = bannerCreateVModel.IsActive,
                DisplayOrder = bannerCreateVModel.DisplayOrder,
                CreatedDate = DateTime.Now
            };
        }
        public static Banner VModelToModel(BannerUpdateVModel bannerUpdateVModel, Banner model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            model.BannerName = bannerUpdateVModel.BannerName;
            model.ImageUrl = bannerUpdateVModel.ImageUrl;
            model.Description = bannerUpdateVModel.Description;
            model.LinkUrl = bannerUpdateVModel.LinkUrl;
            model.ProductId = bannerUpdateVModel.ProductId;
            model.CategoryId = bannerUpdateVModel.CategoryId;
            model.PromotionId = bannerUpdateVModel.PromotionId;
            model.StartDate = bannerUpdateVModel.StartDate;
            model.EndDate = bannerUpdateVModel.EndDate;
            model.IsActive = bannerUpdateVModel.IsActive;
            model.DisplayOrder = bannerUpdateVModel.DisplayOrder;
            model.UpdatedDate = DateTime.Now;
            return model;
        }
    }
}
