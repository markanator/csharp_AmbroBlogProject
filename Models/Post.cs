/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using AmbroBlogProject.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmbroBlogProject.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Display(Name = "Blog Name")]
        public int BlogId { get; set; }

        [Display(Name = "Author Name")]
        public string BlogUserId { get; set; }

        [Required]
        [StringLength(75)]
        public string Title { get; set; }

        [Required]
        public string Abstract { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Updated Date")]
        public DateTime? Updated { get; set; }


        public ReadyStatus ReadyStatus { get; set; }

        public string Slug { get; set; }

        public byte[] ImageDate { get; set; }
        public string ContentType { get; set; }
        
        [NotMapped]
        public IFormFile Image { get; set; }

        // navigation  
        public virtual Blog Blog { get; set; }
        public virtual BlogUser BlogUser { get; set; }


        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
