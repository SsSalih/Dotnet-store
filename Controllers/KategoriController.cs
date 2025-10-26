using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

[Authorize(Roles = "Admin")]
public class KategoriController : Controller
{
    private readonly DataContext _context;

    public KategoriController(DataContext context)
    {
        _context = context;
    }


    public ActionResult Index()
    {
        var categories = _context.Kategoriler.Select(x => new KategoriGetModel
        {
            Id = x.Id,
            KategoriAdi = x.KategoriAdi,
            Url = x.Url,
            UrunSayisi = x.Uruns.Count() // sadece içerisnde tuttuğu veri miktarına yüklüycem işlem kalabalığının önüne geçecek
        }).ToList();
        return View(categories);
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(KategoriCreteModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var kategoriAdi = model.KategoriAdi;
        var kategoriUrl = model.Url;

        // Kategori adı ve URL boş olmamalı
        if (string.IsNullOrEmpty(kategoriAdi) || string.IsNullOrEmpty(kategoriUrl))
        {
            ModelState.AddModelError("", "Kategori adı ve URL boş olamaz.");
            return View(model);
        }

        // Kategori adı ve URL zaten mevcut mu kontrol et
        if (_context.Kategoriler.Any(x => x.KategoriAdi == kategoriAdi || x.Url == kategoriUrl))
        {
            ModelState.AddModelError("", "Bu kategori adı veya URL zaten mevcut.");
            return View(model);
        }

        var entity = new Kategori
        {
            KategoriAdi = model.KategoriAdi,
            Url = model.Url
        };

        _context.Kategoriler.Add(entity);
        _context.SaveChanges();
        return RedirectToAction("Index");

    }

    public ActionResult Edit(int id)
    {
        var entity = _context.Kategoriler.Select(x => new KategoriEditModel
        {
            Id = x.Id,
            KategoriAdi = x.KategoriAdi,
            Url = x.Url
        }).FirstOrDefault(x => x.Id == id);

        return View(entity);
    }

    [HttpPost]
    public ActionResult Edit(int id, KategoriEditModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }


        var entity = _context.Kategoriler.FirstOrDefault(x => x.Id == model.Id);

        if (entity != null)
        {
            entity.KategoriAdi = model.KategoriAdi;
            entity.Url = model.Url;

            _context.SaveChanges();

            TempData["Message"] = $"{model.KategoriAdi} başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
        else
        {
            return NotFound();
        }

    }

    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Kategoriler.FirstOrDefault(i => i.Id == id);
        if (entity != null)
        {
            return View(entity);
        }
        return RedirectToAction("Index");


    }

    [HttpPost]
    public ActionResult Deleteconfirm(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = _context.Kategoriler.FirstOrDefault(i => i.Id == id);
        if (entity != null)
        {
            _context.Kategoriler.Remove(entity);
            _context.SaveChanges();

            TempData["Message"] = $"{entity.KategoriAdi} başarıyla Silindi.";
        }
        return RedirectToAction("Index");
    }

}