using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using vectio.auth.api.db.entities;

namespace vectio.auth.api.controllers
{


    [Route("api/[controller]")]
    [ApiController] 
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailHelper _emailHelper;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, EmailHelper emailHelper)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._emailHelper = emailHelper;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && user.EmailConfirmed && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Fullname),
                    new Claim("roles", string.Join(",",roles))
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("B55AAF36-085B-4D46-BCE3-9CA3641411D1"));

                var token = new JwtSecurityToken(
                    issuer: "https://tasks.vectio.pl",
                    audience: "https://tasks.vectio.pl",
                    expires: DateTime.Now.AddHours(24),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
       
        [HttpGet]
        [Route("user/{id?}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                id = this.User.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user = new
                {
                    id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    fullname = user.Fullname,
                    emailConfirmed = user.EmailConfirmed,
                    roles = roles.ToArray(),
                }
            });
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.OrderBy(u => u.Email).Select(u => new
            {
                id = u.Id,
                email = u.Email,
                fullname = u.Fullname,
                emailConfirmed = u.EmailConfirmed
            }).ToListAsync();

            return Ok(new
            {
                users
            });
        }

        [HttpPost]
        [Route("user")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Fullname = model.Fullname
            };
            var result = await _userManager.CreateAsync(user, model.Password??"QAZ!@#qaz123");

            if (!result.Succeeded)
                return BusinessError("Unable to create user account", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            if (model.Roles != null && model.Roles.Any())
                await _userManager.AddToRolesAsync(user, model.Roles);
            if (model.Role != null)
                await _userManager.AddToRoleAsync(user, model.Role);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = model.ConfirmationUrl + $"?uid={user.Id}&code={code}";
                await _emailHelper.SendEmailAsync(user.Email, "Account activation request", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Państwa  konto zostało stworzone w systemie {1}.<br/><br/>
    Link do aktywacji konta: <a href='{0}'>Aktywuj konto</a><br/><br/>Link zachowuje ważność przez 7 dni.
    <br/><br/><br/>
    Dear Sir / Madame,
    <br/><br/>
    New account has been created for you on {1}.<br/><br/>
    Link to account activation:<br/> <a href='{0}'>Activate account</a><br/><br/>Link remain valid for 7 days.
    <br/><br/><br/>
    E-mail wygenerowany automatycznie z portalu {1} / This email was generated automatically by {1} Portal -  powered by Vectio Business Platform.", callbackUrl, "Vectio tasks"));
    
            return CreatedAtAction(nameof(GetUser), new { id = user.Id, message = $"Confirmation email has been sent to {user.Email}." }, new
            {
                Id = user.Id
            });
        }

        [HttpPost]
        [Route("sendActivationEmail")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> SendActivationEmail([FromBody] SendActivationEmailData model)
        {
            var user = await _userManager.FindByIdAsync(model.Uid);
            if (user == null)
                return NotFound(new { message = "User not found" });
            if (user.EmailConfirmed)
                user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = model.CallbackUrl + $"?uid={user.Id}&code={code}";
            await _emailHelper.SendEmailAsync(user.Email, "Account activation request", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Państwa  konto zostało stworzone w systemie {1}.<br/><br/>
    Link do aktywacji konta: <a href='{0}'>Aktywuj konto</a><br/><br/>Link zachowuje ważność przez 7 dni.
    <br/><br/><br/>
    Dear Sir / Madame,
    <br/><br/>
    New account has been created for you on {1}.<br/><br/>
    Link to account activation:<br/> <a href='{0}'>Activate account</a><br/><br/>Link remain valid for 7 days.
    <br/><br/><br/>
    E-mail wygenerowany automatycznie z portalu {1} / This email was generated automatically by {1} Portal -  powered by Vectio Business Platform.", callbackUrl, "Vectio tasks"));

            return Ok(new { message = "Account activation email has been sent successfully" });
        }

        [HttpPost]
        [Route("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailVerificationData data)
        {
            var user = await _userManager.FindByIdAsync(data.Uid);
            if (user == null)
                return NotFound(new { message = "User not found" });
            if (user.EmailConfirmed)
                return BusinessError("Your email has already been verified", new { issues = new[] { "Please sign in"} });

            var result =await _userManager.ConfirmEmailAsync(user, data.Code);
            if (!result.Succeeded)
                return BusinessError("Email verification failed", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Email has been verified successfully"});
        }

        [HttpPost]
        [Route("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordData model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
                return NotFound(new { message = "User with given email not found" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = model.CallbackUrl + $"?uid={user.Id}&code={token}";
            await _emailHelper.SendEmailAsync(user.Email, "Password reset request", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Otrzymaliśmy żądanie zmiany hasła do państwa konta w systemie {1}.<br/><br/>
    Link do zmiany hasła:<br/> <a href='{0}'>Zmień hasło</a>
    <br/><br/><br/>
    Dear Sir / Madame,
    <br/><br/>
    We have received request for changing your account's password on {1}.<br/><br/>
    Link to password change:<br/> <a href='{0}'>Change password</a>
    <br/><br/><br/>
    E-mail wygenerowany automatycznie z portalu {1} / This email was generated automatically by {1} Portal -  powered by Vectio Business Platform.", callbackUrl, "Vectio tasks"));


            return Ok(new { message = "Password reset confirmation email has been sent successfully" });
        }

        [HttpPost]
        [Route("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordData model)
        {
            var user = await _userManager.FindByIdAsync(model.Uid);
            if (user == null)
                return NotFound(new { message = "User w not found" });
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
                return BusinessError("Password reset failed", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Password has been reset successfully" });
        }

        [HttpPost]
        [Route("setPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordData data)
        {
            var user = await _userManager.FindByIdAsync(data.Uid);
            if (user == null)
                return NotFound(new { message = "User not found" });

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, data.Password);

            if (!result.Succeeded)
            return BusinessError("Unable to set password", new { issues = result.Errors.Select(e => e.Description).ToArray() });

            return Ok(new { message = "Password has been set", email = user.UserName });
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordData model)
        {
            var user = await _userManager.FindByIdAsync(model.Uid);
            if (user == null)
                return NotFound(new { message = "User w not found" });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BusinessError("Password change failed", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Password has been changed successfully" });
        }


        [HttpPut]
        [Route("user")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound(new { message = "User acocunt not found" });

            user.Fullname = model.Fullname;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BusinessError("Unable to save changes", new { issues = result.Errors.Select(e => e.Description).ToArray() });

            await _userManager.RemoveFromRolesAsync(user, new[] { "administrator", "user" });
            if (model.Roles != null && model.Roles.Any())
                await _userManager.AddToRolesAsync(user, model.Roles);
            if (model.Role != null)
                await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = "Changes have been saved successfully" });
        }

        public ObjectResult BusinessError(string description,object value)
        {
            return StatusCode(409, new { result = description, details = value });
        }
    }

    public class EmailVerificationData
    {
        public string Uid { get; set; }
        public string Code { get; set; }
    }
    public class SendActivationEmailData
    {
        public string Uid { get; set; }
        public string CallbackUrl { get; set; }
    }
    public class SetPasswordData
    {
        public string Uid { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPasswordData
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class ResetPasswordData
    {
        public string Uid { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }

    public class ChangePasswordData
    {
        public string  Uid { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}