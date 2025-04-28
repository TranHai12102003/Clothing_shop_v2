namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IProductImageService
    {
        Task<string> AddImages(int productId, List<IFormFile> imageFiles, int? variantId = null);
        Task<string> UpdateImage(int imageId, IFormFile? imageFile, bool? isPrimary);
        Task<string> DeleteImage(int imageId);
    }
}
