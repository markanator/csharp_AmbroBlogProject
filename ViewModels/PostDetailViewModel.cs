using AmbroBlogProject.Models;
using System.Collections.Generic;

namespace AmbroBlogProject.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    
    }
}
