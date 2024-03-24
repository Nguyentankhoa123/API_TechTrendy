using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.User
{
    public class SignInDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
