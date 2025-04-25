namespace Clothing_shop_v2.VModels
{
    public class ProductImageCreateVModel
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
    public class ProductImageUpdateVModel : ProductImageCreateVModel
    {
        public int Id { get; set; }
    }
    public class ProductImageGetVModel : ProductImageUpdateVModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
