namespace Clothing_shop_v2.VModels
{
    public class VariantCreateVModel
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
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
