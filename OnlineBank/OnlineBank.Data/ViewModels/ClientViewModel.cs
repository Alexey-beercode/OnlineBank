using System.ComponentModel.DataAnnotations;

namespace OnlineBank.Data.ViewModel;

public class ClientViewModel
{
    [Required(ErrorMessage = "Имя обязательно.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Фамилия обязательна.")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Дата рождения обязательна.")]
    [DataType(DataType.Date, ErrorMessage = "Некорректный формат даты.")]
    public DateTime BirthDay { get; set; }

    [Required(ErrorMessage = "Адрес обязателен.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Номер телефона обязателен.")]
    [RegularExpression(@"^\+[0-9]{1,3}\([0-9]{3}\)[0-9]{3}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "Некорректный формат номера телефона. Пример: +7(123)456-78-90")]
    public string Phone { get; set; }
}