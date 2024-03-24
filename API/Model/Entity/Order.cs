using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DisplayName("Tình trạng đơn hàng")]
        public string OrderStatus { get; set; }

        [Required]
        [DisplayName("Hình thức giao hàng")]
        public string? ShippingType { get; set; }

        [DisplayName("Thời gian giao hàng dự kiến")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ShippingDate { get; set; }

        [DisplayName("Mã vận chuyển")]
        public string? TrackingNumber { get; set; }

        [DisplayName("Đơn vị vận chuyển")]
        public string? Carrier { get; set; }

        [Required(ErrorMessage = "Chưa chọn phương thức thanh toán")]
        [DisplayName("Phương thức thanh toán")]
        public string? PaymentType { get; set; }

        [DisplayName("Tình trạng thanh toán")]
        public string? PaymentStatus { get; set; }

        [DisplayName("Ngày thanh toán")]
        public DateTime PaymentDate { get; set; }

        public double TotalPrice { get; set; }

        [DisplayName("Giá tiền giảm")]
        public double? DiscountAmount { get; set; }


        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
