using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels
{
    public class ColorCreateVModel
    {
        public string ColorName { get; set; } = null!;
        public string ColorCode { get; set; } = null!;

    }
    public class ColorUpdateVModel : ColorCreateVModel
    {
        public int Id { get; set; }
        public DateTime UpdatetedDate { get; set; }
    }
    public class ColorGetVModel : ColorUpdateVModel
    {
    }
    public class ColorListViewModel
    {
        public IEnumerable<Color> Colors { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? SearchString { get; set; }
    }
}
