namespace dotnet_store.Models;

public class KategoriGetModel
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int UrunSayisi { get; set; } // sadece içerisnde tuttuğu veri miktarına yüklüycem işlem kalabalığının önüne geçecek
}