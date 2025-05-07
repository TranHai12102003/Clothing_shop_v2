using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Mappings
{
    public static class ProductMapping
    {
        public static ProductGetVModel EntityToVModel(Product product)
        {
            var vmodel = new ProductGetVModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
            };
            if(product.Category != null)
            {
                vmodel.Category = CategoryMapping.EntityToVModel(product.Category);
            }
            if(product.ProductImages != null && product.ProductImages.Count > 0)
            {
                vmodel.PrimaryImageUrl = product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl
                         ?? product.ProductImages?.FirstOrDefault()?.ImageUrl;//Tìm ảnh chính nếu không có sẽ lấy ảnh đầu tiên tìm thấy
            }
            if (product.Variants != null && product.Variants.Count > 0)
            {
                vmodel.Price = product.Variants
                    .Where(v=>v.Price > 0)
                    .Min(v => v.Price);
            }
            else
            {
                vmodel.Price = 0;
            }
            return vmodel;
        }
        public static Product VModelToEntity(ProductCreateVModel productVModel)
        {
            return new Product
            {
                ProductName = productVModel.ProductName,
                Description = productVModel.Description,
                CategoryId = productVModel.CategoryId,
                CreatedDate = DateTime.Now,
            };
        }
        public static Product VModelToEntity(ProductUpdateVModel productVModel, Product existingProduct)
        {
            if (existingProduct == null)
            {
                throw new ArgumentNullException(nameof(existingProduct));
            }
            existingProduct.ProductName = productVModel.ProductName;
            existingProduct.Description = productVModel.Description;
            existingProduct.CategoryId = productVModel.CategoryId;
            existingProduct.UpdatedDate = DateTime.Now;
            return existingProduct;
        }
    }
}
