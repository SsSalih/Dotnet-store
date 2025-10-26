using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;
    [Authorize(Roles ="Admin")]    
public class AdminUrunController : Controller
{
    private readonly DataContext _context;
    public AdminUrunController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index(int? kategori)
    {
        var query = _context.Urunler.AsQueryable();

        if(kategori != null)
        {
            query = query.Where(i =>i.KategoriId ==kategori);
        }
        // var urunler = _context.Urunler.Include(i => i.Kategori).ToList();  // include sayesinde kategori tablosunu da ekliyoruz
        // return View(urunler); // listeyi view'e gönder




        var urunler = query.Select(x => new UrunGetModel
        {
            Id = x.Id,
            UrunAdi = x.UrunAdi,
            Fiyat = x.Fiyat,
            Resim = x.Resim,
            Aktif = x.Aktif,
            Anasayfa = x.Anasayfa,
            KategoriAdi = x.Kategori.KategoriAdi // Kategori tablosundan kategori adını al
        }).ToList(); // Listeyi oluştur

        ViewBag.kategori = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi",kategori); // Kategori listesini dropdown için hazırla
        return View(urunler); // Listeyi view'e gönder


    }
     
    public ActionResult Create()
    {
        // ViewBag.kategoriler = _context.Kategoriler.ToList();
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi"); // Kategori listesini dropdown için hazırla

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(UrunCreateModel model)
    {
        if (ModelState.IsValid)
        {

            var fileName = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.Resim!.CopyToAsync(stream);
            }



            var entity = new Urun
            {
                UrunAdi = model.UrunAdi,
                Fiyat = model.Fiyat,
                Aciklama = model.Aciklama,
                Aktif = model.Aktif,
                Anasayfa = model.Anasayfa,
                KategoriId = model.KategoriId,
                Resim = fileName


            };
            _context.Urunler.Add(entity); // Yeni ürünü ekle
            _context.SaveChanges(); // Değişiklikleri kaydet
            return RedirectToAction("Index"); // İşlem tamamlandıktan sonra listeye yönlendir
        }
        ViewBag.kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi"); // Kategori listesini dropdown için hazırla
        return View(model);
    }

    public ActionResult Edit(int id)
    {


        var entity = _context.Urunler.Select(x => new UrunEditModel
        {
            Id = x.Id,
            UrunAdi = x.UrunAdi,
            Fiyat = x.Fiyat,
            ResimAdi = x.Resim,
            Aciklama = x.Aciklama,
            Aktif = x.Aktif,
            Anasayfa = x.Anasayfa

        }).FirstOrDefault(x => x.Id == id);

        ViewBag.kategori = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi"); // Kategori listesini dropdown için hazırla
        return View(entity);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(int id, UrunEditModel model)
    {
        var entity = _context.Urunler.FirstOrDefault(x => x.Id == model.Id);

        if (model.ResimDosyasi != null)
        {

            var fileName = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.ResimDosyasi!.CopyToAsync(stream);
            }
            entity!.Resim = fileName;
        }

        if (id != model.Id)
        {
            return NotFound();
        }


        if (entity != null)
        {
            entity.UrunAdi = model.UrunAdi;
            entity.Fiyat = model.Fiyat;
            entity.Aciklama = model.Aciklama;
            entity.Aktif = model.Aktif;
            entity.Anasayfa = model.Anasayfa;

            _context.SaveChanges();

            TempData["Message"] = $"{model.UrunAdi} başarıyla güncellendi";

            return RedirectToAction("Index");
        }
        return View(model);
    }

    public ActionResult Delete(int? id)
    {
        if(id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);

        if (entity != null)
        {
            return View(entity);

        }
        return RedirectToAction("Index");

    }
    [HttpPost]
    public ActionResult Confirmdelete(int? id)
    {
        if(id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);

        if (entity != null)
        {
            _context.Urunler.Remove(entity);
            _context.SaveChanges();

            TempData["Message"] = $"{entity.UrunAdi} başarıyla silindi";

        }
        return RedirectToAction("Index");

    }


}