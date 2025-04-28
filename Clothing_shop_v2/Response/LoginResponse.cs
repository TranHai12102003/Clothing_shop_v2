namespace Clothing_shop_v2.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }  // JWT Token
        public int UserID { get; set; }  // ID của User
        public string Role { get; set; }  // Quyền (Admin, Customer)
        public string FullName { get; set; }  // Tên đầy đủ của User
    }
}
