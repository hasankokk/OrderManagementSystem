using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Utils;
using Spectre.Console;

namespace OrderManagementSystem.Helpers;

public class AdminHelper
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly OrderRepository _orderRepository;
    private readonly ProductHelper _productHelper;
    private readonly Auth _auth;

    public AdminHelper(AppDbContext context)
    {
        _context = context;
        _userRepository = new UserRepository(_context);
        _productHelper = new ProductHelper(_context);
        _orderRepository = new OrderRepository(_context);
        _auth = new Auth(_userRepository);
    }

    public void RegisterNewUser()
    {
        var email = Helper.Ask("Yeni kullanıcının e-posta adresi", true);
        var name = Helper.Ask("İsim", true);
        var surname = Helper.Ask("Soyisim", true);
        var password = Helper.AskPassword("Şifre");

        var roleOptions = Enum.GetNames(typeof(Role));
        var selectedIndex = Helper.AskOption(roleOptions, "Kullanıcının rolü:");
        var selectedRole = (Role)(selectedIndex - 1);

        var status = _auth.Register(email!, password, out var errorMessage);
        if (status == RegisterStatus.Success)
        {
            _auth.RegistrationCompletion(email, password, name!, surname!, selectedRole);
        }
        else
        {
            ColoredHelper.Error(errorMessage);
        }
    }
    public void ListUser()
    {
        var users = _userRepository.GetUsers();

        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            var usersInRole = users.Where(u => u.Role == role).ToList();

            if (usersInRole.Any())
            {         
                // Rol başlığını renkli ve kalın olarak yaz
                AnsiConsole.MarkupLine($"[bold cyan]\n--- {role} ---[/]");

                var table = new Table();
                // Tablonun sınırlarını yuvarlak çiz
                table.Border = TableBorder.Rounded;
                // Tablonun başlık sütunlarını ekle (renkli)
                table.AddColumn("[yellow]İsim[/]");
                table.AddColumn("[yellow]Soyisim[/]");
                table.AddColumn("[yellow]E-Posta Adresi[/]");

                // Her kullanıcıyı tabloya bir satır olarak ekle
                foreach (var user in usersInRole)
                {
                    table.AddRow(
                        $"[aqua]{user.Name}[/]",
                        $"[aqua]{user.Surname}[/]",
                        $"[green]{user.Email}[/]"
                    );
                }

                AnsiConsole.Write(table);
            }
        }
    }
    public void Show()
    {
        new ConsoleMenu("Admin Paneli")
            .AddOption("Rol ile Kullanıcı Oluştur", RegisterNewUser)
            .AddOption("Kullanıcıları Listele", () => ListUser())
            .AddOption("Ürün Menüsü", () => OrderMenu())
            .AddOption("Stok Raporları", () => _productHelper.StockReport())
            .AddOption("Satış Raporları", () => ShowSalesReport())
            .Show(isRoot: true);
    }

    public void OrderMenu()
    {
        new ConsoleMenu("Ürün Menüsü")
            .AddOption("Ürün Ekle", () => _productHelper.AddProduct())
            .AddOption("Ürünleri Listele", () => _productHelper.ListProducts())
            .AddOption("Stok Güncelle", () => _productHelper.UpdateStock())
            .AddOption("Ürün Sil", () => _productHelper.DeleteProduct())
            .Show();
    }

    public void ShowSalesReport()
    {
        var orders = _orderRepository.GetOrdersWithProductDetails();
        if (!orders.Any())
        {
            ColoredHelper.Error("Hiç sipariş bulunamadı!");
            return;
        }
        var today = DateTime.Today;
        var todayOrders = orders.Where(o => o.Created.Date == today).ToList();
        var inputReport = Helper.AskOption(new[]
        {
            "Günlük Sipariş Özeti",
            "En Çok Satılan Ürünler",
            "Kullanıcı Bazlı Harcama Raporu",
            "Tüm Raporları CSV dosyasına aktar."
        }, "Görmek istediğiniz rapor'u seçiniz", "Geri Dön");
        if (inputReport == 0)
            return;
        if (inputReport == 1)
            ShowTodayReports(todayOrders);
        if (inputReport == 2)
            ShowHighSellingProducts(orders);
        if (inputReport == 3)
            ShowUserStatistics(orders);
        if (inputReport == 4)
        {
            ShowTodayReports(todayOrders, isExportOnly: true);
            ShowHighSellingProducts(orders, isExportOnly: true);
            ShowUserStatistics(orders, isExportOnly: true);
        }
    }

    // SelectMany: Tüm siparişlerin OrderDetails'larını tek bir düz liste haline getirir
    // GroupBy: Ürün adına göre detayları grupla (g.Key = Ürün Adı)
    // Sum: Aynı üründen toplam kaç adet satıldığını bul
    private void ShowTodayReports(List<Order> todayOrders, bool isExportOnly = false)
    {
        var totalOrders = todayOrders.Count;
        var totalProfit = todayOrders
            .SelectMany(o => o.OrderDetails)
            .Sum(t => t.Amount * t.Price);

        if (!isExportOnly)
        {
            AnsiConsole.MarkupLine($"[green]Bugünkü Sipariş Sayısı:[/] {totalOrders}");
            AnsiConsole.MarkupLine($"[green]Toplam Ciro:[/] {totalProfit:0.00}");
            return;
        }

        var today = DateTime.Today.ToString("dd.MM.yyyy");
        var headers = new[] { "Tarih", "Sipariş Sayısı", "Toplam Ciro" };
        var rows = new List<string[]>
        {
            new[] { today, totalOrders.ToString(), $"{totalProfit:0.00}" }
        };

        ExportToCsv(headers, rows, "today_summary.csv");
    }



    private void ShowHighSellingProducts(List<Order> orders, bool isExportOnly = false)
    {
        var productStats = orders
            .SelectMany(o => o.OrderDetails)
            .GroupBy(od => od.Product.ProductName)
            .Select(g => new
            {
                ProductName = g.Key,
                Total = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToList();
        
        if (!isExportOnly)
        {
            var table = new Table().RoundedBorder();
            table.AddColumn("Ürün");
            table.AddColumn("Toplam Satış");

            foreach (var item in productStats)
                table.AddRow(item.ProductName, item.Total.ToString());

            AnsiConsole.MarkupLine("\n[bold underline yellow]En Çok Satılan Ürünler:[/]");
            AnsiConsole.Write(table);
            return;
        }
        
        var headers = new[] { "Ürün Adı", "Toplam Satış" };
        var rows = productStats.Select(d => new[] { d.ProductName, d.Total.ToString() }).ToList();
        ExportToCsv(headers, rows, "top_selling_products.csv");
    }

    private void ShowUserStatistics(List<Order> orders, bool isExportOnly = false)
    {
        var userStats = orders
            .GroupBy(o => o.User)
            .Select(g => new
            {
                UserName = $"{g.Key.Name} {g.Key.Surname}",
                OrderCount = g.Count(),
                TotalSpent = g.SelectMany(o => o.OrderDetails).Sum(d => d.Amount * d.Price)
            })
            .OrderByDescending(x => x.TotalSpent)
            .ToList();

        if (!isExportOnly)
        {
            var table = new Table().RoundedBorder();
            table.AddColumn("Kullanıcı");
            table.AddColumn("Sipariş Sayısı");
            table.AddColumn("Toplam Harcama");

            foreach (var item in userStats)
                table.AddRow(item.UserName, item.OrderCount.ToString(), $"{item.TotalSpent:0.00}");

            AnsiConsole.MarkupLine("\n[bold underline cyan]Kullanıcı Bazlı Satışlar:[/]");
            AnsiConsole.Write(table);
            return;
        }
        
        var headers = new[] { "Kullanıcı", "Sipariş Sayısı", "Toplam Harcama" };
        var rows = userStats
            .Select(u => new[] { u.UserName, u.OrderCount.ToString(), $"{u.TotalSpent:0.00}" })
            .ToList();

        ExportToCsv(headers, rows, "user_sales_report.csv");
    }
    /// <summary>
    /// Verilen başlıklar ve satırlardan CSV dosyası üretir.
    /// </summary>
    /// <param name="headers">Başlıklar (örn: Ürün, Satış)</param>
    /// <param name="rows">Satırlar (her biri string[])</param>
    /// <param name="fileName">Dosya adı (örn: rapor.csv)</param>
    public static void ExportToCsv(string[] headers, List<string[]> rows, string fileName)
    {
        var exportDir = Path.Combine(AppContext.BaseDirectory, "ExportCsvFiles");

        // Export klasörü yoksa oluştur
        if (!Directory.Exists(exportDir))
            Directory.CreateDirectory(exportDir);

        var fullPath = Path.Combine(exportDir, fileName);
        bool fileExists = File.Exists(fullPath);

        using var writer = new StreamWriter(fullPath, append: true);

        // Dosya yoksa başlık satırını yaz
        if (!fileExists)
            writer.WriteLine(string.Join(",", headers));

        foreach (var row in rows)
            writer.WriteLine(string.Join(",", row));

        ColoredHelper.Success($"Rapor dışa aktarıldı: {fullPath}");
    }
}