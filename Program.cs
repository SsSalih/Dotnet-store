using Microsoft.EntityFrameworkCore;
using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using dotnet_store.Services;  // DataContext'in bulunduğu namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IEmailService, SmptpEmailService>(); //* ne zaman IEmail services çağrılırsa SmptpEmailService döndürülür
builder.Services.AddTransient<ICartService, CartService>();//* ne zaman ICartService services çağrılırsa CartService döndürülür
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısı ekleniyor
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit =false;

    options.User.RequireUniqueEmail = true;
    //options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyz0123456789 ";

    options.Lockout.MaxFailedAccessAttempts =5; //5 hatalı girişde hesap kitlenir
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2); //  dk kitlenir
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDeied";
    options.ExpireTimeSpan = TimeSpan.FromDays(5); // kullanıcı tarayıcıda tutalan çerez sayesinde 5 gün boyunca otomatik girer
    options.SlidingExpiration = false; // her talep yapıldığında cookie bilgisi yenilerinr

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();  // Statik dosyaların sunulması

app.MapControllerRoute(
    name: "urunler_by_kategori",
    pattern: "urunler/{url?}",
    defaults: new { controller = "Urun", action = "List" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


SeedDatabase.Initialize(app);//* burası seed databaseyi çağırır ve kullanıcı yoksa çağırılan yer kullanıcı ekler
app.Run();
