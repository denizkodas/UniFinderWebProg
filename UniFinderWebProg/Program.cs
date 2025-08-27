using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using UniFinderWebProg.Utulity; 

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();

// Oturum yönetimini etkinleþtir
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Tarayýcý eriþim sýnýrlamasý
    options.Cookie.IsEssential = true; // GDPR uyumlu
});

// CORS yapýlandýrmasý
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Mail servisini DI konteynerine ekle
builder.Services.AddSingleton<MailService>();

// Veritabaný baðlantýsý yapýlandýrmasý
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Maksimum dosya yükleme boyutunu artýr
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5242880; // 5 MB
});

var app = builder.Build();

// Middleware sýrasýný yapýlandýr
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HSTS'yi etkinleþtir
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll"); 

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


// Varsayýlan rota yapýlandýrmasý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Transition}/{id?}");

app.Run();
