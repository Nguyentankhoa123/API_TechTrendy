namespace API.Model.Dtos.OrderDto
{
    public class OrderDetailDto
    {
        public int ProductId { get; set; }

        public double Total { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrls { get; set; }

        public string Name { get; set; }
    }
}
