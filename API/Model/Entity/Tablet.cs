using System.ComponentModel.DataAnnotations;

namespace API.Model.Entity
{
    public class Tablet : Product
    {


        [Required]
        public string Connectivity { get; set; }
        [Required]
        public string Sim { get; set; }
        [Required]
        public string RearCamera { get; set; }
        [Required]

        public string BackCamera { get; set; }
        [Required]
        public string Battery { get; set; }
    }

    //public class Tablet
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    public int ProductId { get; set; }
    //    public virtual Product Product { get; set; }
    //    [Required]
    //    public string Connectivity { get; set; }
    //    [Required]
    //    public string Sim { get; set; }
    //    [Required]
    //    public string RearCamera { get; set; }
    //    [Required]

    //    public string BackCamera { get; set; }
    //    [Required]
    //    public string Battery { get; set; }
    //}
}
