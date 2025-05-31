using OrderManagementSystem.Data;

namespace OrderManagementSystem;

class Program
{
    static void Main(string[] args)
    {
        using var context = new AppDbContext();
        var appController = new AppController(context);
        string title = "Sipariş Yönetim Sistemi";
        appController.RunApp(title);
    }
}