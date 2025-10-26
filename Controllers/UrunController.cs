using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class UrunController : Controller
{
    private readonly DataContext _context;
    public UrunController(DataContext context)
    {
        _context = context;
    }

    [Authorize(Roles ="Admin")]
    public ActionResult Index()
    {
        return View();
    }

    // route parametreleri için [FromRoute] kullanımı
    // q = query string
    // url = route parametresi
    // [FromRoute] kullanımı ile route parametrelerini alabiliriz
    public ActionResult List(string url, string q)
    {
        var query = _context.Urunler.Where(x => x.Aktif).AsQueryable(); //queryble türüne dönüştürüypruz yani boş bi sorgu yazıyoruz if içinde iki farklı biçimde dolduruyoruz wherede aynı işi görüyor tipi quareble yapıyor ama dursun diye silmedik
        if(!string.IsNullOrEmpty(url))
        {
            query = query.Where (i => i.Kategori.Url == url);
        }

        if(!string.IsNullOrEmpty(q))
        {
            query = query.Where (i => i.UrunAdi.ToLower().Contains(q.ToLower()) );
            ViewData["q"] = q; // arama kutusuna yazılan kelimeyi geri döndürmek için kullanıyoruz
        }
        //var urunler = _context.Urunler.Where(i => i.Aktif && i.Kategori.Url == url).ToList();

        return View(query.ToList()); //queryable türünde döndürüyoruz yani veritabanına sorgu atıyoruz ve listeye çeviriyoruz
    }

    public ActionResult Details(int id)
    {
        var urun = _context.Urunler.Find(id);

        if (urun == null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["BenzerUrunler"] = _context.Urunler
                                        .Where(i => i.Aktif && i.KategoriId == urun.KategoriId && i.Id != id)
                                        .Take(4)
                                        .ToList();

        return View(urun);
    }
}