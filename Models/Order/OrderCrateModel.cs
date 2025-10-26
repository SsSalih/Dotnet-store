namespace dotnet_store.Models;

public class OrderCrateModel
{
    public string AdSoyad { get; set; } = null!;
    public string Sehir { get; set; } = null!;
    public string AdresSatiri { get; set; } = null!;
    public string PostaKodu { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public string? SiparişNotu { get; set; }

    public string CartName { get; set; } = null!;
    public string CartNumber { get; set; } = null!;
    public string CartExpirationYear { get; set; } = null!;
    public string CartExpirationMonth { get; set; } = null!;
    public string CartCVV { get; set; } = null!;
    
}