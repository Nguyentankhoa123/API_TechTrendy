using System.ComponentModel;

namespace API.Model.Entity
{
    public class DiscountUser
    {
        public int Id { get; set; }
        [DisplayName("Mã khuyến mãi")]
        public int DiscountId { get; set; }
        [DisplayName("Người dùng")]
        public string UserId { get; set; }
        [DisplayName("Thời gian sử dụng")]
        public DateTime UsedAt { get; set; }
        public bool IsActive { get; set; }
        public virtual Discount Discount { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
