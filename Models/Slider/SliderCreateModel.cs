using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class SliderCreatModel
{
    
    public string Baslik { get; set; } = null!;

    
    public string? Aciklama { get; set; }
    
    public IFormFile Resim { get; set; } = null!;
    public int Index { get; set; }
    public bool Aktif { get; set; }
}