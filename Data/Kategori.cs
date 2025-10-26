namespace dotnet_store.Models;

public class Kategori
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;
    public List<Urun> Uruns { get; set; } = new(); //navigation propperty iki tablonun kesişimi liste şeklinde gelir
}