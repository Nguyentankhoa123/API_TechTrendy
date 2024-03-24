using System.Text.Json.Serialization;

namespace API.Model.Entity
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
