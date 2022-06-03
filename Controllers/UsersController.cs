using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MarketPlace.Models.ImagesModels;


namespace MarketPlace.Controllers
{
    public class UsersController : Controller
    {
        readonly UserManager<UserModel> _uM;
        readonly RoleManager<UserRole> _rM;
        readonly SignInManager<UserModel> _sM;
        public UsersController(UserManager<UserModel> uM, RoleManager<UserRole> rM, SignInManager<UserModel> sM)
        {
            _uM = uM;
            _rM = rM;
            _sM = sM;
        }
        public IActionResult Register()
        {
           
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegUserViewModel user)
        {
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                UserModel m = new UserModel();
                m.Email = user.Email;
                m.UserName = user.Name;
                var rCreate = await _uM.CreateAsync(m, user.Password);
                
                if (rCreate.Succeeded)
                {

                    var claims = GetClaims(m);
                    await _sM.SignInWithClaimsAsync(m, user.IsRemember, claims);
                    await _uM.AddToRoleAsync(m, "user");

                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    foreach (var error in rCreate.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
            }
            else
            {
                return View(user);
            }
        }

        
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginUserViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel user)
        {
            if (user != null)
            {
                RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var model = await _uM.FindByEmailAsync(user.Email) ?? new UserModel();
                var result =
                await _sM.CheckPasswordSignInAsync(model, user.Password, false);
                if (result.Succeeded)
                {
                    var claims = GetClaims(model);
                    await _sM.SignInWithClaimsAsync(model, user.IsRemember, claims);
                    if (!string.IsNullOrEmpty(user.ReturnUrl) && Url.IsLocalUrl(user.ReturnUrl))
                    {
                        return Redirect(user.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    return View(user);
                }

            }
            else
            {
                return View(user);
            }
            
        }

        [Authorize]
        public IActionResult Logout()
        {
            _sM.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel editUser)
        {

            if (editUser == null)
            {
                return NotFound();
            }

            var currentUser = await _uM.FindByIdAsync(editUser.Id);
            if (currentUser == null)
            {
                return NotFound();
            }
            //var changed = await ChangePassword(currentUser, editUser.CurrentPassword, editUser.NewPasword);

           

            {
                currentUser.UserName = editUser.NewName ?? currentUser.UserName;
                currentUser.Email = editUser.NewEmail ?? currentUser.Email;
                if (editUser.Avatar != null)
                {
                    var formFile = editUser.Avatar;
                    ImageStreamModel image = new ImageStreamModel();
                    image.Name = formFile.FileName;
                    image.Extention = formFile.ContentType;
                    var stream = new MemoryStream();
                    await formFile.CopyToAsync(stream);
                    image.Stream = stream; 
                }
            }

            var res = await _uM.UpdateAsync(currentUser);

            if(res.Succeeded)
                return Ok();

            return NotFound();
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var find = await _uM.FindByIdAsync(id);

            if (find == null)
            {
                return NotFound();
            }
            var res = await _uM.DeleteAsync(find);
            if (res.Succeeded)
            {
                return Ok();
            }
            return NotFound();
        }

        IEnumerable<Claim> GetClaims(UserModel user)
        {
            
            List<Claim> claims = new List<Claim> 
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName), 
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                new Claim("email", user.Email)
            };
            

            return claims;
        }

        [HttpPost]
        public async Task<bool> ChangePassword(UserModel user, string newPass, string currentPass)
        {


            if (newPass != null && newPass != null && currentPass != null && user != null)
            {
                var res = await _uM.ChangePasswordAsync(user, currentPass, newPass);
                return res.Succeeded;
            }
            return false;
        }
    }
}
