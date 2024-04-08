using API.Model.Dtos.ExternalAuthDto;
using API.Model.Dtos.User;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace API.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpRequest request, string role);

        Task<TokenObjectResponse> SignInAsync(SignInRequest request);

        Task<TokenObjectResponse> GetRefreshTokenAsync(RefreshTokenRequest request);

        Task<UserAddressObjectResponse> AddUserAddressAsync(UserAddressRequest request, string userId);

        Task<UserAddressObjectResponse> UpdateUserAddressAsync(UserAddressRequest request, string userId, int addressId);

        Task<UserAddressListObjectResponse> GetUserAddressAsync(string userId);

        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth);

        Task<TokenObjectResponse> GoogleLogin(ExternalAuthDto externalAuth);

        Task<FacebookUserData> VerifyFacebookToken(ExternalAuthDto externalAuth);

        Task<TokenObjectResponse> FacebookLogin(ExternalAuthDto externalAuth);
    }
}
