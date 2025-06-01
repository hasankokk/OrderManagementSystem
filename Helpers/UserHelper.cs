using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Utils;
using Spectre.Console;

namespace OrderManagementSystem.Helpers;

public class UserHelper
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly OrderHelper _orderHelper;
    //private readonly Auth _auth;

    public UserHelper(AppDbContext context)
    {
        _context = context;
        _userRepository = new UserRepository(_context);
        _orderHelper = new OrderHelper(_context);
        //_auth = new Auth(_userRepository);
    }

    public void Show(User user)
    {
        new ConsoleMenu("Kullanıcı Paneli")
            .AddOption("Sipariş Oluştur", ()=> _orderHelper.ShoppingCartManagement(user))
            .AddOption("Sipariş Takibi ve Geçmişi", () => _orderHelper.ShowOrderHistory(user))
            .AddOption("Bilgilerimi Görüntüle", () => ShowUserInfo(user))
            .AddOption("Şifre Değiştir", () => ChangePassword(user))
            .Show(isRoot: true);
    }
    public void ShowUserInfo(User user)
    {
        AnsiConsole.MarkupLine("[bold underline green]\nKullanıcı Bilgileri[/]");

        var table = new Table().RoundedBorder();
        table.AddColumn("Alan");
        table.AddColumn("Değer");

        table.AddRow("İsim", user.Name);
        table.AddRow("Soyisim", user.Surname);
        table.AddRow("E-posta", user.Email);
        table.AddRow("Rol", user.Role.ToString());

        AnsiConsole.Write(table);
    }

    public void ChangePassword(User user)
    {
        ColoredHelper.Title("Şifre Değiştirme");

        var currentPassword = Helper.AskPassword("Mevcut şifrenizi girin");
        if (!Validation.IsValidPassword(currentPassword, out var error))
        {
            ColoredHelper.Error(error);
            return;
        }
        if (!PasswordHash.VerifyPassword(currentPassword, user.Password))
        {
            ColoredHelper.Error("Hatalı şifre!");
            return;
        }

        var newPassword = Helper.AskPassword("Yeni şifre girin");
        var confirm = Helper.AskPassword("Yeni şifreyi tekrar girin");

        if (newPassword != confirm)
        {
            ColoredHelper.Error("Şifreler eşleşmiyor!");
            return;
        }
        var pass = _userRepository.UpdatePassword(PasswordHash.HashPassword(newPassword), user);
        if (!pass)
        {
            ColoredHelper.Error("Şifre değiştirme başarısız!");
            return;
        }
        ColoredHelper.Success("Şifreniz başarıyla değiştirildi.");
    }
}