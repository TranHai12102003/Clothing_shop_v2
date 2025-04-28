using System.ComponentModel.DataAnnotations;
using Clothing_shop_v2.Common.Constants;

namespace Clothing_shop_v2.VModels
{
    public class PromotionCreateVmodel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khuyến mãi.")]
        public string PromotionName { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập phần trăm khuyến mãi.")]
        public int PercentDiscount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá trị khuyến mãi.")]
        public decimal DiscountValue { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu khuyến mãi.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc khuyến mãi.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập trạng thái khuyến mãi.")]
        public bool IsActive { get; set; }

    }
    public class PromotionUpdateVmodel : PromotionCreateVmodel
    {
        public int Id { get; set; }
    }
    public class PromotionGetVmodel : PromotionUpdateVmodel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    //public class PromotionListViewModel
    //{
    //    public IEnumerable<PromotionGetVmodel> Promotions { get; set; }
    //    public int PageNumber { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages { get; set; }
    //    public int TotalItems { get; set; }
    //    public string? SearchString { get; set; }
    //}
    public class PromotionFilterParams
    {
        public string? SearchString { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}
