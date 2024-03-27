namespace API.Model.Dtos.ExternalAuthDto
{
    public class FacebookVerifyDto
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
