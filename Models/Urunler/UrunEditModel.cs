using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class UrunEditModel
{
    public int Id { get; set; }

    [Display(Name = "Ürün Adi")]
    public string UrunAdi { get; set; } = null!;

    [Display(Name = "Ürün Fiyatı")]
    public double Fiyat { get; set; }
    public string? ResimAdi { get; set; }
    public IFormFile? ResimDosyasi { get; set; }
    
    [Display(Name = "Ürün Aciklama")]
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa{ get; set; }
    public int KategoriId { get; set; }
    
}
