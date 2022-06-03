using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Controllers
{
    public class RolesController : Controller
    {
        UserManager<UserModel> _uM;
        RoleManager<UserRole> _rM;
        SignInManager<UserModel> _sM;
        public RolesController(UserManager<UserModel> uM, RoleManager<UserRole> rM, SignInManager<UserModel> sM)
        {
            _uM = uM;
            _rM = rM;
            _sM = sM;
        }
        [HttpPost]
        public async Task<IActionResult> EditUserRole(string userId, List<string> roles)
        {
            var user = await _uM.FindByIdAsync(userId);

            if (user != null && roles is not null && roles.Count > 1)
            {
                //var allRoles = _rM.Roles.ToList();
                var userRoles = await _uM.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _uM.RemoveFromRolesAsync(user, removedRoles);
                await _uM.AddToRolesAsync(user, addedRoles);
                return Ok();
            }
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(EditRoleViewModel role)
        {
            if (role == null)
            {
                return NotFound();
            }

            var newRole = new UserRole {Name = role.Name};
            var res = await _rM.CreateAsync(newRole);
            if (res.Succeeded)
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(EditRoleViewModel role)
        {
            if (role == null)
            {
                return NotFound();
            }

            var dRole = await _rM.FindByNameAsync(role.Name);
            if (dRole is not null)
            {
                var res = await _rM.DeleteAsync(dRole);
                if (res.Succeeded)
                {
                    return Ok();
                }
            }
            return NotFound();
        }
        
    }
}
