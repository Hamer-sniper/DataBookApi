using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DataBookApi.Roles;
using DataBookApi.Authentification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Rewrite;
using System.Xml.Linq;

namespace DataBookApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesApiController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;

        public RolesApiController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public List<IdentityRole> GetRoles()
        {
            return _roleManager.Roles.ToList();
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return new ObjectResult(name);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return BadRequest();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            IdentityRole role = await _roleManager.FindByNameAsync(name);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                return new ObjectResult(name);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetUserAndRoles/{name}")]
        public async Task<IActionResult> GetUserAndRoles(string name)
        {
            // получаем пользователя
            User user = await _userManager.FindByNameAsync(name);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRole model = new ChangeRole
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    AllRoles = allRoles,
                    UserRoles = userRoles
                };
                return new ObjectResult(model);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("EditUserAndRoles/{name}")]
        public async Task<IActionResult> EditUserAndRoles(string name, [FromBody] List<string> roles)
        {
            // получаем пользователя
            User user = await _userManager.FindByNameAsync(name);

            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return new ObjectResult(roles);
            }

            return NotFound();
        }
    }
}
