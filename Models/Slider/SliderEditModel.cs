using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class SliderEditModel
{
    
    public int Id { get; set; }
    public string Baslik { get; set; } = null!;
    public string? Aciklama { get; set; }
    public IFormFile? ResimDosyasi { get; set; }
    public string? ResimAdi { get; set; }
    public int Index { get; set; }
    public bool Aktif { get; set; }
}