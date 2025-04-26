using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class ProductCreateVModel
    {
        public string ProductName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public List<IFormFile>? imageFiles { get; set; }
        //public List<int> ColorIds { get; set; } = new List<int>();
        //public List<int> SizeIds { get; set; } = new List<int>();
    }
    public class ProductUpdateVModel : ProductCreateVModel
    {
        public int Id { get; set; }
    }
    public class ProductGetVModel : ProductUpdateVModel
    {
        public CategoryGetVModel Category { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string PrimaryImageUrl { get; set; } // URL của ảnh chính
    }
    public class ProductListViewModel
    {
        public IEnumerable<ProductGetVModel> Products { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }
}
