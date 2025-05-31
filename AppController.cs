using OrderManagementSystem.Data;
using OrderManagementSystem.Helpers;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Routing;
using OrderManagementSystem.Utils;

namespace OrderManagementSystem;

public class AppController
{
    private readonly AppDbContext _context;
    private readonly Auth _auth;
    private readonly UserRepository _userRepository;
    private readonly MenuRouter _menuRouter;
    private User? _userLoggedIn;
    
    public AppController(AppDbContext context)
    { 
        _context = context;
        _userRepository = new UserRepository(_context);
        _auth = new Auth(_userRepository);
        _menuRouter = new MenuRouter(_context);
    }

    public void RunApp(string title)
    {
        var user = new User
        {
            Name = "admin",
            Surname = "admin",
            Password = PasswordHash.HashPassword("admin"),
            Email = "admin@admin.com",
            Role = Role.Admin,
        };
        if (_userRepository.GetUserByEmail(user.Email) == null)
            _userRepository.RegisterUser(user);

        WelcomeScreen(title);

        while (_userLoggedIn == null)
        {
            var option = Helper.AskOption(
                new[] { "Giriş Yap", "Kayıt Ol" },
                cancelOption: "Çıkış"
            );

            switch (option)
            {
                case 1:
                    LoginFlow();
                    if (_userLoggedIn != null)
                        _menuRouter.RouteToRoleMenu(_context, _userLoggedIn);
                    break;
                case 2:
                    RegisterFlow();
                    break;
                case 0:
                    ColoredHelper.Info("Çıkış yapılıyor...");
                    return;
            }

            Console.Clear();
        }
    }
    private void RegisterFlow()
    {
        while (true)
        {
            ColoredHelper.Title("Yeni Kullanıcı Kaydı");

            var email = Helper.Ask("E-posta", true);
            var password = Helper.AskPassword("Şifre");
            var name = Helper.Ask("İsim", true);
            var surname = Helper.Ask("Soyisim", true);

            var status = _auth.Register(email!, password, out var errorMessage);

            if (status == RegisterStatus.Success)
            {
                _auth.RegistrationCompletion(email!, password, name!, surname!, Role.Kullanıcı);
                Thread.Sleep(2000);
                break;
            }

            ColoredHelper.Error(errorMessage);

            var retry = Helper.AskOption(
                new[] { "Yeniden dene", "İptal et" },
                question: "İşleme devam etmek istiyor musun?"
            );

            if (retry == 2)
            {
                ColoredHelper.Info("Kayıt iptal edildi.");
                break;
            }
            Console.Clear();
        }
    }
    private void LoginFlow()
    {
        while (true)
        {
            Console.Clear();
            var email = Helper.Ask("E-posta", true);
            var password = Helper.AskPassword("Şifre");

            var loginStatus = _auth.Login(email!, out var errorMessage);

            if (loginStatus == LoginStatus.Success)
            {
                var user = _userRepository.GetUserByEmail(email!);
                if (user != null && PasswordHash.VerifyPassword(password, user.Password))
                {
                    _userLoggedIn = user;
                    ColoredHelper.Success("Giriş başarılı!");
                    Thread.Sleep(1000);
                    return;
                }

                ColoredHelper.Error("Şifreniz hatalı!");
            }
            else
            {
                ColoredHelper.Error(errorMessage);
            }

            // Ortak retry akışı
            var retry = Helper.AskOption(
                new[] { "Yeniden dene", "İptal et" },
                question: "İşleme devam etmek istiyor musun?"
            );

            if (retry == 2)
            {
                ColoredHelper.Info("Giriş iptal edildi.");
                break;
            }
        }
    }
    private void WelcomeScreen(string title)
    {
        ColoredHelper.Title("===================================");
        ColoredHelper.Title($"   {title}   ");
        ColoredHelper.Title("===================================");
    }
    
}