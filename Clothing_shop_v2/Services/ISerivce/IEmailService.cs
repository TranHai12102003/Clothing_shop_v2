namespace Clothing_shop_v2.Services.ISerivce
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
