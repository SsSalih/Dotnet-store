using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountChangePasword()
{
    
    [Required]
    [Display(Name = "Eski şifreyi giriniz")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required]
    [Display(Name = "Yeni Şifre")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword2", ErrorMessage="Parola Eşleşmiyor")]
    public string ConfirmPassword { get; set; } = null!;

    [Required]
    [Display(Name = "Yeni Şifre Tekrar")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage="Parola Eşleşmiyor")]
    public string ConfirmPassword2 { get; set; } = null!;
}