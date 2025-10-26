using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models
{
    public class UserCreateModel
    {
        [Required]
        [StringLength(30)]
        [Display(Name = "kullanıcı Adı")]
        public string AdSoyad { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")]

        public string Email { get; set; } = null!;
        


    }
}