using Clothing_shop_v2.Common.Constants;

namespace Clothing_shop_v2.VModels
{
    public class CustomerTypeCreateVModel
    {
        public string TypeName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? MinOrderCount { get; set; }
        public decimal? MinTotalAmount { get; set; }
    }
    public class CustomerTypeUpdateVModel : CustomerTypeCreateVModel
    {
        public int Id { get; set; }
    }
    public class CustomerTypeGetVModel : CustomerTypeUpdateVModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class CustomerTypeFilterParams
    {
        public string? SearchString { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}
