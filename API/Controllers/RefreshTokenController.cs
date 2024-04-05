using API.Model.Dtos.User;
using API.Model.Entity;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenController(IAccountRepository accountRepository, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _userManager = userManager;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _accountRepository.GetRefreshTokenAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Revoke refresh token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("Invalid username");
            }

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Ok("Success");
        }
    }
}