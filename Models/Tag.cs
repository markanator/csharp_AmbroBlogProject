/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AmbroBlogProject.Models
{
    public class Tag
    {
        public int Id { get; set; }//PK
        public int PostId { get; set; }//FK
        public string BlogUserId { get; set; }//FK

        [Required]
        [StringLength(25,ErrorMessage = "The {0} must be at least {2} and no more than {1}", MinimumLength =2)]
        public string Text { get; set; }

        // navigation  
        public virtual Post Post { get; set; }
        public virtual BlogUser BlogUser { get; set; }
    }
}
