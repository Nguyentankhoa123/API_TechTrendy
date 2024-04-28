namespace API.Model.Entity
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int OrderId { get; set; }
        public double Price { get; set; }
        public string PictureUrls { get; set; }
        public string Name { get; set; }
        public Order Order { get; set; }
        public double Total { get; set; }
        public int Quantity { get; set; }
    }
}
