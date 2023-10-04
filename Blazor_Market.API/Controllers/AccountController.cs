using Blazor_Market.API.Model;
using Blazor_Market.API.Model.AccountModel;
using Blazor_Market.API.Static;
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
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
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
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description);

                    return BadRequest(ModelState);
                }
                await _userManager.AddToRoleAsync(user, "User");
            }
            return Ok("User Registration success!!");
        }


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> login([FromBody] LoginModel loginModel)
        {
            var user =await _userManager.FindByEmailAsync(loginModel.Email!);
            var passwordValid = await _userManager.CheckPasswordAsync(user!, loginModel.Password!);
            if (user == null || passwordValid==false) 
            {
                return Unauthorized();
            }
            var tokenString = await GeneratedToken(user);
            var response = new LoginResponse
            {
                Email = loginModel.Email,
                Token = tokenString,
                UserId = user.Id,
            };
            return Ok(response);
        }

        
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new { message = "Logout successful" });
        }

        private async Task<string> GeneratedToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials= new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims= await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(CustomClaimsTypes.Uid,user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["Jwt:Duration"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}