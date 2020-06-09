using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using vectio.eventmanagement.api.db.entities;
using vectio.eventmanagement.api.helpers;
using vectio.eventmanagement.api.models;

namespace vectio.eventmanagement.api.controllers
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
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Firstname),
                    new Claim("roles", string.Join(",",roles))
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("81251570-7113-4D0A-AD50-6A3499643432"));

                var token = new JwtSecurityToken(
                    issuer: "https://alumni.vectio.pl",
                    audience: "https://alumni.vectio.pl",
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
                    firstname = user.Firstname,
                    lastname = user.Lastname,
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
            var users = await _userManager.Users.OrderBy(u => u.Email).Select(user => new
            {
                id = user.Id,
                email = user.Email,
                firstname = user.Firstname,
                lastname = user.Lastname,
                emailConfirmed = user.EmailConfirmed
            }).ToListAsync();

            return Ok(new
            {
                users
            });
        }

        [HttpPost]
        [Route("createUser")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
            };
            var result = await _userManager.CreateAsync(user, model.Password ?? "QAZ!@#qaz123");

            if (!result.Succeeded)
                return BusinessError("Nie można utworzyć użytkownika", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            if (model.Roles != null && model.Roles.Any())
                await _userManager.AddToRolesAsync(user, model.Roles);
            if (model.Role != null)
                await _userManager.AddToRoleAsync(user, model.Role);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = model.ConfirmationUrl + $"?uid={user.Id}&code={code}";
            var x = _emailHelper.SendEmail(user.Email, "[VBP EMBA] - Konto utworzone", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Państwa  konto zostało stworzone w systemie {1}.<br/><br/>
    Link do aktywacji konta: <a href='{0}'>Aktywuj konto</a><br/><br/>Link zachowuje ważność przez 7 dni.
    <br/>", callbackUrl, "[VBP - EMBA]"));

            return CreatedAtAction(nameof(GetUser), new { id = user.Id, message = $"Potwierdzenie zostało wysałne na adres: {user.Email}." }, new
            {
                Id = user.Id
            });
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password ?? "QAZ!@#qaz123");

            if (!result.Succeeded)
                return BusinessError("Nie można utworzyć użytkownika", new { issues = result.Errors.Select(e => e.Description).ToArray() });

            await _userManager.AddToRoleAsync(user, "user");
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = model.ConfirmationUrl + $"?uid={user.Id}&code={code}";
            var x = _emailHelper.SendEmail(user.Email, "[VBP EMBA] - Konto utworzone", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Państwa  konto zostało stworzone w systemie {1}.<br/><br/>
    Link do aktywacji konta: <a href='{0}'>Aktywuj konto</a><br/><br/>Link zachowuje ważność przez 7 dni.
    <br/>", callbackUrl, "[VBP - EMBA]"));

            return CreatedAtAction(nameof(GetUser), new { id = user.Id, message = $"Potwierdzenie zostało wysałne na adres: {user.Email}." }, new
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
                return NotFound(new { message = "Użytkonik nie znaleziony" });
            if (user.EmailConfirmed)
                user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = model.CallbackUrl + $"?uid={user.Id}&code={code}";
            var x = _emailHelper.SendEmail(user.Email, "[VBP EMBA] - Aktywacja konta", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Państwa  konto zostało stworzone w systemie {1}.<br/><br/>
    Link do aktywacji konta: <a href='{0}'>Aktywuj konto</a><br/><br/>Link zachowuje ważność przez 7 dni.
    <br/>", callbackUrl, "[VBP - EMBA]"));
            return Ok(new { message = "Account activation email has been sent successfully" });
        }

        [HttpPost]
        [Route("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailVerificationData data)
        {
            var user = await _userManager.FindByIdAsync(data.Uid);
            if (user == null)
                return NotFound(new { message = "Użytkonik nie znaleziony" });
            if (user.EmailConfirmed)
                return BusinessError("Ten adres mailowy jest już aktywny", new { issues = new[] { "Zaloguj się" } });

            var result = await _userManager.ConfirmEmailAsync(user, data.Code);
            if (!result.Succeeded)
                return BusinessError("Adres mailowy nie aktywowany", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Adres mailowy aktywowany" });
        }

        [HttpPost]
        [Route("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordData model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
                return NotFound(new { message = "Użytkownik o podanym adresie mailowym nie znaleziony" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = model.CallbackUrl + $"?uid={user.Id}&code={token}";
            var x = _emailHelper.SendEmail(user.Email, "[VBP EMBA] - Resetowanie hasła", string.Format(@"Szanowni Państwo,
    <br/><br/>
    Otrzymaliśmy żądanie zmiany hasła do państwa konta w systemie {1}.<br/><br/>
    Link do zmiany hasła:<br/> <a href='{0}'>Zmień hasło</a>
    <br/>", callbackUrl, "[VBP - EMBA]"));


            return Ok(new { message = "Mail z linkiem do zmiany hasła został wysłany" });
        }

        [HttpPost]
        [Route("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordData model)
        {
            var user = await _userManager.FindByIdAsync(model.Uid);
            if (user == null)
                return NotFound(new { message = "Użytkonik nie znaleziony" });
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
                return BusinessError("Hasło nie zostało zmienione", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Hasło zostało zmienione" });
        }

        [HttpPost]
        [Route("setPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordData data)
        {
            var user = await _userManager.FindByIdAsync(data.Uid);
            if (user == null)
                return NotFound(new { message = "Użytkonik nie znaleziony" });

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, data.Password);

            if (!result.Succeeded)
                return BusinessError("Hasło nie zostało zmienione", new { issues = result.Errors.Select(e => e.Description).ToArray() });

            return Ok(new { message = "Hasło zostało zmienione", email = user.UserName });
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordData model)
        {
            var user = await _userManager.FindByIdAsync(model.Uid);
            if (user == null)
                return NotFound(new { message = "Użytkonik nie znaleziony" });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BusinessError("Hasło nie zostało zmienione", new { issues = result.Errors.Select(e => e.Description).ToArray() });
            return Ok(new { message = "Hasło zostało zmienioney" });
        }


        [HttpPut]
        [Route("user")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound(new { message = "Użytkonik nie znaleziony" });

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BusinessError("Zmiany nie zostały zapisane", new { issues = result.Errors.Select(e => e.Description).ToArray() });

            await _userManager.RemoveFromRolesAsync(user, new[] { "administrator", "user" });
            if (model.Roles != null && model.Roles.Any())
                await _userManager.AddToRolesAsync(user, model.Roles);
            if (model.Role != null)
                await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = "Zmiany zostały zapisane" });
        }

        public ObjectResult BusinessError(string description, object value)
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
        public string Uid { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
