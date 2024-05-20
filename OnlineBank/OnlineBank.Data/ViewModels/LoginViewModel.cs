using System.ComponentModel.DataAnnotations;

namespace OnlineBank.Data.ViewModel;

public class LoginViewModel
{
    [Required(ErrorMessage = "Необходимо заполнить поле для логина")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Необходимо заполнить поле для пароля")]
    public string Password { get; set; }
}