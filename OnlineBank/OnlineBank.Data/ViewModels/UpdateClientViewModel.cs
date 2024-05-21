using System.ComponentModel.DataAnnotations;

namespace OnlineBank.Data.ViewModels;

public class UpdateClientViewModel
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
    
    public Guid Id { get; set; }
}