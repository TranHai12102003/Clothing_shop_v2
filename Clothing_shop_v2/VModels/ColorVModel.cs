using Clothing_shop_v2.Common.Constants;
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
    }
    public class ColorGetVModel : ColorUpdateVModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
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
    public class ColorFilterParams
    {
        public string? SearchString { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}
