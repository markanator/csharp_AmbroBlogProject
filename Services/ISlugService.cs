/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
namespace AmbroBlogProject.Services
{
    public interface ISlugService
    {
        string UrlFriendly(string title);
        bool IsUnique(string slug);
    }
}
