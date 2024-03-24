using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.User
{
    public class SignUpDto
    {
        [Required]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]

        public string ComfirmPassword { get; set; }
    }
}
