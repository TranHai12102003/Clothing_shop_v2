using Microsoft.EntityFrameworkCore.ChangeTracking;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Shopapp.Mappings
{
    public static class CategoryMapping
    {
        public static CategoryGetVModel EntityToVModel (Category category)
        {
            return new CategoryGetVModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate,
                IsActive = category.IsActive,
                ImageUrl = category.ImageUrl
            };
        }
        public static Category VModelToEntity(CategoryCreateVModel categoryVModel)
        {
            return new Category
            {
                CategoryName = categoryVModel.CategoryName,
                ParentCategoryId = categoryVModel.ParentCategoryId,
                CreatedDate = DateTime.Now,
                UpdatedDate = null,
                IsActive = categoryVModel.IsActive,
                ImageUrl = categoryVModel.ImageUrl
            };
        }
        public static Category VModelToEntity(CategoryUpdateVModel categoryVModel)
        {
            return new Category
            {
                Id = categoryVModel.Id,
                CategoryName = categoryVModel.CategoryName,
                ParentCategoryId = categoryVModel.ParentCategoryId,
                CreatedDate = categoryVModel.CreatedDate,
                UpdatedDate = DateTime.Now,
                IsActive = categoryVModel.IsActive,
                ImageUrl = categoryVModel.ImageUrl
            };
        }
    }
}
