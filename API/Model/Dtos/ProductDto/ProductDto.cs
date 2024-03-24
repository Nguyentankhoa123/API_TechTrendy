using API.Model.Entity;

namespace API.Model.Dtos.ProductDto
{
    public class ProductDto
    {
        public Product Product { get; set; }
        public string GraphicsCard { get; set; }

        public string Connectivity { get; set; }
        public string Sim { get; set; }
        public string RearCamera { get; set; }

        public string BackCamera { get; set; }
        public string Battery { get; set; }
    }
}
