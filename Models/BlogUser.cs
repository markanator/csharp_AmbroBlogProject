/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbroBlogProject.Models
{
    public class BlogUser: IdentityUser
    {
        [Required]
        [StringLength(50,ErrorMessage ="The {0} must be at least {2} and no more than {1} characters long",MinimumLength =2)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        public string FacebookUrl { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
        public string TwitterUrl { get; set; }

        [NotMapped]
        public string FullName { get => $"{this.FirstName} {this.LastName}"; }

        public ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
