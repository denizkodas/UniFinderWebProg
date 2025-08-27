# UniFinder

UniFinder, üniversite öğrencilerinin birbirleriyle kolayca iletişim kurmasını ve yeni arkadaşlıklar edinmesini sağlayan modern bir web uygulamasıdır. ASP.NET Core Razor Pages (.NET 8) teknolojisiyle geliştirilmiştir.

## Özellikler

- **Kullanıcı Kaydı ve Girişi:** Güvenli kayıt ve giriş sistemi.
- **Profil Yönetimi:** Kullanıcılar profil bilgilerini (üniversite, konum, ilgi alanları, fotoğraf vb.) güncelleyebilir.
- **Eşleşme Sistemi:** Ortak ilgi alanlarına veya üniversiteye göre diğer öğrencilerle eşleşme.
- **Mesajlaşma:** Eşleşen kullanıcılar arasında gerçek zamanlı sohbet.
- **E-posta Bildirimleri:** SMTP (Gmail) ile e-posta bildirim desteği.
- **Duyarlı Tasarım:** Mobil ve masaüstü cihazlarda modern ve kullanıcı dostu arayüz.

## Kullanılan Teknolojiler

- ASP.NET Core Razor Pages (.NET 8)
- Entity Framework Core (SQL Server)
- SMTP (Gmail) ile e-posta gönderimi
- HTML, CSS, JavaScript

## Kurulum

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/tr-tr/sql-server/sql-server-downloads)
- Visual Studio 2022 veya üzeri

2. **Veritabanı bağlantısını ayarlayın:**
- `appsettings.json` dosyasındaki `DefaultConnection` kısmını kendi SQL Server bilgilerinizle güncelleyin.
- Örnek:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=SUNUCU_ADI;Database=UniFinder;Trusted_Connection=True;TrustServerCertificate=True"
  }
  ```

3. **Veritabanı migrasyonlarını uygulayın:**

4. **E-posta ayarlarını yapılandırın:**
- `appsettings.json` dosyasındaki `MailSettings` bölümünü kendi Gmail SMTP bilgilerinizle doldurun.
- Örnek:
  ```json
  "MailSettings": {
    "Mail": "mailadresiniz@gmail.com",
    "DisplayName": "UniFinder",
    "Password": "uygulama-şifresi",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
  ```

5. **Projeyi başlatın:**

6. **Uygulamayı açın:**
- Tarayıcınızda `https://localhost:5001` adresine gidin.

## Kullanım

- **Kayıt Ol:** Yeni bir hesap oluşturun.
- **Profilinizi Düzenleyin:** Üniversite, konum, ilgi alanları ve profil fotoğrafınızı ekleyin.
- **Eşleşmeleri Görüntüleyin:** Diğer öğrencilerle eşleşin ve sohbet başlatın.
- **Mesajlaşın:** Eşleştiğiniz kişilerle mesajlaşın.

## Proje Yapısı

- `UniFinderWebProg/` - Ana Razor Pages projesi
- `Controllers/` - İş mantığı ve istek yönetimi
- `Views/` - Razor Pages ve görünümler
- `wwwroot/` - Statik dosyalar (CSS, JS, görseller)
- `appsettings.json` - Konfigürasyon dosyası

## Güvenlik

- Şifreler ve hassas bilgiler kaynak kodda tutulmamalıdır.
- Gerçek ortamda [User Secrets](https://learn.microsoft.com/tr-tr/aspnet/core/security/app-secrets) veya ortam değişkenleri kullanın.

## Katkı

Katkıda bulunmak isterseniz, lütfen issue açın veya pull request gönderin.

## Lisans

Bu proje MIT lisansı ile lisanslanmıştır.

---

**Not:** Bu proje eğitim amaçlıdır. Gerçek ortamda kullanmadan önce veri gizliliği ve güvenlik gereksinimlerinizi gözden geçirin.
### Adımlar

1. **Projeyi klonlayın:**
