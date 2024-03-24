using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.OrderDto
{
    public class OrderRequest
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; }

        public string? ShippingType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ShippingDate { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }


        public string? PaymentType { get; set; }

        public string? PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }



    }
}
