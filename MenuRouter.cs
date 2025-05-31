using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Helpers;

namespace OrderManagementSystem.Routing;

public class MenuRouter
{
    private readonly AppDbContext _context;
    private readonly AdminHelper _adminHelper;
    private readonly UserHelper _userHelper;
    private readonly KitchenHelper _kitchenHelper;

    public MenuRouter(AppDbContext context)
    {
        _context = context;
        _adminHelper = new AdminHelper(_context);
        _userHelper = new UserHelper(_context);
        _kitchenHelper = new KitchenHelper(_context);
    }
    public void RouteToRoleMenu(AppDbContext context, User user)
    {
        switch (user.Role)
        {
            case Role.Admin:
                ColoredHelper.Success($"Hoş geldiniz, {user.Name} ({user.Role})");
                _adminHelper.Show();
                break;
            case Role.Mutfak:
                ColoredHelper.Success($"Hoş geldiniz, {user.Name} ({user.Role})");
                _kitchenHelper.Show();
                break;

            case Role.Kullanıcı:
                ColoredHelper.Success($"Hoş geldiniz, {user.Name} ({user.Role})");
                _userHelper.Show(user);
                break;
            default:
                ColoredHelper.Error("Tanımlanamayan bir rol ile giriş yapıldı.");
                break;
        }
    }
}