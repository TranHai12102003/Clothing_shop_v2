using Clothing_shop_v2.Models;
using Clothing_shop_v2.Response;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IUserService
    {
        Task<RegisterReponse> RegisterUser(RegisterVModel model);
        Task<RegisterReponse> ActivateAccount(string token);
        Task<LoginResponse> Login(LoginVModel model);

    }
}
