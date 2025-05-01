namespace Clothing_shop_v2.VModels
{
    public class RoleCreateVModel
    {
        public string Name { get; set; }
    }
    public class RoleUpdateVModel : RoleCreateVModel
    {
        public int Id { get; set; }
    }
    public class RoleGetVModel : RoleUpdateVModel
    {
    }
}
