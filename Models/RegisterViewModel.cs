using System.ComponentModel.DataAnnotations;

namespace KampMVC.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "E-posta alanı boş olamaz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    //[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$",
       // ErrorMessage = "Şifre en az bir büyük ve bir küçük harf içermelidir.")]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
    public string ConfirmPassword { get; set; }
}