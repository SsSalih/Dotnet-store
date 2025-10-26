using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class AdminController : Controller
{
    [Authorize(Roles ="Admin")]//yetkisiz girişleri önlüyor
    public ActionResult Index()
    {
        return View();
    }

}
