using System.Threading.Tasks;

namespace AbrantosAPI.Utils
{
    public interface IEmailSender
    {
         Task SendEmailAsync(string email, string subject, string message);
    }
}