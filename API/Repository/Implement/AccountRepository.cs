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
using Newtonsoft.Json;
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
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.GivenName, user.FirstName),
               new Claim(ClaimTypes.Surname, user.LastName),
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
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
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

        public async Task<UserAddressObjectResponse> AddUserAddressAsync(UserAddressRequest request, string userId)
        {

            var address = _mapper.Map<UserAddress>(request);
            address.UserId = userId;

            _storeContext.UserAddresses.Add(address);
            await _storeContext.SaveChangesAsync();


            var response = new UserAddressObjectResponse()
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Get UserAddress",
                Data = _mapper.Map<UserAddressResponse>(address)
            };

            return response;
        }

        public async Task<UserAddressObjectResponse> UpdateUserAddressAsync(UserAddressRequest request, string userId, int addressId)
        {
            var existingAddress = await _storeContext.UserAddresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == addressId);

            if (existingAddress == null)
            {
                throw new NotFoundException("No address found for the specified user.");
            }


            _mapper.Map(request, existingAddress);
            _storeContext.UserAddresses.Update(existingAddress);
            await _storeContext.SaveChangesAsync();

            var response = new UserAddressObjectResponse()
            {
                StatusCode = ResponseCode.CREATED,
                Message = "Update UserAddress",
                Data = _mapper.Map<UserAddressResponse>(existingAddress)
            };

            return response;
        }

        public async Task<UserAddressListObjectResponse> GetUserAddressAsync(string userId)
        {
            var existingAddresses = await _storeContext.UserAddresses
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (existingAddresses == null || !existingAddresses.Any())
            {
                throw new NotFoundException("No address found for the specified user.");
            }


            var response = new UserAddressListObjectResponse()
            {
                StatusCode = ResponseCode.OK,
                Message = "Get UserAddress Id",
                Data = _mapper.Map<List<UserAddressResponse>>(existingAddresses)
            };

            return response;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Google:ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                //log an exception
                throw new Exception(ex.Message);
            }
        }

        public async Task<TokenObjectResponse> GoogleLogin(ExternalAuthDto externalAuth)
        {
            var response = new TokenObjectResponse();
            var payload = await VerifyGoogleToken(externalAuth);
            if (payload == null)
            {
                throw new BadRequestException("Invalid External Authentication.");
            };

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, "Customer"); // Add user to "Customer" role
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            if (user == null)
            {
                throw new BadRequestException("Invalid External Authentication.");
            }

            // Create a list of claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "Customer") // Add "Customer" role claim
            };

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(claims),
                RefreshToken = GenerateRefreshToken()
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<FacebookUserData> VerifyFacebookToken(ExternalAuthDto externalAuth)
        {
            var appId = _configuration["Facebook:AppId"];
            var appSecret = _configuration["Facebook:AppSecret"];
            var validationUri = $"https://graph.facebook.com/debug_token?input_token={externalAuth.IdToken}&access_token={appId}|{appSecret}";

            using var client = new HttpClient();
            var validationResponse = await client.GetAsync(validationUri);

            if (!validationResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to validate Facebook access token.");
            }

            var validationContent = await validationResponse.Content.ReadAsStringAsync();
            var validationResult = JsonConvert.DeserializeObject<dynamic>(validationContent);

            if (validationResult.data.app_id != appId)
            {
                throw new BadRequestException("Invalid Facebook App ID.");
            }

            var userUri = $"https://graph.facebook.com/v13.0/me?fields=id,name,email,first_name,last_name&access_token={externalAuth.IdToken}";
            var userResponse = await client.GetAsync(userUri);

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to fetch Facebook user data.");
            }

            var userContent = await userResponse.Content.ReadAsStringAsync();
            var fbUserData = JsonConvert.DeserializeObject<FacebookUserData>(userContent);

            return fbUserData;


        }



        public async Task<TokenObjectResponse> FacebookLogin(ExternalAuthDto externalAuth)
        {
            var response = new TokenObjectResponse();

            // Verify Facebook token
            var fbUserData = await VerifyFacebookToken(externalAuth);
            if (fbUserData == null)
            {
                throw new BadRequestException("Invalid Facebook access token.");
            }

            // Find user by email (assuming email is unique)
            var user = await _userManager.FindByEmailAsync(fbUserData.Email);
            if (user != null)
            {
                throw new BadRequestException("Email đã được tạo rồi bạn ơi");
            }
            // Create user if not found
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = fbUserData.Email,
                    UserName = fbUserData.Email,
                    FirstName = fbUserData.First_Name,
                    LastName = fbUserData.Last_Name
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user: " + result.Errors.FirstOrDefault()?.Description);
                }
                await _userManager.AddToRoleAsync(user, "Customer"); // Add user to "Customer" role
            }

            // Associate Facebook login with the user (assuming user already exists)
            var info = new UserLoginInfo(externalAuth.Provider, fbUserData.Id, externalAuth.Provider);
            await _userManager.AddLoginAsync(user, info);

            // Generate authentication token (replace with your token generation logic)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "Customer") // Add "Customer" role claim
            };

            response.StatusCode = ResponseCode.OK;
            response.Message = "Success";
            response.Data = new TokenResponse
            {
                AccessToken = GenerateToken(claims), // Replace with your JWT generation logic
                RefreshToken = GenerateRefreshToken() // Replace with your refresh token generation logic
            };

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWT:RefreshTokenValidityInDays"]);
            user.RefreshToken = response.Data.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return response;
        }
    }
}
