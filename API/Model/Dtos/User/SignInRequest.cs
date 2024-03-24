using System.ComponentModel.DataAnnotations;

namespace API.Model.Dtos.User
{
    public class SignInRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
