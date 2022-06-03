using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPlace.Models.UsersModels
{
    public class RegUserViewModel : UserViewModel
    {
        [Required(ErrorMessage = "Кровь хранить не сервере еще не научились")]
        [Display(Name = "Супер секретный пароль")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина пароля может быть от 3 до 50 символов")]
        public override string Password { get; set; }

        [Required(ErrorMessage = "Голубинная почта будет добавленна позже")]
        [Display(Name = "Почту позжалуйтса")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина почты может быть от 3 до 50 символов")]
        [EmailAddress(ErrorMessage = "Неверная почта")]
        public override string Email { get; set; }

        [Required(ErrorMessage = "Кто-то пересмотрел нет игры нет жизни")]
        [Display(Name = "Придумайте псевдоним")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина имени может быть от 3 до 20 символов")]
        public override string Name { get; set; }
        
        [Compare("Password", ErrorMessage = "пароли не совпадают")]
        [Display(Name = "Повтор пороля")]
        [Required(ErrorMessage = "повторите пожалуйста")]
        [StringLength(50, MinimumLength = 3)]
        public string RepeatPassword { get; set; }

        
        public bool IsRemember { get; set; } = false;
    }
}
