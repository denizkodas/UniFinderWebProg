using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using UniFinderWebProg.Utulity; 

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();

// Oturum y�netimini etkinle�tir
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true; // Taray�c� eri�im s�n�rlamas�
    options.Cookie.IsEssential = true; // GDPR uyumlu
});

// CORS yap�land�rmas�
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

// Veritaban� ba�lant�s� yap�land�rmas�
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Maksimum dosya y�kleme boyutunu art�r
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5242880; // 5 MB
});

var app = builder.Build();

// Middleware s�ras�n� yap�land�r
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HSTS'yi etkinle�tir
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll"); 

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


// Varsay�lan rota yap�land�rmas�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Transition}/{id?}");

app.Run();
