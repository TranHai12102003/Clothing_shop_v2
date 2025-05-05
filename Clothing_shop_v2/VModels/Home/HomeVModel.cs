using Clothing_shop_v2.Models;

namespace Clothing_shop_v2.VModels.Home
{
    public class HomeVModel
    {
        public List<ProductGetVModel> Products { get; set; }
        public List<BannerGetVModel> Banners { get; set; } = new List<BannerGetVModel>();
        //public List<PromotionGetVmodel> Promotions { get; set; } = new List<PromotionGetVmodel>();
        public List<CategoryGetVModel> Categories { get; set; }

    }
}
