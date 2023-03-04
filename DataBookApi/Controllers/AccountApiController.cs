using DataBookApi.Authentification;
using DataBookApi.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyWebADataBookApipi.Authentification;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DataBookApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAccount _account;

        public AccountApiController(UserManager<User> userManager, IAccount account)
        {
            _userManager = userManager;
            _account = account;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var loginResult = await _account.LoginResultIsSucceed(userLogin.LoginProp, userLogin.Password);

            if (loginResult)
            {
                var roleResult = await _account.RoleChecker(userLogin.LoginProp);
                return new ObjectResult(GenerateToken(userLogin.LoginProp, roleResult));
            }
            else
                return BadRequest();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration userRegistration)
        {
            var user = new User { UserName = userRegistration.LoginProp };
            var createResult = await _userManager.CreateAsync(user, userRegistration.Password);

            if (createResult.Succeeded)
            {
                return new ObjectResult(GenerateToken(userRegistration.LoginProp, new List<string>()));
            }
            else
                return BadRequest();
        }

        private string GenerateToken(string username, IEnumerable<string> roles)
        {
            List<Claim> claimsToToken = new List<Claim> { new Claim(ClaimTypes.Name, username) };

            foreach (var role in roles)
                claimsToToken.Add(new Claim(ClaimTypes.Role, role));

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claimsToToken,                
                expires: new DateTimeOffset(DateTime.Now.AddMinutes(60)).DateTime,
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
