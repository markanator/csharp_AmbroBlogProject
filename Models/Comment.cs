using AmbroBlogProject.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AmbroBlogProject.Models
{
    public class Comment
    {
        public int Id { get; set; }//PK
        public int PostId { get; set; }//FK
        public string AuthorId { get; set; }//FK
        public string ModeratorId { get; set; }//FK

        [Required]
        [StringLength(500, ErrorMessage ="The {0} must be at least {2} and no more than {1}", MinimumLength = 2)]
        [Display(Name ="Comment")]
        public string Body { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Moderated { get; set; }
        public DateTime? Deleted { get; set; }

        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1}", MinimumLength = 2)]
        [Display(Name = "Moderated Comment")]
        public string ModeratedBody { get; set; }

        public ModerationType ModerationType { get; set; }

        // navigation  
        public virtual Post Post { get; set; }
        public virtual IdentityUser Author { get; set; }
        public virtual IdentityUser Moderator { get; set; }

    }
}
