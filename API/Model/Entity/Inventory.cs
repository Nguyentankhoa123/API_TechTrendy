using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity
{
    public class Inventory
    {
        public int Id { get; set; }

        public string Location { get; set; } = "unKnow";
        [Required]
        public int Stock { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
