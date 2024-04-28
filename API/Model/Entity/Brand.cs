using System.Text.Json.Serialization;

namespace API.Model.Entity
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ProductType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
