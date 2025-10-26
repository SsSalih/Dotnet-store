using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;

public static class SeedDatabase
{
    public static async void Initialize(IApplicationBuilder app)
    {
        
        var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        
        if(!roleManager.Roles.Any())
        {
            var admin = new AppRole{ Name = "Admin"};
            await roleManager.CreateAsync(admin);
        }

        if(!userManager.Users.Any())
        {
            var admin = new AppUser
            {
                AdSoyad= "Salih kıılç",
                UserName = "ahmetsuk08",
                Email = "ahmetsuk55@gmail.com",
            };

            await userManager.CreateAsync(admin,"123456");
            await userManager.AddToRoleAsync(admin, "Admin");
            
            var customer = new AppUser
            {
                AdSoyad= "ahmet kıılç",
                UserName = "ahmetsuk53",
                Email = "ahmetsuk55@info.com",
            };

            await userManager.CreateAsync(customer,"123456");
        }
    }
}