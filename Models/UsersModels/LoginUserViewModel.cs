using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models.UsersModels
{
    public class LoginUserViewModel : UserViewModel
    {
        [Required(ErrorMessage = "Голубинная почта будет добавленна позже")]
        [Display(Name = "Почту позжалуйтса")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина почты может быть от 3 до 50 символов")]
        [EmailAddress(ErrorMessage = "Неверный формат почты")]
        public override string Email { get; set; }
        [Required(ErrorMessage = "Кровь хранить не сервере еще не научились")]
        [Display(Name = "Супер секретный пароль")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина пароля может быть от 3 до 50 символов")]
        
        public override string Password { get; set; }
        public bool IsRemember { get; set; } = false;
        public string ReturnUrl { get; set; }

    }
}
