using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class ProductMapping
    {
        public static ProductGetVModel EntityToVModel(Product product)
        {
            return new ProductGetVModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
            };
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
