using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;
    [Authorize(Roles ="Admin")]
public class SliderController : Controller
{
    private readonly DataContext _context;

    public SliderController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index()
    {
        var entity = _context.Sliderlar.Select(x => new SliderGetModel
        {
            Id = x.Id,
            Baslik = x.Baslik,
            Resim = x.Resim,
            Index = x.Index,
            Aktif = x.Aktif,
        }).ToList();

        return View(entity);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(SliderCreatModel model)
    {
        if(model.Resim == null || model.Resim.Length == 0)
        {
            ModelState.AddModelError("resim", "resim seçin");
        }
        if (ModelState.IsValid)
        {
            var fileName = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
            await model.Resim!.CopyToAsync(stream);
            }

            var entity = new Slider
            {
                Baslik = model.Baslik,
                Aciklama = model.Aciklama,
                Index = model.Index,
                Aktif = model.Aktif,
                Resim = fileName,
            };

            _context.Sliderlar.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");

        }
        return View(model);

    }

    public ActionResult Edit(int id)
    {
        var entity = _context.Sliderlar.Select(x => new SliderEditModel
        {
            Id = x.Id,
            Baslik = x.Baslik!,
            Aciklama = x.Aciklama,
            ResimAdi = x.Resim,
            Index = x.Index,
            Aktif = x.Aktif,
        }).FirstOrDefault(x => x.Id == id);

        return View(entity);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(int id, SliderEditModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }
         if (ModelState.IsValid)
         {
            var entity = _context.Sliderlar.FirstOrDefault(x => x.Id == model.Id);

            if (entity != null)
            {            
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
            
                entity.Baslik = model.Baslik;
                entity.Aciklama = model.Aciklama;
                entity.Aktif = model.Aktif;
                entity.Index = model.Index;

                _context.SaveChanges();

                TempData["Message"] = $"{model.Baslik} başarıyla güncellendi";
                return RedirectToAction("Index");
            }
        }
        
        return View(model);
    }

    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var entity = _context.Sliderlar.FirstOrDefault(x => x.Id == id);

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
        var entity = _context.Sliderlar.FirstOrDefault(x => x.Id == id);

        if (entity != null)
        {
            _context.Sliderlar.Remove(entity);
            _context.SaveChanges();

            TempData["Message"] = $"{entity.Baslik} Silme başarılı";
        }

        return RedirectToAction("Index");
    }
}