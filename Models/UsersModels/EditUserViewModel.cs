using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MarketPlace.Models.UsersModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string NewPasword { get; set; }
        public string NewName { get; set; }
        public string NewEmail { get; set; }
        public string CurrentPassword { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
