using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models
{
    public class KategoriCreteModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Kategori Adı")]
        public string KategoriAdi { get; set; } = null!;
        
        [Display(Name = "Url")]
        [Required]
        [StringLength(30)]
        public string Url { get; set; } = null!;
    }
} 