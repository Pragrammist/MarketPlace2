using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.Models.UsersModels
{
    public class UserViewModel
    {        
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public int IdIcon { get; set; }
    }
}
