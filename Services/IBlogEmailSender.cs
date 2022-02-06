using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AmbroBlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailfrom, string name, string subject, string htmlMessage);
    }
}
