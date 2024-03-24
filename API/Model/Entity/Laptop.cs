using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity
{
    public class Laptop : Product
    {

        [Required]
        public string GraphicsCard { get; set; }
    }

    //public class Laptop
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    public int ProductId { get; set; }
    //    public virtual Product Product { get; set; }
    //    [Required]
    //    public string GraphicsCard { get; set; }

    //}
}
