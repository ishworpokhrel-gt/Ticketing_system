namespace HEI.Support.Service.Interface
{
    public interface IEmailService
    {
        Task<bool> SendMailAsync(string toAddress, string subject, string msg);
        Task<bool> SendEmailToMultipleUsersAsync(IEnumerable<string> emails, string subject, string msg);
    }
}
