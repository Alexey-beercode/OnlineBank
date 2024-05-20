using System.ComponentModel.DataAnnotations;

namespace OnlineBank.Data.ViewModel;

public class RegisterViewModel
{
    [MinLength(4, ErrorMessage = "Слишком короткий логин: минимальная длина 4 символа")]
    [Required(ErrorMessage = "Необходимо заполнить поле для логина")]
    public string Login { get; set; }

    [MinLength(4, ErrorMessage = "Слишком короткий пароль: минимальная длина 5 символов")]
    [Required(ErrorMessage = "Необходимо заполнить поле для пароля")]
    public string Password { get; set; }
}