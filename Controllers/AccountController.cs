using System.Security.Claims;
using dotnet_store.Models;
using dotnet_store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager;

    private IEmailService _emailService;

    private readonly DataContext _context;

    private readonly ICartService _cartService;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, DataContext context, ICartService cartService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _context = context;
        _cartService = cartService;
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email, AdSoyad = model.AdSoyad };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public ActionResult Login()
    {
        return View();
    }
    [HttpPost]

    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.BeniHatırla, true);//3. kısım coockie kullanılsın mı soruyor biz bunu chechbozdan alıyoruz 4. program csde yazılan kuralları aktif eder

                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    await _cartService.TransferCartToUser(user.UserName!);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);//bu kod sayesinde iznsiz kodlardan önce logine atıyor login doğruysa girmek istediğin yere bu sayede geri atıyor
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                else if (result.IsLockedOut)
                {
                    var lockedTime = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockedTime.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"hesabınız kitlendi lütfen {timeLeft.Minutes + 1} dakika sonra tekrar deneyiniz"); // +1 2.59 2ye yuvarlanıyor +1le onu enhelliyoruz
                }
                else
                {
                    ModelState.AddModelError("", "hatalı parola");
                }
            }

            else
            {
                ModelState.AddModelError("", "hatalı email");
            }
        }
        return View(model);
    }


    [Authorize]
    public async Task<ActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");

    }

    [Authorize]
    public ActionResult Settings()


    {
        return View();
    }

    public ActionResult AccessDeied()
    {
        return View();
    }

    [Authorize]
    public async Task<ActionResult> UserEdit()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //bu claim kullanıcın cokide tutulan idsini bulur farklı claimslerle farklı bilgiler cockiden  çekilebilir @*
        // @using System.Security.Claims
        //  <ul>
        // @foreach (var claim in User.Claims)
        // {
        //     <li><strong>@claim.Type</strong>: @claim.Value</li>
        // }
        // </ul>  bu kod tüm claimleri gösrteirir*@

        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }


        return View(new AccountEditUserModel
        {
            AdSoyad = user.AdSoyad,
            Email = user.Email!
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> UserEdit(AccountEditUserModel model)
    {
        if (ModelState.IsValid)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                user.Email = model.Email;
                user.AdSoyad = model.AdSoyad;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Bilgileriniz Güncellendi";
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }


            return View(model);
        }
        return View(model);
    }

    [Authorize]
    public ActionResult ChangePassword()
    {
        return View();
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> ChangePassword(AccountChangePasword model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.Password, model.ConfirmPassword);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Parolanız Güncellendi";
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
        }
        return View(model);
    }

    public ActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["Message"] = "e posta adresi boş bırakılamz";
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            TempData["Message"] = "e posta adresi kayıtlı değil";
            return View();
        }

        //builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders(); maile kod gitmesi için sondaki kod bu şekilde eklenmeli

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);


        var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token });

        var link = $@"
<!DOCTYPE html>
<html>
  <head>
    <meta charset='UTF-8' />
    <title>Şifre Sıfırlama</title>
  </head>
  <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
    <table width='100%' bgcolor='#f4f4f4' cellpadding='0' cellspacing='0' border='0'>
      <tr>
        <td>
          <table align='center' width='600' cellpadding='20' cellspacing='0' border='0' bgcolor='#ffffff' style='margin-top: 30px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
            <tr>
              <td align='center' style='border-bottom: 1px solid #eeeeee;'>
                <h2 style='color: #333333;'>Şifre Sıfırlama Talebi</h2>
              </td>
            </tr>
            <tr>
              <td>
                <p style='color: #555555; font-size: 16px;'>
                  Merhaba, <br><br>
                  Şifrenizi sıfırlamak için aşağıdaki butona tıklayın. Eğer bu isteği siz yapmadıysanız, bu e-postayı yok sayabilirsiniz.
                </p>
                <div style='text-align: center; margin: 30px 0;'>
                  <a href='http://localhost:5162{url}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                    Şifremi Sıfırla
                  </a>
                </div>
                <p style='color: #888888; font-size: 14px;'>
                  Bağlantı yalnızca 24 saat geçerlidir.
                </p>
              </td>
            </tr>
            <tr>
              <td align='center' style='border-top: 1px solid #eeeeee; font-size: 12px; color: #aaaaaa;'>
                &copy; 2026 kılıç yapı. Tüm hakları saklıdır.
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";


        // var link= $"<a href='http://localhost:5162{url}'>Şifre Sıfırlama</a>";

        await _emailService.SendEmailAsync(user.Email!, "parola sıfırlama", link);

        TempData["Message"] = "e posta adresine sıfırlama kodu gönderildi";


        return RedirectToAction("Login");
    }

    public async Task<ActionResult> ResetPassword(string userId, string token)
    {
        if (userId == null || token == null)
        {
            TempData["Message"] = "userId veya token bulunamadı";
            return RedirectToAction("Login");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            TempData["Message"] = "kullanıcı adı bulunamadı ";
            return RedirectToAction("Login");
        }

        var model = new AccountResetPaswordModel()
        {
            Token = token,
            Email = user.Email!
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ResetPassword(AccountResetPaswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Şifreniz Güncellendi";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
}
