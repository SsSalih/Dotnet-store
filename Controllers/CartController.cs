using System.Threading.Tasks;
using dotnet_store.Models;
using dotnet_store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;


public class CartController : Controller
{
    private readonly DataContext _context;

    private readonly ICartService _cartService;//* buradak, kart servisinin çalışması için program.cs e builder eklenmeli kodun yaşam döngüsü için

    public CartController(DataContext context,ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }


    public async Task<ActionResult> Index()
    {
        var customerId = _cartService.GetCustomerId();
        var cart = await _cartService.GetCart(customerId);
        return View(cart);
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart(int urunId, int miktar = 1)
    {
        await _cartService.AddToCart(urunId, miktar);

        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    public async Task<ActionResult> RemoveItem(int urunId, int miktar)
    {
        await _cartService.RemoveItem(urunId, miktar);

        return RedirectToAction("Index", "Cart");
    }


}
