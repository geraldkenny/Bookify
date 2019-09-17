using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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


        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email, };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorDescription = "Your Email or Password is Incorrect"
                });
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                await _signInManager.SignInAsync(user, isPersistent: false);
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
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var jwt =  BuildToken(model);
                    //var jwt = await GenerateEncodedToken(model.Email, identity);

                    return Ok(new LoginResponse
                    {
                        Token = jwt
                    });
                }
                var identity = await GetClaimsIdentity(model.Email, model.Password);

                if (identity == null)
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

        private string BuildToken(LoginModel user)
        {
            var claims = new[] {
                   // new Claim(JwtRegisteredClaimNames.Sub, user.),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    //new Claim(ClaimTypes.Role, "Admin"),

                   // new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
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

        private async Task<ClaimsIdentity> GetClaimsIdentity(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByEmailAsync(email);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);

                //return await Task.FromResult(GenerateClaimsIdentity(email, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        //public ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
        //{
        //    return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
        //    {
        //        new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id),
        //        new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.ApiAccess)
        //    });
        //}

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Email, userName),
                 new Claim(JwtRegisteredClaimNames.Sub, userName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64),

            };

            // Create the JWT security token and encode it.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
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