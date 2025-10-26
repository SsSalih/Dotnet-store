using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

 [Authorize(Roles ="Admin")]
public class UserController : Controller
{
    private  UserManager<AppUser>  _userManager;

    private  RoleManager<AppRole> _roleManager;

    public UserController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ActionResult> Index(string role)
    {
        ViewBag.Roller = new SelectList(_roleManager.Roles, "Name", "Name",role);

        if(!string.IsNullOrEmpty(role))
        {
            return View(await _userManager.GetUsersInRoleAsync(role));
        }
        return View(_userManager.Users.ToList());
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(UserCreateModel model)
    {
        if(ModelState.IsValid)
        {
            var result = await _userManager.CreateAsync(new AppUser
            {
                AdSoyad = model.AdSoyad,
                Email = model.Email,
                UserName = model.Email,
            });

            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var user in result.Errors)
            {
                ModelState.AddModelError("" , user.Description);
            }
        }
        return View(model);
    }

    public async Task<ActionResult> Edit(string id)
    {
        var entity = await _userManager.FindByIdAsync(id);

        if(entity != null)
        {

            ViewBag.Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            return View(new UserEditModel
            {
                AdSoyad = entity.AdSoyad,
                Email = entity.Email!,
                SelectedRoles = await _userManager.GetRolesAsync(entity)
                
            });
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    
    public async Task<ActionResult> Edit(UserEditModel model,string id)
    {
        if(ModelState.IsValid)
        {
        var entity = await _userManager.FindByIdAsync(id);

        if(entity != null)
        {
            entity.Email = model.Email;
            entity.AdSoyad = model.AdSoyad;

            var result = await _userManager.UpdateAsync(entity);

            if(result.Succeeded && !string.IsNullOrEmpty(model.Password))
            {
                await _userManager.RemovePasswordAsync(entity);
                await _userManager.AddPasswordAsync(entity, model.Password);
            }

            if(result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(entity);
                await _userManager.RemoveFromRolesAsync(entity ,roles);
                if(model.SelectedRoles != null)
                {
                    await _userManager.AddToRolesAsync(entity, model.SelectedRoles);
                }
                return RedirectToAction("Index");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError ("" , error.Description);
            }
        }             
        }
        return View();
    }

    public async Task<ActionResult> Delete(string? id)
    {
        if(id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = await _userManager.FindByIdAsync(id);

        if(entity != null)
        {
            
            return View(entity);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<ActionResult> Deleteconfirm(string? id)
    {
        if(id == null)
        {
            return RedirectToAction("Index");
        }

        var entity = await _userManager.FindByIdAsync(id);

        if(entity != null)
        {
            await _userManager.DeleteAsync(entity);
            TempData["Message"] = $"{entity.UserName} kaydı silindi";
        }
        return RedirectToAction("Index");
    }
}
    
