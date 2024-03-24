using API.Model.Dtos.User;
using Microsoft.AspNetCore.Identity;

namespace API.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpRequest request, string role);

        Task<TokenObjectResponse> SignInAsync(SignInRequest request);

        Task<TokenObjectResponse> GetRefreshTokenAsync(RefreshTokenRequest request);

        Task<UserAddressDto> AddUserAddressAsync(UserAddressDto userAddressDto, string userId);

        Task<UserAddressDto> UpdateUserAddressAsync(UserAddressDto userAddressDto, string userId);

        Task<UserAddressDto> GetUserAddressAsync(string userId);
    }
}
