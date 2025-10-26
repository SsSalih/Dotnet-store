namespace dotnet_store.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime SiparişTarihi { get; set; }
    public string AdSoyad { get; set; } = null!;
    public string Username { get; set; } = null!;

    public string Sehir { get; set; } = null!;
    public string AdresSatiri { get; set; } = null!;
    public string PostaKodu { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public double ToplamFiyat { get; set; }
    public string? SiparişNotu { get; set; }
    public List<OrderItem> OrderItem { get; set; } = new();
    
     public double AraToplam()
    {
        return OrderItem.Sum(i => i.Urun.Fiyat * i.Miktar);
    }

    public double Toplam()
    {
        return OrderItem.Sum(i => i.Urun.Fiyat * i.Miktar) * 1.2;
    }
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public Order Order{ get; set; } = null!;
    public int UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
    public double Fiyat { get; set; }
    public int Miktar { get; set; }
}