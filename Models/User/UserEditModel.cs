using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models
{
    public class UserEditModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "kullanıcı Adı")]
        public string AdSoyad { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")] 

        public string Email { get; set; } = null!;


        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Şifre tekrar")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parola Eşleşmiyor")]
        public string? ConfirmPassword { get; set; }

        public IList<string>? SelectedRoles { get; set; }
        
    
    }
} 