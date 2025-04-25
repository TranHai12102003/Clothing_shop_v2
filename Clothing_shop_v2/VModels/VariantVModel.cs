using System.ComponentModel.DataAnnotations;

namespace Clothing_shop_v2.VModels
{
    public class VariantCreateVModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn kích thước.")]
        public int SizeId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn màu sắc.")]
        public int ColorId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn hoặc bằng 0.")]
        public decimal? SalePrice { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0.")]
        public int QuantityInStock { get; set; }
    }
    public class VariantUpdateVModel : VariantCreateVModel
    {
        public int Id { get; set; }
    }

    public class VariantGetVModel : VariantUpdateVModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }


    public class VariantListViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int? ProductId { get; set; } // Để lọc theo sản phẩm
    }
}
