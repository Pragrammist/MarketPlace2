using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models
{
    public class StartInitialization
    {
        public static async Task InitializateDb(UserManager<UserModel> usersManager, RoleManager<UserRole> rolesManager)
        {
            var resRole = await rolesManager.CreateAsync(new UserRole { Name = "user" });
            var resRole1 = await rolesManager.CreateAsync(new UserRole { Name = "admin" });
            var resRole2 = await rolesManager.CreateAsync(new UserRole { Name = "moderator" });

            UserModel userModel = new UserModel();
            userModel.Email = "a@a.ru";
            userModel.UserName = "admin";
            string password = "admin";


            var res = await usersManager.CreateAsync(userModel, password);
            if (res.Succeeded)
                await usersManager.AddToRoleAsync(userModel, "admin");
        }
    }
}
