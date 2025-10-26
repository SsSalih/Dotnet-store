using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class UrunCreateModel
{
    
    [Required(ErrorMessage = "Ürün adı girmelisiniz")]
    [Display(Name = "Ürün Adi")]
    public string UrunAdi { get; set; } = null!;
    

    [Display(Name = "Ürün Fiyatı")]
    public double Fiyat { get; set; }
    public IFormFile? Resim { get; set; }
    
    [Display(Name = "Ürün Aciklama")]
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa{ get; set; }
    public int KategoriId { get; set; }
    
}
