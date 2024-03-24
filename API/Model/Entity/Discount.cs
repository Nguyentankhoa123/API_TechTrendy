using API.Shared.Enums;
using System.ComponentModel;

namespace API.Model.Entity
{
    public class Discount
    {
        public int Id { get; set; }
        [DisplayName("Tên mã giảm giá")]
        public string Name { get; set; }
        [DisplayName("Mô tả mã giảm giá")]
        public string Description { get; set; }
        [DisplayName("Giá trị giảm giá")]
        public double Percent { get; set; }
        [DisplayName("Giá tiền tối đa có thể giảm")]
        public double? MaxDiscountAmount { get; set; }
        [DisplayName("Giá trị đơn hàng tối thiểu")]
        public double? MinOrderValue { get; set; }
        [DisplayName("Mã code")]
        public string Code { get; set; }
        [DisplayName("Thời gian bắt đầu")]
        public DateTime StartDate { get; set; }
        [DisplayName("Thời gian kết thúc")]
        public DateTime EndDate { get; set; }
        [DisplayName("Số lượng mã được áp dụng")]
        public int MaxUse { get; set; }
        [DisplayName("Số lượng đã sử dụng")]
        public int UseCount { get; set; }
        [DisplayName("Trạng thái mã")]
        public bool IsActive { get; set; }
        public DiscountApply ApplyTo { get; set; } = DiscountApply.All;
        public List<int>? ProductId { get; set; }

        public virtual ICollection<DiscountUser> DiscountUsers { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
