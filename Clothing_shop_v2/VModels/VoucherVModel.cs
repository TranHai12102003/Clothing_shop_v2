using Clothing_shop_v2.Common.Constants;
using Clothing_shop_v2.Common.Contansts;

namespace Clothing_shop_v2.VModels
{
    public class VoucherCreateVModel
    {
        public string VoucherCode { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string DiscountType { get; set; } = null!;

        public decimal DiscountValue { get; set; }

        public decimal? MinOrderAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? MaxUsage { get; set; }

        public int UsedCount { get; set; }

        public bool IsActive { get; set; }

        public int? CustomerTypeId { get; set; }
    }
    public class VoucherUpdateVModel: VoucherCreateVModel
    {
        public int Id { get; set; }
    }
    public class VoucherGetVModel : VoucherUpdateVModel
    {
        public IdNameVModel CustomerType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class VoucherFilterParams
    {
        public string? VoucherCode { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}
