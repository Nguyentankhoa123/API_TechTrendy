using API.Data;
using API.Model.Dtos.ExternalAuthDto;
using API.Model.Dtos.User;
using API.Model.Entity;
using API.Repositories;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Repository.Implement
{
    public class AccountRepository : IAccountRepository
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;
        private readonly StoreContext _storeContext;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper, StoreContext storeContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _storeContext = storeContext;
        }



        public async Task<TokenObjectResponse> GetRefreshTokenAsync(RefreshTokenRequest request)
        {
            var response = new TokenObjectResponse();
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            string username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new NotFoundException("Invalid access token or refresh token");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var newAccessToken = GenerateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            return response;
        }

        public async Task<TokenObjectResponse> SignInAsync(SignInRequest request)
        {
            var response = new TokenObjectResponse();
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {

                throw new NotFoundException("Invalid Username");
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new NotFoundException("Invalid Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(authClaims),
                RefreshToken = GenerateRefreshToken(),
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpRequest request, string role)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists" });
            }

            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var secret = _configuration["JWT:Secret"] ?? "";
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var expirationTimeUtc = DateTime.UtcNow.AddHours(1);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTimeUtc, localTimeZone);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Expires = expirationTimeInLocalTimeZone,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
        {
            var secret = _configuration["JWT:Secret"] ?? "";
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<UserAddressDto> AddUserAddressAsync(UserAddressDto userAddressDto, string userId)
        {
            // Check if a UserAddress already exists for the user
            var existingAddress = await _storeContext.UserAddresses
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existingAddress != null)
            {
                throw new NotFoundException("The address already exists. Please update the address instead of creating a new one.");
            }

            var address = _mapper.Map<UserAddress>(userAddressDto);
            address.UserId = userId;

            _storeContext.UserAddresses.Add(address);
            await _storeContext.SaveChangesAsync();

            var addressDto = _mapper.Map<UserAddressDto>(address);

            return addressDto;
        }

        public async Task<UserAddressDto> UpdateUserAddressAsync(UserAddressDto userAddressDto, string userId)
        {
            var existingAddress = await _storeContext.UserAddresses
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAddress == null)
            {
                throw new NotFoundException("No address found for the specified user.");
            }

            _mapper.Map(userAddressDto, existingAddress);
            _storeContext.UserAddresses.Update(existingAddress);
            await _storeContext.SaveChangesAsync();

            var updatedAddressDto = _mapper.Map<UserAddressDto>(existingAddress);
            return updatedAddressDto;
        }

        public async Task<UserAddressDto> GetUserAddressAsync(string userId)
        {
            var existingAddress = await _storeContext.UserAddresses
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existingAddress == null)
            {
                throw new NotFoundException("No address found for the specified user.");
            }

            var userAddressDto = _mapper.Map<UserAddressDto>(existingAddress);

            return userAddressDto;
        }

        public async Task<string> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = payload.Name,
                    Email = payload.Email,
                    // Add additional fields if needed
                };

                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    throw new BadRequestException("Failed to create new user.");
                }

                await _userManager.AddToRoleAsync(user, "Viewer");
                await _userManager.AddLoginAsync(user, info);
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateToken(authClaims);

            return token;
        }
    }
}
