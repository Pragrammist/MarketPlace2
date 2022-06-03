using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarketPlace.Models.UsersModels;
using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Services.Validators
{
    public class OwnPasswordValidator : IPasswordValidator<UserModel>
    {
        public int RequiredLength { get; set; } = 4;  // минимальная длина


        public Task<IdentityResult> ValidateAsync(UserManager<UserModel> manager, UserModel user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (String.IsNullOrEmpty(password) || password.Length < RequiredLength)
            {
                errors.Add(new IdentityError
                {
                    Description = $"Минимальная длина пароля равна {RequiredLength}"
                });
            }
            

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
