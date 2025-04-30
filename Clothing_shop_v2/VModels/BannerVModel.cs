using System.ComponentModel.DataAnnotations;
using Clothing_shop_v2.Common.Constants;

namespace Clothing_shop_v2.VModels
{
    public class BannerCreateVModel
    {
        [Required(ErrorMessage = "Tên banner là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên banner không được vượt quá 100 ký tự")]
        public string BannerName { get; set; } = null!;

        public string? ImageUrl { get; set; } // Không bắt buộc, sẽ được điền sau khi upload

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Đường dẫn là bắt buộc")]
        [Url(ErrorMessage = "Đường dẫn không hợp lệ")]
        public string LinkUrl { get; set; } = null!;

        public int? ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int? PromotionId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải là số không âm")]
        public int DisplayOrder { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn một ảnh")]
        public IFormFile ImageFile { get; set; }
    }
    public class BannerUpdateVModel : BannerCreateVModel
    {
        public int Id { get; set; }
    }
    public class BannerGetVModel : BannerUpdateVModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class BannerFilterParams
    {
        public string? SearchString { get; set; }
        public bool? IsActive { get; set; }
        public int PageSize { get; set; } = Numbers.Pagination.DefaultPageSize;
        public int PageNumber { get; set; } = Numbers.Pagination.DefaultPageNumber;
    }
}
