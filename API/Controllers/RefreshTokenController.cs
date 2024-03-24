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

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _accountRepository.GetRefreshTokenAsync(request);
            return Ok(result);
        }

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

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
            return Ok("Success");
        }


        //[HttpPost]
        //[Route("Refresh-Token")]
        //public async Task<IActionResult> RefreshTokenTest(TokenRespone token)
        //{
        //    var result = await _accountRepository.RenewAccessTokenAsync(token);


        //    return Ok(result);

        //}
    }
}