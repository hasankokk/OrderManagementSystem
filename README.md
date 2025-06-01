# 🛒 Order Management System (.NET 9 - C#)

Terminal tabanlı, sipariş takibi, stok yönetimi ve kullanıcı rollerine göre işlemlerini yöneten .NET 9 destekli sipariş yönetim sistemidir.

---

## 🚀 Proje Amacı

Kullanıcıların sipariş oluşturabilmesini, mutfak personelinin sipariş hazırlığını yönetmesini ve yöneticilerin stok/satış raporlarını kontrol etmesini sağlar. Tüm işlemler roller arası erişim kontrolleriyle ayrılmıştır.

---

## 📦 Özellikler

### 👥 Kullanıcı Girişi ve Rol Yapısı

* Roller: `Admin`, `Mutfak`, `Kullanıcı`
* Giriş bilgisi: E-posta ve şifre (Hashlenmiş olarak saklanır)
* Yeni kayıt akışı & doğrulama

### 🧑‍🍳 Kullanıcı Paneli (Kullanıcı Rolü)

* Sipariş oluşturma (sepete ürün ekleme / çıkarma / güncelleme)
* Sipariş geçmişi görüntüleme (Aktif / Teslim edilmiş siparişler)
* Kullanıcı bilgileri görüntüleme / şifre değiştirme

### 🍲 Mutfak Paneli (Mutfak Rolü)

* Hazırlanacak siparişleri listeleme
* Sipariş durumu güncelleme (Alındı → Hazırlanıyor → Hazır → Teslim Edildi)
* Teslim edilmiş sipariş geçmişi

### 🛠️ Admin Paneli (Admin Rolü)

* Rol bazlı kullanıcı oluşturma
* Kullanıcı listeleme (rol bazlı)
* Ürün yönetimi: Ekle / Güncelle / Sil / Listele
* Stok raporu görüntüleme
* Satış raporları: Günlük özet, en çok satılan ürünler, kullanıcı bazlı harcamalar
* CSV olarak dışa aktarma (ExportCsvFiles klasörüne kayıt edilir)

---

## 🧰 Kullanılan Teknolojiler

| Katman/Modül | Teknoloji                |
| ------------ | ------------------------ |
| Backend      | C# (.NET 9)              |
| ORM          | Entity Framework Core    |
| CLI UI       | Spectre.Console          |
| DB           | SQLite, SQLServer        |
| Auth         | HashPassword + Role Enum |

---

## 📦 NuGet Paketleri ve Kullanım Amaçları

Aşağıdaki komutlar ile projeye gerekli NuGet bağımlılıklarını yükleyebilirsiniz:

```bash
dotnet add package Microsoft.EntityFrameworkCore
```

* **Amaç**: DbContext, LINQ, `Include`, `SaveChanges` gibi ORM işlemleri için.

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

* **Amaç**: Projeyi SQLite veritabanı ile kullanabilmek için gerekli sağlayıcı.

```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

* **Amaç**: `dotnet ef migrations` gibi CLI destekli migration işlemleri için.

```bash
dotnet add package Spectre.Console
```

* **Amaç**: Konsol üzerinde renkli yazı, tablo, etkileşimli seçimler gibi zengin içerikler üretmek için.

---

## 🗄️ Veritabanı Kurulumu & Migrations

Proje EF Core ile çalıştığı için ilk başlatma öncesinde aşağıdaki adımları uygulayın:
Veritabanı parametrelerini geçerli bir şekilde kendiniz doldurmalısınız.

1. Proje dizinine terminalden gidin:

```bash
cd OrderManagementSystem
```

2. İlk migration dosyasını oluşturun:

```bash
dotnet ef migrations add InitialCreate
```

3. Veritabanını oluşturun:

```bash
dotnet ef database update
```

> Not: EF Core CLI yüklü değilse, yüklemek için:

```bash
dotnet tool install --global dotnet-ef
```

---

## 📁 Proje Yapısı

* `Models/`: User, Product, Order, OrderDetail, ShoppingCart, Enum(Roles, Status)
* `Helpers/`: UserHelper, KitchenHelper, AdminHelper, ProductHelper, OrderHelper, ConsoleMenu, ColoredHelper
* `Repositories/`: OrderRepository, UserRepository, ProductRepository
* `Data/`: AppDbContext
* `Routing/`: MenuRouter → Rollere göre yönlendirme
* `AppController.cs`: Giriş/Kayıt akışı ve uygulama başlangıç kontrolü
* `Program.cs`: Entry point

---

## 🧪 Örnek Spectre.Console Kullanımı

```csharp
var table = new Table().RoundedBorder();
table.AddColumn("Ürün");
table.AddColumn("Fiyat");
table.AddRow("Hamburger", "75.00₺");
table.AddRow("Kola", "15.00₺");
AnsiConsole.Write(table);
```

Projedeki canlı kullanım örneği (Product Listesi):

```csharp
var table = new Table().RoundedBorder();
table.AddColumn("[blue]Ürün Adı[/]");
table.AddColumn("[green]Fiyat[/]");
table.AddColumn("[yellow]Stok[/]");

foreach (var product in products)
{
    table.AddRow(
        $"[aqua]{product.ProductName}[/]",
        $"[lime]{product.Price:0.00}[/]",
        $"[white]{product.Stock}[/]"
    );
}
AnsiConsole.Write(table);
```

---

## 🔧 Kurulum ve Çalıştırma

```bash
git clone https://github.com/kullaniciadi/OrderManagementSystem.git
cd OrderManagementSystem
dotnet restore
dotnet run
```

İlk çalıştırmada `admin@admin.com / admin` girişi ile admin olarak sistemi test edebilirsiniz.

---


## 🔗 Bağlantılar

* LinkedIn: [linkedin.com/in/hasankokk](https://linkedin.com/in/hasankokk)

---

## 📁 Export Klasörü

Tüm CSV raporlar, çalıştırma dizininde `ExportCsvFiles` klasörüne kaydedilir:

* `today_summary.csv`
* `top_selling_products.csv`
* `user_sales_report.csv`

Her dosya, ilgili raporlama ekranından tetiklenir ve sistem otomatik oluşturur.
