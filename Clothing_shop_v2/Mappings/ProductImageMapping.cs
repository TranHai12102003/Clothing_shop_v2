using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class ProductImageMapping
    {
        public static ProductImageGetVModel EntityToVModel(ProductImage productImage)
        {
            return new ProductImageGetVModel
            {
                Id = productImage.Id,
                ProductId = productImage.ProductId,
                VariantId = productImage.VariantId,
                ImageUrl = productImage.ImageUrl,
                IsPrimary = productImage.IsPrimary,
                CreatedDate = productImage.CreatedDate,
                UpdatedDate = productImage.UpdatedDate
            };
        }
        public static ProductImage VModelToEntity(ProductImageCreateVModel productImageVModel)
        {
            return new ProductImage
            {
                ProductId = productImageVModel.ProductId,
                VariantId = productImageVModel.VariantId,
                ImageUrl = productImageVModel.ImageUrl,
                IsPrimary = productImageVModel.IsPrimary,
                CreatedDate = DateTime.Now
            };
        }
        public static ProductImage VModelToEntity(ProductImageUpdateVModel productImageVModel, ProductImage existingProductImage)
        {
            if (existingProductImage == null)
            {
                throw new ArgumentNullException(nameof(existingProductImage));
            }
            existingProductImage.ProductId = productImageVModel.ProductId;
            existingProductImage.VariantId = productImageVModel.VariantId;
            existingProductImage.ImageUrl = productImageVModel.ImageUrl;
            existingProductImage.IsPrimary = productImageVModel.IsPrimary;
            existingProductImage.UpdatedDate = DateTime.Now;
            return existingProductImage;
        }
    }
}
