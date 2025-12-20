# 🏋️‍♂️ Spor Salonu Yönetim ve Randevu Sistemi

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-336791?style=for-the-badge&logo=postgresql&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**Modern, Kapsamlı ve AI Destekli Fitness Center Yönetim Platformu**

[Canlı Demo](#) • [Dokümantasyon](#kurulum) • [API Docs](#api-dokümantasyonu) • [Özellikler](#özellikler)

</div>

---

## 📋 İçindekiler

- [Proje Hakkında](#proje-hakkında)
- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Ekran Görüntüleri](#ekran-görüntüleri)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [API Dokümantasyonu](#api-dokümantasyonu)
- [Veritabanı Şeması](#veritabanı-şeması)
- [Proje Yapısı](#proje-yapısı)
- [Katkıda Bulunma](#katkıda-bulunma)
- [Lisans](#lisans)
- [İletişim](#iletişim)

---

## 🎯 Proje Hakkında

Bu proje, **SAÜ Web Programlama Dersi** kapsamında geliştirilmiş, spor salonlarının günlük operasyonlarını dijitalleştiren, üye ve antrenör yönetimini kolaylaştıran, yapay zeka destekli bir web uygulamasıdır.

### Problem
Geleneksel spor salonları, üye kayıtları, randevu yönetimi ve antrenör planlamasını manuel olarak yapmakta, bu da zaman kaybı ve hatalara yol açmaktadır.

### Çözüm
Modern web teknolojileri kullanarak:
- ✅ **Otomatik randevu yönetimi** ile çakışmaları önleme
- ✅ **Gerçek zamanlı üye takibi** ve istatistikler
- ✅ **AI destekli fitness tavsiyeleri** ile kişiselleştirilmiş deneyim
- ✅ **RESTful API** ile mobil uygulama entegrasyonu
- ✅ **Rol bazlı yetkilendirme** ile güvenli veri erişimi

---

## ✨ Özellikler

### 👥 Üye Yönetimi
- 📝 Hızlı ve kolay üye kaydı
- 👤 Kişisel profil yönetimi
- 📊 Üyelik geçmişi ve istatistikleri
- 🔐 Güvenli kimlik doğrulama (ASP.NET Identity)

### 💪 Antrenör Yönetimi
- 🎓 Detaylı antrenör profilleri
- 🏆 Sertifika ve uzmanlık alanları yönetimi
- 📅 Müsaitlik takvimi
- 📈 Performans takibi

### 📅 Randevu Sistemi
- ⏰ Online randevu alma
- 🔄 Otomatik çakışma kontrolü
- ✅ Onay/reddetme mekanizması
- 📧 Bildirim sistemi (geliştirilecek)
- 📱 Responsive takvim görünümü

### 🎛️ Admin Paneli
- 📊 Kapsamlı dashboard
- 📈 Gelir ve üye istatistikleri
- 🔧 Sistem ayarları
- 📋 Detaylı raporlama
- 👥 Kullanıcı yönetimi

### 🤖 AI Fitness Asistanı
- 💬 Doğal dil işleme ile sohbet
- 🏋️ Kişiselleştirilmiş egzersiz önerileri
- 🥗 Diyet planı tavsiyeleri
- 📊 Vücut analizi (geliştirilecek)
- 🎯 Hedef belirleme ve takip

### 🔌 REST API
- 📡 RESTful mimari
- 📚 Swagger/OpenAPI dokümantasyonu
- 🔐 Token bazlı kimlik doğrulama
- 🔍 LINQ ile gelişmiş filtreleme
- 📊 JSON formatında veri transferi

---

## 🛠️ Teknolojiler

### Backend
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-11.0-239120?style=flat-square&logo=c-sharp)
![Entity Framework](https://img.shields.io/badge/Entity_Framework_Core-8.0-512BD4?style=flat-square)

- **Framework:** ASP.NET Core MVC 8.0
- **Dil:** C# 11.0
- **ORM:** Entity Framework Core 8.0
- **Authentication:** ASP.NET Core Identity
- **API Documentation:** Swashbuckle (Swagger)

### Database
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-336791?style=flat-square&logo=postgresql&logoColor=white)

- **Veritabanı:** PostgreSQL 14+
- **Migration:** EF Core Migrations
- **Connection Pool:** Npgsql

### Frontend
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=flat-square&logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat-square&logo=css3)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat-square&logo=javascript&logoColor=black)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)

- **UI Framework:** Bootstrap 5.3
- **Template Engine:** Razor Pages
- **JavaScript:** Vanilla JS + jQuery
- **CSS:** Custom minimalist theme
- **Icons:** Bootstrap Icons

### AI Integration
![Claude AI](https://img.shields.io/badge/Claude_AI-Anthropic-000000?style=flat-square)

- **AI Provider:** Anthropic Claude API
- **Model:** Claude Sonnet 4.5
- **Integration:** RESTful API calls

### DevOps & Tools
![Git](https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual_Studio-5C2D91?style=flat-square&logo=visual-studio)

- **Version Control:** Git & GitHub
- **IDE:** Visual Studio 2022
- **Package Manager:** NuGet

---

## 📸 Ekran Görüntüleri

### Ana Sayfa
> Modern ve kullanıcı dostu arayüz

![Ana Sayfa](Screenshots/homepage.png)

### Admin Dashboard
> Kapsamlı yönetim paneli ve istatistikler

![Admin Dashboard](Screenshots/dashboard.png)

### Randevu Sistemi
> Kolay ve hızlı randevu alma

![Randevu](Screenshots/appointments.png)

### AI Fitness Asistanı
> Yapay zeka destekli kişisel antrenör

![AI Assistant](Screenshots/ai-assistant.png)


---

## 🚀 Kurulum

### Ön Gereksinimler

```bash
✅ .NET 8.0 SDK veya üzeri
✅ PostgreSQL 14+ 
✅ Visual Studio 2022 (önerilir) veya VS Code
✅ Git
✅ Anthropic API Key (AI özelliği için - opsiyonel)
```

### Adım 1: Projeyi Klonlayın

```bash
git clone https://github.com/KULLANICI_ADINIZ/spor-salonu-yonetim.git
cd spor-salonu-yonetim
```

### Adım 2: Veritabanı Ayarları

**PostgreSQL'i başlatın** ve yeni bir veritabanı oluşturun:

```sql
CREATE DATABASE SporSalonuDB;
```

**appsettings.json** dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SporSalonuDB;Username=postgres;Password=SIFRENIZ"
  },
  "Anthropic": {
    "ApiKey": "ANTHROPIC_API_KEY" // Opsiyonel
  }
}
```

### Adım 3: Bağımlılıkları Yükleyin

```bash
dotnet restore
```

### Adım 4: Veritabanı Migration'larını Uygulayın

```bash
dotnet ef database update
```

### Adım 5: Projeyi Çalıştırın

```bash
dotnet run
```

veya Visual Studio'da `F5` tuşuna basın.

Tarayıcınızda şu adrese gidin: `https://localhost:5001`

---

## 👤 Kullanım

### Varsayılan Hesaplar

#### Admin Hesabı
```
Email: ogrencinumarasi@sakarya.edu.tr
Şifre: sau
```

#### Test Üye Hesabı
```
Email: uye@test.com
Şifre: Uye123!
```

### İlk Adımlar

1. **Admin olarak giriş yapın** ve sistemi keşfedin
2. **Antrenörler ekleyin** (Admin → Antrenörler → Yeni Ekle)
3. **Hizmetler tanımlayın** (Admin → Hizmetler → Yeni Ekle)
4. **Üye olarak giriş yapın** ve randevu alın
5. **AI Asistanı deneyin** (Üye Paneli → AI Fitness Asistanı)

---

## 📡 API Dokümantasyonu

### Swagger UI

Proje çalışırken şu adresten API dokümantasyonuna erişebilirsiniz:

```
https://localhost:5001/swagger
```

### Örnek API Endpoints

#### Antrenörler

```http
GET /api/antrenorler
GET /api/antrenorler/{id}
POST /api/antrenorler
PUT /api/antrenorler/{id}
DELETE /api/antrenorler/{id}
```

#### Randevular

```http
GET /api/randevular
GET /api/randevular/{id}
GET /api/randevular/uye/{uyeId}
GET /api/randevular/antrenor/{antrenorId}
POST /api/randevular
PUT /api/randevular/{id}
DELETE /api/randevular/{id}
```

#### Üyeler

```http
GET /api/uyeler
GET /api/uyeler/{id}
POST /api/uyeler
PUT /api/uyeler/{id}
DELETE /api/uyeler/{id}
```

### Örnek API Kullanımı

```javascript
// Tüm antrenörleri getir
fetch('https://localhost:5001/api/antrenorler')
  .then(response => response.json())
  .then(data => console.log(data));

// Belirli bir randevuyu getir
fetch('https://localhost:5001/api/randevular/1')
  .then(response => response.json())
  .then(data => console.log(data));
```



## 📁 Proje Yapısı

```
SporSalonuYonetim/
├── 📁 Controllers/              # MVC Controller'lar
│   ├── AccountController.cs     # Kimlik doğrulama
│   ├── AdminController.cs       # Admin işlemleri
│   ├── HomeController.cs        # Ana sayfa
│   ├── UyeController.cs         # Üye işlemleri
│   ├── AntrenorController.cs    # Antrenör CRUD
│   ├── HizmetController.cs      # Hizmet CRUD
│   ├── RandevuController.cs     # Randevu yönetimi
│   └── 📁 Api/                  # REST API Controllers
│       ├── AntrenorlerController.cs
│       ├── RandevularController.cs
│       ├── UyelerController.cs
│       └── AIController.cs      # AI entegrasyonu
│
├── 📁 Models/                   # Veri modelleri
│   ├── ApplicationUser.cs       # Identity kullanıcısı
│   ├── Uye.cs                   # Üye modeli
│   ├── Antrenor.cs              # Antrenör modeli
│   ├── Hizmet.cs                # Hizmet modeli
│   ├── Randevu.cs               # Randevu modeli
│   ├── Sertifika.cs             # Sertifika modeli
│   ├── 📁 Enums/                # Enum'lar
│   │   └── OnayDurumu.cs
│   └── 📁 ViewModels/           # View Model'ler
│       ├── LoginViewModel.cs
│       ├── RegisterViewModel.cs
│       ├── ProfileViewModel.cs
│       └── HomeViewModel.cs
│
├── 📁 Views/                    # Razor View'lar
│   ├── 📁 Home/                 # Ana sayfa view'ları
│   ├── 📁 Account/              # Giriş/Kayıt view'ları
│   ├── 📁 Admin/                # Admin panel view'ları
│   ├── 📁 Uye/                  # Üye panel view'ları
│   ├── 📁 Antrenor/             # Antrenör view'ları
│   ├── 📁 Hizmet/               # Hizmet view'ları
│   ├── 📁 Randevu/              # Randevu view'ları
│   └── 📁 Shared/               # Paylaşılan view'lar
│       ├── _Layout.cshtml       # Ana layout
│       └── _ValidationScriptsPartial.cshtml
│
├── 📁 Data/                     # Veritabanı context
│   └── ApplicationDbContext.cs  # EF Core DbContext
│
├── 📁 Migrations/               # EF Core migrations
│   ├── 20241205_AddIdentity.cs
│   ├── 20241206_CreateMainTables.cs
│   └── ApplicationDbContextModelSnapshot.cs
│
├── 📁 wwwroot/                  # Statik dosyalar
│   ├── 📁 css/                  # CSS dosyaları
│   │   └── site.css             # Ana stil dosyası
│   ├── 📁 js/                   # JavaScript dosyaları
│   │   ├── site.js
│   │   └── ai-chat.js           # AI sohbet arayüzü
│   ├── 📁 lib/                  # Client-side kütüphaneler
│   └── 📁 images/               # Görseller
│
├── 📁 Services/                 # Servis katmanı (opsiyonel)
│   └── AnthropicService.cs      # AI API servisi
│
├── Program.cs                   # Uygulama başlangıcı
├── appsettings.json             # Yapılandırma
├── appsettings.Development.json # Development ayarları
├── .gitignore                   # Git ignore dosyası
└── README.md                    # Bu dosya
```

---

## 🔐 Güvenlik

### Uygulanan Güvenlik Önlemleri

- ✅ **Password Hashing:** ASP.NET Identity ile güvenli şifre saklama
- ✅ **CSRF Protection:** Anti-forgery token'lar
- ✅ **XSS Prevention:** Razor'ın otomatik HTML encoding'i
- ✅ **SQL Injection Prevention:** Entity Framework parameterized queries
- ✅ **Role-based Authorization:** Admin ve Üye rolleri
- ✅ **HTTPS:** SSL/TLS şifreleme
- ✅ **Input Validation:** Client-side ve server-side doğrulama

### Güvenlik Önerileri

```bash
⚠️ appsettings.json'daki hassas bilgileri GitHub'a yüklemeyin
⚠️ Production'da güçlü şifreler kullanın
⚠️ API key'leri environment variables'da saklayın
⚠️ CORS ayarlarını production'a göre yapılandırın
```

---

## 🧪 Test

### Unit Test (Geliştirilecek)

```bash
dotnet test
```

### Manuel Test Senaryoları

1. **Üye Kaydı:** Yeni üye oluşturma
2. **Giriş Yapma:** Kimlik doğrulama testi
3. **Randevu Alma:** Çakışma kontrolü testi
4. **Admin İşlemleri:** CRUD operasyonları
5. **API Endpoints:** Swagger üzerinden test

---

## 📈 Performans

### Optimizasyon Teknikleri

- ✅ **Eager Loading:** Include() ile N+1 query problemi önleme
- ✅ **Async/Await:** Asenkron veritabanı işlemleri
- ✅ **Caching:** (Geliştirilecek) Response caching
- ✅ **Pagination:** (Geliştirilecek) Büyük veri setleri için sayfalama
- ✅ **Indexing:** Veritabanı indeksleme

---

## 🚧 Gelecek Planlar

### Versiyon 2.0 Roadmap

- [ ] 📧 Email bildirim sistemi
- [ ] 📱 Mobil uygulama (React Native)
- [ ] 💳 Online ödeme entegrasyonu
- [ ] 📊 Gelişmiş analitik ve raporlama
- [ ] 🎥 Canlı ders akışı
- [ ] 🏆 Gamification (Başarı rozetleri, liderlik tablosu)
- [ ] 🌐 Çoklu dil desteği (i18n)
- [ ] 📸 AI görsel analiz (vücut ölçümü)
- [ ] 🔔 Push notification
- [ ] 💬 Gerçek zamanlı chat (SignalR)

---

## 🤝 Katkıda Bulunma

Katkılarınızı bekliyoruz! Lütfen katkıda bulunmadan önce aşağıdaki adımları izleyin:

### Adımlar

1. **Projeyi fork edin**
2. **Feature branch oluşturun**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Değişikliklerinizi commit edin**
   ```bash
   git commit -m 'feat: add some amazing feature'
   ```
4. **Branch'inizi push edin**
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Pull Request açın**

### Commit Mesaj Formatı

```
<type>: <subject>

<body>

<footer>
```

**Type'lar:**
- `feat`: Yeni özellik
- `fix`: Bug düzeltme
- `docs`: Dokümantasyon
- `style`: Kod formatı
- `refactor`: Code refactoring
- `test`: Test ekleme
- `chore`: Bakım işleri

---

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakınız.

```
MIT License

Copyright (c) 2024 [Adınız Soyadınız]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

---

## 👨‍💻 Geliştirici

<div align="center">

### **[ahmet can alpay]**

**Sakarya Üniversitesi - Bilgisayar Mühendisliği**

**Öğrenci No:** B251210350 
**Ders:** Web Programlama  
**Dönem:** 2025-2026 Güz  
**Proje Tarihi:** Aralık 2025

</div>

---

## 🙏 Teşekkürler

- **Sakarya Üniversitesi** - Eğitim desteği için
- **Anthropic** - Claude AI API için
- **Stack Overflow Community** - Sorun çözümlerinde yardımları için
- **Microsoft Docs** - Kapsamlı dokümantasyon için

---

---

<div align="center">

### ⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!

**Made with ❤️ by [Ahmet can alpay]**

[🔝 Başa Dön](#-spor-salonu-yönetim-ve-randevu-sistemi)

</div>
