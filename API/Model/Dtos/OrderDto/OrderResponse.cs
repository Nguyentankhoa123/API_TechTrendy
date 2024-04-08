using API.Model.Dtos.User;
using API.Services.Common;
using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.OrderDto
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string OrderStatus { get; set; }

        public string? ShippingType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ShippingDate { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }


        public string? PaymentType { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public double TotalPrice { get; set; }
        public double? DiscountAmount { get; set; }
        public string? PaymentUrl { get; set; }
        public ICollection<UserAddressDto> Address { get; set; }
        public ICollection<OrderDetailDto> OrderDetails { get; set; }
    }

    public class OrderObjectResponse : ObjectResponse<OrderResponse> { }
}
