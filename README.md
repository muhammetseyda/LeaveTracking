# 🗂️ Leave Tracking System

**Leave Tracking**, çalışanların izin taleplerini kolayca oluşturabildiği, yöneticilerin bu talepleri onaylayabildiği ve tüm sürecin takibini sağlayan bir izin yönetim sistemidir. Sistem; kullanıcı rolleri, JWT korumalı API, mail bildirimleri ve modern Clean Architecture yaklaşımıyla geliştirilmiştir. MVC projesi API projesini tüketmektedir.

---

## 🚀 Teknolojiler ve Mimariler

- ✅ .NET Core 9
- ✅ Entity Framework Core
- ✅ SQL Server (SSMS)
- ✅ Clean Architecture
- ✅ Minimal API + MVC (Hybrid yapı)
- ✅ AutoMapper
- ✅ Fluent Validation
- ✅ JWT Authentication
- ✅ Scalar UI
- ✅ SMTP (Gmail) ile Mail Gönderimi

---

## 📌 Proje Mimarisi

Sistem Clean Architecture yaklaşımı ile katmanlara ayrılmıştır:

- `LeaveTracking.MinimalAPI`: Minimal API sunumu
- `LeaveTracking.MVC`: MVC üzerinden kullanıcı arayüzü
- `LeaveTracking.Infrastructure`: Veritabanı işlemleri ve context
- `LeaveTracking.Application`: DTO'lar, Servisler, Validation'lar
- `LeaveTracking.Domain`: Entity ve Enum'lar

---

## 👤 Rol Bazlı Yetkiler

### 👨‍💼 **Employee**
- İzin talebi oluşturma
- Kendi izin listesini görüntüleme
- Bekleyen, onaylı ve reddedilen izinleri ayırt etme
- Kalan izin, toplam hak, kullanılan gün sayısı özet bilgisi
- Dashboard kart görünüm desteği

### 👨‍💼 **Manager**
- Tüm çalışanların izinlerini listeleme
- Onay/Red işlemleri ve red sebebi belirtme
- Takvim görünümünde izinleri kontrol etme
- Kullanıcı listeleme / silme / ekleme
- Kendi adına izin talebi oluşturma ve listeleme
- 5 günden az izni kalan çalışanlara **toplu mail gönderimi**

---

## 🌱 Seed Data

Aşağıdaki kullanıcılar sistemde hazır olarak gelir:

| Rol      | Email                         | Şifre      |
|----------|-------------------------------|------------|
| Manager  | muhammedseyda@hotmail.com     | Aa123456.  |
| Employee | yazilimcisarman@hotmail.com   | Aa123456.  |

> Bu kullanıcılar ile direkt giriş yapabilirsiniz.

---

## ⚙️ Kurulum Adımları

1. **Projeyi klonla:**
   ```bash
   git clone https://github.com/muhammetseyda/LeaveTracking.git
   cd LeaveTracking
   ```

2. **ConnectionString ayarları:**
   - `appsettings.json` ve `Infrastructure` altındaki ilgili dosyada **SQL Server bağlantısı** güncellenmeli.
   - **LeaveTracking.MinimalAPI\appsettings.json**
   - **LeaveTracking.Infrastructure\Persistence\Context\AppDbContextFactory.cs**

3. **SMTP ayarı:**
   - Mail göndermek için EmailServices.cs içinde kendi **Gmail adresinizi ve Uygulama Şifrenizi** girin.
   - **LeaveTracking.Infrastructure\Services\EmailService.cs**

4. **Migration ve Seed işlemleri:**
   - Migration ve seed işlemleri otomatik olarak yapılır:

5. **Projeyi çalıştır:**
   - API:
     ```bash
     cd LeaveTracking.MinimalAPI
     dotnet run
     ```
   - MVC:
     ```bash
     cd LeaveTracking.MVC
     dotnet run
     ```

---

## 🔐 Scalar UI Desteği

API endpoint'leri `Scalar` UI üzerinden test edilebilir:  
📍 `https://localhost:{port}/scalar/v1`

---

## ✉️ E-Posta Bildirimleri

- İzin oluşturulduğunda (Employee, Manager)
- Onaylandığında / Reddedildiğinde (Employee)
- 5 günden az izni kalan çalışanlara uyarı (Employee, Manager)
gibi durumlarda sistem otomatik olarak e-posta gönderir.

---

## 📦 Kullanılan Başlıca NuGet Paketleri

- AutoMapper
- FluentValidation
- Microsoft.EntityFrameworkCore
- Microsoft.AspNetCore.Authentication.JwtBearer

---

## 💡 Ekstra Bilgiler

- Scalar UI yalnızca Minimal API'de açıktır
- UI, MVC üzerinden sağlanır
- Proje tamamen katmanlı mimaride geliştirilmiştir
- Kodlar ve katmanlar SOLID prensiplerine göre ayrılmıştır

---

## ✍️ Geliştirici

**Muhammet Seyda Armağan**  
📧 [muhammedseyda@hotmail.com](mailto:muhammedseyda@hotmail.com)

---

## 🏗️ Dosya Yapısı

### 📦 LeaveTracking.Application

```
LeaveTracking.Application/
├── DTOs/
│   ├── EmailDTOs/
│   │   ├── EmailLeaveRequestDto.cs
│   │   └── EmailUserDto.cs
│   ├── LeaveRequestDTOs/
│   │   ├── CreateLeaveRequestDto.cs
│   │   ├── LeaveRequestDto.cs
│   │   ├── RejectLeaveRequestDto.cs
│   │   ├── ResultLeaveRequestDto.cs
│   │   └── UpdateLeaveRequestDto.cs
│   ├── UserDTOs/
│   │   ├── LoginDto.cs
│   │   ├── RegisterDto.cs
│   │   ├── ResultUserDto.cs
│   │   ├── UserDto.cs
│   │   └── UserLeaveSummaryDto.cs
│   └── WebDTOs/
│       ├── EmployeeDash.cs
│       └── ManagerDash.cs
├── Helpers/
│   ├── LeaveCalculator.cs
│   └── LeaveTypeHelpers.cs
├── Interfaces/
│   ├── Repositories/
│   │   ├── ILeaveRequestRepository.cs
│   │   └── IUserRepository.cs
│   └── Services/
│       ├── IAuthService.cs
│       ├── ICurrentUserService.cs
│       ├── IEmailService.cs
│       ├── IEmailTemplateService.cs
│       ├── ILeaveRequestService.cs
│       └── IUserServices.cs
├── Mapper/
│   └── MappingProfile.cs
├── Services/
│   ├── AuthService.cs
│   ├── LeaveRequestService.cs
│   └── UserServices.cs
├── Shared/
│   └── ResponseResult.cs
└── Validators/
    ├── LeaveRequestValidator.cs
    └── UserRegisterValidator.cs
```

### 🏢 LeaveTracking.Domain

```
LeaveTracking.Domain/
├── Common/
│   └── BaseEntity.cs
├── Entities/
│   ├── LeaveRequest.cs
│   └── User.cs
└── Enums/
    ├── LeaveStatus.cs
    ├── LeaveType.cs
    └── UserRole.cs
```

### 🔧 LeaveTracking.Infrastructure

```
LeaveTracking.Infrastructure/
├── Externals/
│   └── EmailTemplates/
│       ├── ApprovedLeaveRequestTemplate.html
│       ├── CreateLeaveRequestTemplate.html
│       ├── NewNotificationLeaveRequestTemplate.html
│       ├── RejectedLeaveRequestTemplate.html
│       └── UserLeaveQuotaTemplate.html
├── Migrations/
│   ├── 20250807183828_mig1.cs
│   ├── 20250807183828_mig1.Designer.cs
│   └── AppDbContextModelSnapshot.cs
├── Persistence/
│   ├── Configurations/
│   │   ├── LeaveRequestConfiguration.cs
│   │   └── UserConfiguration.cs
│   └── Context/
│       ├── AppDbContext.cs
│       └── AppDbContextFactory.cs
├── Repositories/
│   ├── LeaveRequestRepository.cs
│   └── UserRepository.cs
├── Seed/
│   └── SeedData.cs
└── Services/
    ├── CurrentUserService.cs
    ├── EmailService.cs
    └── EmailTemplateService.cs
```

### 🚀 LeaveTracking.MinimalAPI

```
LeaveTracking.MinimalAPI/
├── Properties/
│   └── launchSettings.json
├── AuthModule.cs
├── LeaveTracking.MinimalAPI.csproj
├── ManagerModule.cs
├── Program.cs
├── UserModule.cs
├── appsettings.Development.json
└── appsettings.json
```

### 🌐 LeaveTracking.Web

```
LeaveTracking.Web/
├── Controllers/
│   ├── AccountController.cs
│   ├── BaseController.cs
│   ├── EmployeeController.cs
│   ├── HomeController.cs
│   └── ManagerController.cs
├── Models/
│   └── ErrorViewModel.cs
├── Properties/
│   └── launchSettings.json
├── Views/
│   ├── Account/
│   │   └── Login.cshtml
│   ├── Employee/
│   │   ├── CreateLeave.cshtml
│   │   ├── Index.cshtml
│   │   └── LeaveRequest.cshtml
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Manager/
│   │   ├── CalendarLeaveRequest.cshtml
│   │   ├── Index.cshtml
│   │   ├── LeaveRequest.cshtml
│   │   └── Register.cshtml
│   ├── Shared/
│   │   ├── Error.cshtml
│   │   ├── _Layout.cshtml
│   │   ├── _Layout.cshtml.css
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── favicon.ico
├── Program.cs
├── appsettings.Development.json
└── appsettings.json
```
