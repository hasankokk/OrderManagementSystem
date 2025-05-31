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
    private readonly ProductHelper _productHelper;
    private readonly Auth _auth;

    public AdminHelper(AppDbContext context)
    {
        _context = context;
        _userRepository = new UserRepository(_context);
        _productHelper = new ProductHelper(_context);
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
            .AddOption("Satış Raporları", () => Console.WriteLine("Yapılacak"))
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
}