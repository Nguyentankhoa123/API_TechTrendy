using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        // Foreign key for User
        [Required]
        public string UserId { get; set; }

        // Navigation property for User
        public virtual ApplicationUser User { get; set; }
    }
}
