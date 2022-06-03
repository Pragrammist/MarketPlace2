using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MarketPlace.Models.UsersModels
{
    public class UserModel : IdentityUser
    {
        public int IconId { get; set; }
    }
}
