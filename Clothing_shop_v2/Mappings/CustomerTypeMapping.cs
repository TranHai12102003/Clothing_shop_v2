using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class CustomerTypeMapping
    {
        public static CustomerType VModelToModel(CustomerTypeCreateVModel vmodel)
        {
            return new CustomerType
            {
                TypeName = vmodel.TypeName,
                Description = vmodel.Description,
                MinOrderCount = vmodel.MinOrderCount,
                MinTotalAmount = vmodel.MinTotalAmount,
                CreatedDate = DateTime.Now
            };
        }
        public static CustomerType VModelToModel(CustomerTypeUpdateVModel vmodel, CustomerType model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            model.TypeName = vmodel.TypeName;
            model.Description = vmodel.Description;
            model.MinOrderCount = vmodel.MinOrderCount;
            model.MinTotalAmount = vmodel.MinTotalAmount;
            model.UpdatedDate = DateTime.Now;
            return model;
        }
        public static CustomerTypeGetVModel ModelToVModel(CustomerType model)
        {
            return new CustomerTypeGetVModel
            {
                Id = model.Id,
                TypeName = model.TypeName,
                Description = model.Description,
                MinOrderCount = model.MinOrderCount,
                MinTotalAmount = model.MinTotalAmount,
                CreatedDate = model.CreatedDate,
                UpdatedDate = model.UpdatedDate
            };
        }
    }
}
