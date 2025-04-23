using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class ColorMapping
    {
        public static ColorGetVModel EntityToVModel(Color color)
        {
            return new ColorGetVModel
            {
                Id = color.Id,
                ColorName = color.ColorName,
                ColorCode = color.ColorCode,
            };
        }
        public static Color VModelToEntity(ColorCreateVModel colorVModel)
        {
            return new Color
            {
                ColorName = colorVModel.ColorName,
                ColorCode = colorVModel.ColorCode,
                CreatedDate = DateTime.Now,
            };
        }
        public static Color VModelToEntity(ColorUpdateVModel colorVModel, Color existingColor)
        {
            if (existingColor == null)
            {
                throw new ArgumentNullException(nameof(existingColor));
            }
            existingColor.ColorName = colorVModel.ColorName;
            existingColor.ColorCode = colorVModel.ColorCode;
            existingColor.UpdatedDate = DateTime.Now;
            return existingColor;
        }
    }
}
