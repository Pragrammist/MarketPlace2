using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Services.Validators
{
    public class UserValidator : IUserValidator<UserModel>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<UserModel> manager, UserModel user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
