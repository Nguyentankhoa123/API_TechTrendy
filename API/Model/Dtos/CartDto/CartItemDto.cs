namespace API.Model.Dtos.CartDto
{
    public class CartItemDto
    {

        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }
    }
}
