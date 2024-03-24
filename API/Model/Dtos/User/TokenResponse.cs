using API.Services.Common;

namespace API.Model.Dtos.User
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class TokenObjectResponse : ObjectResponse<TokenResponse> { }
}
