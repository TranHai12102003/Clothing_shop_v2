using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class SizeCreateVModel
    {
        public string SizeName { get; set; } = null!;
    }
    public class SizeUpdateVModel : SizeCreateVModel
    {
        public int Id { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
    public class SizeGetVModel : SizeUpdateVModel
    {
    }
    public class SizeListViewModel
    {
        public IEnumerable<Size> Sizes { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }
}
