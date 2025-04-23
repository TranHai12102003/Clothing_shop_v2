using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class SizeMapping
    {
        public static SizeGetVModel EntityToVModel(Size size)
        {
            return new SizeGetVModel
            {
                Id = size.Id,
                SizeName = size.SizeName,
            };
        }
        public static Size VModelToEntity(SizeCreateVModel sizeVModel)
        {
            return new Size
            {
                SizeName = sizeVModel.SizeName,
                CreatedDate = DateTime.Now,
            };
        }
        public static Size VModelToEntity(SizeUpdateVModel sizeVModel, Size existingSize)
        {
            if (existingSize == null)
            {
                throw new ArgumentNullException(nameof(existingSize));
            }

            existingSize.SizeName = sizeVModel.SizeName;
            existingSize.UpdatedDate = DateTime.Now;

            return existingSize;
        }
    }
}
