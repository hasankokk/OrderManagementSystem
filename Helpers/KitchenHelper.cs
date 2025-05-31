using OrderManagementSystem.Data;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Utils;

namespace OrderManagementSystem.Helpers;

public class KitchenHelper
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly Auth _auth;

    public KitchenHelper(AppDbContext context)
    {
        _context = context;
        _userRepository = new UserRepository(_context);
        _auth = new Auth(_userRepository);
    }
    public void Show()
    {
        new ConsoleMenu("Kullanıcı Paneli")
            .AddOption("Sipariş Görüntüle", ()=> Console.WriteLine("Admin Paneli"))
            .AddOption("Sipariş Güncelle", () => Console.WriteLine("Admin Paneli"))
            .AddOption("Bilgilerimi Görüntüle", () => Console.WriteLine("Admin Paneli"))
            .AddOption("Şifre Değiştir", () => Console.WriteLine("Admin Paneli"))
            .Show(isRoot: true);
    }
}