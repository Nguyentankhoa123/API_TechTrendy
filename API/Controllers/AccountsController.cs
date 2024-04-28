using API.Helpers;
using API.Model.Dtos.ExternalAuthDto;
using API.Model.Dtos.User;
using API.Model.Entity;
using API.Repositories;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scriban;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailSenderRepository _emailSender;
        public readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(IAccountRepository accountRepository, IEmailSenderRepository emailSender, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var result = await _accountRepository.SignInAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Sign up a new admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignUpAdmin")]
        public async Task<IActionResult> SignUpAdmin([FromBody] SignUpRequest request)
        {
            var result = await _accountRepository.SignUpAsync(request, Roles.Admin);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(SignUpAdmin), request);
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Sign up a new customer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignUpCustomer")]
        public async Task<IActionResult> SignUpCustomer([FromBody] SignUpRequest request)
        {
            var result = await _accountRepository.SignUpAsync(request, Roles.Customer);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(SignUpCustomer), request);
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Sign up a new employee
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignUpEmployee")]
        public async Task<IActionResult> SignUpEmployee([FromBody] SignUpRequest request)
        {
            var result = await _accountRepository.SignUpAsync(request, Roles.Employee);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(SignUpEmployee), request);
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Sign in with Google
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignInGoogle")]
        public async Task<IActionResult> SignInGoogle([FromBody] ExternalAuthDto request)
        {
            var result = await _accountRepository.GoogleLogin(request);
            return Ok(result);
        }

        /// <summary>
        /// Sign in with Facebook
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignInFacebook")]
        public async Task<IActionResult> SignInFacebook([FromBody] ExternalAuthDto request)
        {
            var result = await _accountRepository.FacebookLogin(request);
            return Ok(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.Email != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var forgotPassword = $"http://localhost:5173/resetpassword?email={email}&token={token}";
                    if (forgotPassword != null)
                    {
                        var template = Template.Parse(@"
                            <!DOCTYPE html>
                            <html lang=""en-US"">
                              <head>
                                <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"" />
                                <title>Reset Password Email Template</title>
                                <meta name=""description"" content=""Reset Password Email Template."" />
                              </head>

                              <body
                                marginheight=""0""
                                topmargin=""0""
                                marginwidth=""0""
                                style=""margin: 0px; background-color: #f2f3f8; height: 100vh""
                                leftmargin=""0""
                              >
                                <table
                                  cellspacing=""0""
                                  border=""0""
                                  cellpadding=""0""
                                  width=""100%""
                                  bgcolor=""#f2f3f8""
                                  style=""height: 100%""
                                >
                                  <tr>
                                    <td>
                                      <table
                                        style=""background-color: #f2f3f8; max-width: 670px; margin: 0 auto""
                                        width=""100%""
                                        border=""0""
                                        align=""center""
                                        cellpadding=""0""
                                        cellspacing=""0""
                                      >
                                        <tr>
                                          <td style=""text-align: center"">
                                            <img
                                              width=""150px""
                                              src=""https://res.cloudinary.com/dija8tzko/image/upload/v1711021340/E-Commerce/TechTrendyChangeSize_j4jfa9.png""
                                              title=""logo""
                                              alt=""logo""
                                            />
                                          </td>
                                        </tr>
                                        <tr>
                                          <td style=""height: 20px"">&nbsp;</td>
                                        </tr>
                                        <tr>
                                          <td>
                                            <table
                                              width=""95%""
                                              border=""0""
                                              align=""center""
                                              cellpadding=""0""
                                              cellspacing=""0""
                                              style=""
                                                max-width: 670px;
                                                background: #fff;
                                                border-radius: 3px;
                                                text-align: center;
                                                -webkit-box-shadow: 0 6px 18px 0 rgba(0, 0, 0, 0.06);
                                                -moz-box-shadow: 0 6px 18px 0 rgba(0, 0, 0, 0.06);
                                                box-shadow: 0 6px 18px 0 rgba(0, 0, 0, 0.06);
                                              ""
                                            >
                                              <tr>
                                                <td style=""height: 40px"">&nbsp;</td>
                                              </tr>
                                              <tr>
                                                <td style=""padding: 0 35px"">
                                                  <h1
                                                    style=""
                                                      color: #1e1e2d;
                                                      font-weight: 500;
                                                      margin: 0;
                                                      font-size: 32px;
                                                      font-family: 'Rubik', sans-serif;
                                                    ""
                                                  >
                                                    Yêu cầu lấy lại mật khẩu
                                                  </h1>
                                                  <span
                                                    style=""
                                                      display: inline-block;
                                                      vertical-align: middle;
                                                      margin: 29px 0 26px;
                                                      border-bottom: 1px solid #cecece;
                                                      width: 100px;
                                                    ""
                                                  ></span>
                                                  <p
                                                    style=""
                                                      color: #455056;
                                                      font-size: 16px;
                                                      line-height: 24px;
                                                      margin: 0;
                                                    ""
                                                  >
                                                    Để thay đổi mật khẩu hãy nhấp vào liên kết sau và làm
                                                    theo hướng dẫn
                                                  </p>
                                                  <a
                                                    href=""{{ link }}""
                                                    style=""
                                                      background: #20e277;
                                                      text-decoration: none !important;
                                                      font-weight: 500;
                                                      margin-top: 35px;
                                                      color: #fff;
                                                      text-transform: uppercase;
                                                      font-size: 14px;
                                                      padding: 10px 24px;
                                                      display: inline-block;
                                                    ""
                                                    >Thay đổi mật khẩu</a
                                                  >
                                                </td>
                                              </tr>
                                              <tr>
                                                <td style=""height: 40px"">&nbsp;</td>
                                              </tr>
                                            </table>
                                          </td>
                                        </tr>
                                      </table>
                                    </td>
                                  </tr>
                                </table>
                              </body>
                            </html>");

                        var emailContent = await template.RenderAsync(new { link = forgotPassword });
                        var message = new Message(new string[] { user.Email }, "Forgot Password link", emailContent);
                        _emailSender.SendEmail(message);

                        return Ok($"Email has been successfully sent to {user.Email}");
                    }
                    else
                    {
                        return BadRequest("Failed to generate forgot password link");
                    }
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var result = new ResetPassword { Token = token, Email = email };
            return Ok(result);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    var errors = resetPassResult.Errors.Select(error => new { error.Code, error.Description });
                    return BadRequest(errors);
                }

                return Ok("Password has been changed");
            }

            return BadRequest("Couldnot send link to email, please try again.");
        }

        /// <summary>
        /// Delete account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok("User deleted successfully");
            }
            else
            {
                return BadRequest("Error deleting user");
            }
        }

    }
}
