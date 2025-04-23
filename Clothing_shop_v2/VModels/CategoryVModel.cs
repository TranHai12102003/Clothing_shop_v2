using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class CategoryCreateVModel
    {
        public string CategoryName { get; set; } = null!;

        public int? ParentCategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public string? ImageUrl { get; set; }
    }
    public class CategoryUpdateVModel : CategoryCreateVModel
    {
        public int Id { get; set; }
    }
    public class CategoryGetVModel : CategoryUpdateVModel
    {
    }
    public class CategoryListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }
}
