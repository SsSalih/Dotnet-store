using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountCreateModel()
{
    [Required]
    [Display(Name = "Adı Soyad")]
    //[RegularExpression("^[0-9A-Za-z]{6,16}$",ErrorMessage="6-16 karakter arası olmalı")]
    public string AdSoyad { get; set; } = null!;
    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email  { get; set; } = null!;
    [Required]
    [Display(Name = "Yeni Şifre")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required]
    [Display(Name = "Yeni Şifre Tekrar")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage="Parola Eşleşmiyor")]
    public string ConfirmPassword { get; set; } = null!;
}