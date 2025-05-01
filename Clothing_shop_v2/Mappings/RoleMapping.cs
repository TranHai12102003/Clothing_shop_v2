using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class RoleMapping
    {
        public static Role VModelToEntity(RoleCreateVModel roleCreateVModel)
        {
            return new Models.Role
            {
                RoleName = roleCreateVModel.Name
            };
        }
        public static Role VModelToEntity(RoleUpdateVModel roleUpdateVModel)
        {
            return new Role
            {
                Id = roleUpdateVModel.Id,
                RoleName = roleUpdateVModel.Name
            };
        }
        public static RoleGetVModel EntityToVModel(Role role)
        {
            return new RoleGetVModel
            {
                Id = role.Id,
                Name = role.RoleName
            };
        }
    }
}
