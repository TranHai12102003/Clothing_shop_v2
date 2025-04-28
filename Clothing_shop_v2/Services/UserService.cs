using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Response;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Clothing_shop_v2.Services
{
    public class UserService : IUserService
    {
        private readonly ClothingShopDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        public UserService(ClothingShopDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }
        public async Task<RegisterReponse> RegisterUser(RegisterVModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return new RegisterReponse { Success = false, Message = "Email đã tồn tại" };

            //var user = new User
            //{
            //    FullName = model.FullName,
            //    Username = model.Username,
            //    Email = model.Email,
            //    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            //    PhoneNumber = model.PhoneNumber,
            //    Gender = model.Gender,
            //    Address = model.Address,
            //    RoleId = 1,
            //    IsActive = false,
            //    CreatedDate = DateTime.Now,
            //};
            var user = RegisterMapping.Register(model);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            try
            {
                await SendActivationEmail(user);
            }
            catch (Exception ex)
            {
                return new RegisterReponse
                {
                    Success = true,
                    Message = $"Đăng ký thành công, nhưng gửi email xác nhận thất bại: {ex.Message}. Vui lòng liên hệ hỗ trợ."
                };
            }

            return new RegisterReponse
            {
                Success = true,
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để kích hoạt tài khoản."
            };
        }

        private async Task SendActivationEmail(User user)
        {
            var activationToken = GenerateActivationToken(user);
            var confirmationLink = $"{_config["AppSettings:BaseUrl"]}/activate?token={activationToken}";
            var resendLink = $"{_config["AppSettings:BaseUrl"]}/resend-activation?email={user.Email}";
            var emailBody = GenerateEmailBody(user.FullName, confirmationLink, resendLink);

            await _emailService.SendEmailAsync(user.Email, "Xác nhận đăng ký tài khoản", emailBody);
        }
        private string GenerateActivationToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateEmailBody(string userName, string confirmationLink, string resendLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Template", "index.html");
            var templateContent = File.ReadAllText(templatePath);
            return templateContent
                .Replace("{{userName}}", userName)
                .Replace("{{confirmationLink}}", confirmationLink)
                .Replace("{{resendActivationEmail}}", resendLink);
        }

        public async Task<RegisterReponse> ActivateAccount(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Loại bỏ độ lệch thời gian mặc định (5 phút)
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                var userIdClaim = principal.FindFirst("Id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return new RegisterReponse { Success = false, Message = "Token không hợp lệ" };

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return new RegisterReponse { Success = false, Message = "Người dùng không tồn tại" };

                if (user.IsActive)
                    return new RegisterReponse { Success = false, Message = "Tài khoản đã được kích hoạt" };

                user.IsActive = true;
                await _context.SaveChangesAsync();

                return new RegisterReponse { Success = true, Message = "Tài khoản đã được kích hoạt thành công" };
            }
            catch (SecurityTokenExpiredException)
            {
                return new RegisterReponse
                {
                    Success = false,
                    Message = "Link kích hoạt đã hết hạn. Vui lòng nhấp vào 'Gửi lại' trong email để nhận link mới."
                };
            }
            catch (Exception ex)
            {
                return new RegisterReponse { Success = false, Message = $"Lỗi khi kích hoạt: {ex.Message}" };
            }
        }

        public async Task<LoginResponse> Login(LoginVModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return new LoginResponse { Success = false, Message = "Email hoặc mật khẩu không đúng" };

            if (!user.IsActive)
                return new LoginResponse { Success = false, Message = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email hoặc yêu cầu gửi lại link kích hoạt." };

            var token = GenerateJwtToken(user);
            return new LoginResponse { Success = true, Token = token, Role = user.Role.RoleName, UserID = user.Id, FullName = user.FullName, Message = "Đăng nhập thành công" };
        }

        private string GenerateJwtToken(User user)
        {
            // Đảm bảo user đã có Role
            if (user.Role == null)
            {
                user = _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == user.Id);

                if (user == null || user.Role == null)
                {
                    throw new Exception("Không tìm thấy vai trò người dùng.");
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };
            Console.WriteLine(claims);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
