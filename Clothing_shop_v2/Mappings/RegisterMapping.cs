using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class RegisterMapping
    {
        public static User Register (RegisterVModel register)
        {
            return new User
            {
                FullName = register.FullName,
                Username = register.Username,
                Gender = register.Gender,
                Email = register.Email,
                BirthDate = register.BirthDate,
                PhoneNumber = register.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Address = register.Address,
                IsActive = false,
                RoleId= 2,
                CreatedDate = DateTime.Now,
            };
        }
    }
}
