using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class VariantMapping
    {
        public static VariantGetVModel EntityGetVModel(Variant variant)
        {
            return new VariantGetVModel
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                SizeId = variant.SizeId,
                ColorId = variant.ColorId,
                Price = variant.Price,
                SalePrice = variant.SalePrice,
                QuantityInStock = variant.QuantityInStock,
                CreatedDate = variant.CreatedDate,
                UpdatedDate = variant.UpdatedDate
            };
        }
        public static Variant VModelToEntity(VariantCreateVModel variantVModel)
        {
            return new Variant
            {
                ProductId = variantVModel.ProductId,
                SizeId = variantVModel.SizeId,
                ColorId = variantVModel.ColorId,
                Price = variantVModel.Price,
                SalePrice = variantVModel.SalePrice,
                QuantityInStock = variantVModel.QuantityInStock,
                CreatedDate = DateTime.Now
            };
        }
        public static Variant VModelToEntity(VariantUpdateVModel variantVModel, Variant existingVariant)
        {
            if (existingVariant == null)
            {
                throw new ArgumentNullException(nameof(existingVariant));
            }
            existingVariant.ProductId = variantVModel.ProductId;
            existingVariant.SizeId = variantVModel.SizeId;
            existingVariant.ColorId = variantVModel.ColorId;
            existingVariant.Price = variantVModel.Price;
            existingVariant.SalePrice = variantVModel.SalePrice;
            existingVariant.QuantityInStock = variantVModel.QuantityInStock;
            existingVariant.UpdatedDate = DateTime.Now;
            return existingVariant;
        }
    }
}
