using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bookify.Repositories.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Bookify.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;



        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IUnitOfWork unitOfWork,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _unitOfWork = unitOfWork;

        }

        /// <summary>
        /// creates users
        /// </summary>
        /// <remarks>Only authorized for everyone!</remarks>
        /// <response code="200">Users created</response>
        // POST: api/Account

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email, };
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Your Email or Password is Incorrect"
                });
            }
            else
            {
                var user = new User { CreatedAt = DateTime.Now, FirstName = model.FirstName, LastName = model.LastName, EmailAddress = model.Email, UserName = model.Email };
                await _unitOfWork.User.CreateUserAsync(user);
                await _userManager.AddToRoleAsync(identityUser, "Admin");
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
            }
            return Ok(new RegisterResponse
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,

            });
        }

        /// <summary>
        /// creates admin users
        /// </summary>
        /// <remarks>Only authorized for everyone!</remarks>
        /// <response code="200">Users created</response>
        // POST: api/Account

        [AllowAnonymous]
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identityUser = new IdentityUser { UserName = model.Email, Email = model.Email, };
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Your Email or Password is Incorrect"
                });
            }
            else
            {
                var user = new User { CreatedAt = DateTime.Now, FirstName = model.FirstName, LastName = model.LastName, EmailAddress = model.Email, UserName = model.Email };
                await _unitOfWork.User.CreateUserAsync(user);
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
            }
            return Ok(new RegisterResponse
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,

            });
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var userTask = _userManager.FindByEmailAsync(model.Email);
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await userTask;
                    var userRole = await _userManager.IsInRoleAsync(user, "Admin");
                    var jwt =  BuildToken(model, userRole);
                    return Ok(new LoginResponse
                    {
                        Token = jwt
                    });
                }
                else 
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorDescription = "Your Email or Password is Incorrect"
                    });
                }
               
            }

            return BadRequest(new ErrorResponse
            {
                ErrorDescription = "Your Email or Password is Incorrect"
            });

        }

        private string BuildToken(LoginModel user, bool isAdmin)
        {
            var userRole = (!isAdmin) ? "User" : "Admin";
            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     new Claim(ClaimTypes.Role, userRole),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        public class LoginResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }
        }


        public class LoginModel
        {
            [Required]
#if DEBUG
            [DefaultValue("dev@mobileforms.co")]
#endif
            [JsonProperty("email")]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [JsonProperty("password")]
#if DEBUG
            [DefaultValue("Dev@12345")]
#endif
            public string Password { get; set; }

        }

        public class RegisterModel
        {

            [Required]
#if DEBUG
            [DefaultValue("dev@mobileforms.co")]
#endif
            [JsonProperty("email")]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [Required]
#if DEBUG
            [DefaultValue("Dev@12345")]
#endif
            [JsonProperty("password")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "Password must be at least 8 characters " +
                "and must contain at least 1 uppercase letter, 1 lowercase letter, 1 numerical value and 1 special character")]
            public string Password { get; set; }

            [Required]
#if DEBUG
            [DefaultValue("Dev@12345")]
#endif
            [JsonProperty("confirm_password")]
            [DataType(DataType.Password)]
            [Compare(nameof(Password))]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "Password must be at least 8 characters " +
                "and must contain at least 1 uppercase letter, 1 lowercase letter, 1 numerical value and 1 special character")]
            public string ConfirmPassword { get; set; }

            [Required]
#if DEBUG
            [DefaultValue("Developer")]
#endif
            [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Please enter a valid name with bwtween 3 - 10 letters")]
            [JsonProperty("first_name")]
            public string FirstName { get; set; }


            [Required]
#if DEBUG
            [DefaultValue("Mobile Forms")]
#endif
            [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Please enter a valid name with bwtween 3 - 10 letters")]
            [JsonProperty("last_name")]
            public string LastName { get; set; }


            [Required]
#if DEBUG
            [DefaultValue("09060697346")]
#endif
            [JsonProperty("phone_number")]
            //[RegularExpression("/^\\s*(?:\\+?(\\d{1,3}))?([-. (]*(\\d{3})[-. )]*)?((\\d{3})[-. ]*(\\d{2,4})(?:[-.x ]*(\\d+))?)\\s*$/gm")]
            public string PhoneNumber { get; set; }
        }

        public class ErrorResponse
        {
            [JsonProperty("error_description")]
            public string ErrorDescription { get; set; } = "An Error Occured";
        }

        public class RegisterResponse
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("first_name")]
            public string FirstName { get; set; }

            [JsonProperty("last_name")]
            public string LastName { get; set; }
        }



        public IActionResult Index()
        {
            return Ok();
        }
    }
}