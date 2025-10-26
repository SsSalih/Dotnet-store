
using System.Linq;
using System.Threading.Tasks;
using dotnet_store.Models;
using dotnet_store.Services;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ICartService _cartService;
    private readonly DataContext _context;

    private readonly IConfiguration _configuration;

    public OrderController(ICartService cartService, DataContext context, IConfiguration configuration)
    {
        _cartService = cartService;
        _context = context;
        _configuration = configuration;
    }

    [Authorize(Roles = "Admin")]
    public ActionResult Index()
    {
        return View(_context.Orders.ToList());

    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int? id) // Sipariş ID'si parametre olarak alınır
    {
        if (id == null)
        {
            return NotFound(); // ID yoksa bulunamadı döndür
        }

        // Veritabanından siparişi çekerken OrderItem'ları ve her OrderItem'ın Urun'unu yükle
        var order = await _context.Orders // _context, sizin DbContext örneğiniz olmalı
                            .Include(o => o.OrderItem)    // Siparişin OrderItem listesini yükle
                                .ThenInclude(oi => oi.Urun) // Her bir OrderItem için ilişkili Urun'u yükle
                            .FirstOrDefaultAsync(m => m.OrderId == id); // ID'ye göre siparişi bul

        if (order == null)
        {
            return NotFound(); // Sipariş bulunamadıysa bulunamadı döndür
        }

        return View(order); // Bulunan siparişi View'a model olarak gönder
    }
    public async Task<ActionResult> Checkout()
    {
        ViewBag.Cart = await _cartService.GetCart(User.Identity?.Name!);
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Checkout(OrderCrateModel model)
    {
        var userName = User.Identity?.Name!;
        var cart = await _cartService.GetCart(userName);

        if (cart.CartItems.Count == 0)
        {
            ModelState.AddModelError("", "Sepetinizde herhangi bir ürün yok");
        }

        if (ModelState.IsValid)
        {
            var order = new Order
            {
                AdSoyad = model.AdSoyad,
                Sehir = model.Sehir,
                Telefon = model.Telefon,
                PostaKodu = model.PostaKodu,
                AdresSatiri = model.AdresSatiri,
                SiparişNotu = model.SiparişNotu!,
                SiparişTarihi = DateTime.Now,
                ToplamFiyat = cart.Toplam(),
                Username = userName,
                OrderItem = cart.CartItems.Select(x => new Models.OrderItem
                {
                    UrunId = x.UrunId,
                    Fiyat = x.Urun.Fiyat,
                    Miktar = x.Miktar
                }).ToList(),
            };

            var paymnet = await ProcessPayment(model, cart);

            if (paymnet.Status == "success")
            {

                _context.Orders.Add(order);
                _context.Carts.Remove(cart);

                await _context.SaveChangesAsync();

                return RedirectToAction("Completed", new { orderId = order.OrderId });
            }

            else
            {
                ModelState.AddModelError("",paymnet.ErrorMessage);
            }
        }

        ViewBag.Cart = cart;
        return View(model);
    }

    public ActionResult Completed(string orderId)
    {
        return View("Completed", orderId);
    }

    public async Task<ActionResult> OrderIndex()
    {
        var username = User.Identity?.Name;
        var orders = await _context.Orders
                          .Include(o => o.OrderItem)
                          .ThenInclude(x => x.Urun)
                          .Where(i => i.Username == username)
                          .ToListAsync();
        return View(orders);
    }
    public ActionResult OrderDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = _context.Orders
                           .Include(o => o.OrderItem)
                           .ThenInclude(x => x.Urun)
                           .FirstOrDefault(i => i.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);


    }

    private async Task<Payment> ProcessPayment(OrderCrateModel model, Cart cart)
    {
        Options options = new Options();
        options.ApiKey = _configuration["PaymentAPI:ApıKey"];
        options.SecretKey = _configuration["PaymentAPI:SecretKey"];
        options.BaseUrl = "https://sandbox-api.iyzipay.com";

        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = Guid.NewGuid().ToString(); //! guid ile benzersiz kimlik atıyoruz
        request.Price = cart.AraToplam().ToString(); //! burada eğer toplamı verirsek sepetteki toplamla uyuşmaz son hesaplananla ve hata verir
        request.PaidPrice = cart.AraToplam().ToString();
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.BasketId = "B67832";
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = model.CartName;
        paymentCard.CardNumber = model.CartNumber;
        paymentCard.ExpireMonth = model.CartExpirationMonth;
        paymentCard.ExpireYear = model.CartExpirationYear;
        paymentCard.Cvc = model.CartCVV;
        paymentCard.RegisterCard = 0;
        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = "BY789";
        buyer.Name = model.AdSoyad;
        buyer.Surname = "Doe";
        buyer.GsmNumber = model.Telefon;
        buyer.Email = "email@email.com";
        buyer.IdentityNumber = "74300864791";
        buyer.LastLoginDate = "2015-10-05 12:43:35";
        buyer.RegistrationDate = "2013-04-21 15:12:09";
        buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        buyer.Ip = "85.34.78.112";
        buyer.City = model.Sehir;
        buyer.Country = "Turkey";
        buyer.ZipCode = model.PostaKodu;
        request.Buyer = buyer;

        Address address = new Address();
        address.ContactName = model.AdSoyad;
        address.City = model.Sehir;
        address.Country = "Turkey";
        address.Description = model.AdresSatiri;
        address.ZipCode = model.PostaKodu;
        request.ShippingAddress = address;
        request.BillingAddress = address;

        List<BasketItem> basketItems = new List<BasketItem>();

        foreach (var item in cart.CartItems)  //* foreachi biz oluşturduk sepetin içindeki her ürünü eklemek için
        {
            BasketItem basketItem = new BasketItem();
            basketItem.Id = item.CartItemId.ToString();
            basketItem.Name = item.Urun.UrunAdi;
            basketItem.Category1 = "telefon";
            basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            basketItem.Price = item.Urun.Fiyat.ToString();

            basketItems.Add(basketItem);

        }


        request.BasketItems = basketItems;
        return await Payment.Create(request, options);
    }
}