using Blazor_Market.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blazor_Market.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new UserModel
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                UserName = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber,
            };
            if (user != null && registerModel.Password != null)
            {
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new { token = token });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            // Handle any unexpected conditions or errors
            return BadRequest("An error occurred while registering.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody]LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (loginModel.Email != null && loginModel.Password != null)
            {
                var user = await _userManager.FindByNameAsync(loginModel.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
                {
                    // User successfully logged in

                    // Generate JWT token
                    var token = GenerateJwtToken(user);

                    return Ok(new { Token = token });
                }
            }
                // Login failed
                return Unauthorized("Invalid credentials");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logout successful" });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];
            if (key != null)
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),

                    // Use keyBytes instead of key here
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            else
            {
                throw new InvalidOperationException("Jwt:Key is not configured");
            }
        }

    }
}
