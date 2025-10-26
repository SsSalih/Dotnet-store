using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountResetPaswordModel()
{
    public string Token { get; set; } = null!;
    public string Email { get; set; }= null!;
    
    
    [Required]
    [Display(Name = "Yeni Şifre")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage="Parola Eşleşmiyor")]
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "Yeni Şifre Tekrar")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage="Parola Eşleşmiyor")]
    public string ConfirmPassword { get; set; } = null!;
}