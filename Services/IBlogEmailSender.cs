/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AmbroBlogProject.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailfrom, string name, string subject, string htmlMessage);
    }
}
