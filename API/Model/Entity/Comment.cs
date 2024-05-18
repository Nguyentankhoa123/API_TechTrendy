using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity

// Nested set model
{
    public class Comment
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [DisplayName("Nội dung")]
        public string Content { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }

        [DisplayName("Ngày đánh giá")]
        public DateTime Date { get; set; } = DateTime.Now;

        public int? ParentId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsQuestion { get; set; } = false;


        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
