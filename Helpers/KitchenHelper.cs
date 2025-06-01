using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Utils;
using Spectre.Console;

namespace OrderManagementSystem.Helpers;

public class KitchenHelper
{
    private readonly AppDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly OrderRepository _orderRepository;
    private readonly UserHelper _userHelper;
    private readonly Auth _auth;

    public KitchenHelper(AppDbContext context)
    {
        _context = context;
        _userRepository = new UserRepository(_context);
        _orderRepository = new OrderRepository(_context);
        _userHelper = new UserHelper(_context);
        _auth = new Auth(_userRepository);
    }
    public void Show(User user)
    {
        new ConsoleMenu("Mutfak Paneli")
            .AddOption("Hazırlanacak Siparişleri Listele", ListActiveOrders)
            .AddOption("Sipariş Durumunu Güncelle", UpdateOrderStatus)
            .AddOption("Teslim Edilenleri Görüntüle", DeliveredOrders)
            .AddOption("Bilgilerimi Görüntüle", () => _userHelper.ShowUserInfo(user))
            .AddOption("Şifre Değiştir", () => _userHelper.ChangePassword(user))
            .Show(isRoot: true);
    }
    public void ListActiveOrders()
    {
        var orders = _orderRepository.GetActiveOrders();

        if (!orders.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Hazırlanacak sipariş bulunamadı.[/]");
            return;
        }

        foreach (var order in orders)
        {
            AnsiConsole.MarkupLine($"[bold blue]\nSipariş No:[/] {order.OrderId}  " +
                                   $"[blue]Tarih:[/] {order.Created:dd.MM.yyyy HH:mm}  " +
                                   $"[green]Müşteri:[/] {order.User.Name} {order.User.Surname}  " +
                                   $"[blue]Durum:[/] {order.Status}");

            DrawOrderDetailTable(order);
        }
    }


    private void UpdateOrderStatus()
    {
        var orders = _orderRepository.GetActiveOrders();

        if (!orders.Any())
        {
            ColoredHelper.Error("Aktif sipariş yok!");
            return;
        }

        var orderOptions = orders.Select(o => $"#{o.OrderId} | {o.User.Name} {o.User.Surname} | {o.Status}").ToArray();
        var selectedIndex = Helper.AskOption(orderOptions, "Hangi siparişin durumunu güncellemek istersiniz");
        var selectedOrder = orders[selectedIndex - 1];

        /*
            Aşağıdaki switch expression yapısı, bir sonraki sipariş durumunu otomatik belirler.
            Bu kullanım, if-else-if zincirinin sadeleştirilmiş halidir:

            Örneği:

            OrderStatus nextStatus;
            if (selectedOrder.Status == OrderStatus.Alındı)
                nextStatus = OrderStatus.Hazırlanıyor;
            else if (selectedOrder.Status == OrderStatus.Hazırlanıyor)
                nextStatus = OrderStatus.Hazır;
            else if (selectedOrder.Status == OrderStatus.Hazır)
                nextStatus = OrderStatus.TeslimEdildi;
            else
                nextStatus = selectedOrder.Status;

            Aşağıdaki switch ifadesi ile aynı mantıktadır.
        */
        OrderStatus nextStatus = selectedOrder.Status switch
        {
            OrderStatus.Alındı => OrderStatus.Hazırlanıyor,
            OrderStatus.Hazırlanıyor => OrderStatus.Hazır,
            OrderStatus.Hazır => OrderStatus.TeslimEdildi,
            _ => selectedOrder.Status
        };

        _orderRepository.UpdateOrderStatus(selectedOrder.OrderId, nextStatus);
        ColoredHelper.Success($"Sipariş #{selectedOrder.OrderId} -> {nextStatus} durumuna güncellendi.");
    }


    private void DeliveredOrders()
    {
        var orders = _orderRepository.GetOrdersStatus(OrderStatus.TeslimEdildi);
        if (!orders.Any())
        {
            ColoredHelper.Error("Teslim edilmiş sipariş yok!");
            return;
        }

        foreach (var order in orders)
        {
            AnsiConsole.MarkupLine($"[bold grey]\nSipariş No:[/] {order.OrderId}  " +
                                   $"[grey]Tarih:[/] {order.Created:dd.MM.yyyy HH:mm}  " +
                                   $"[grey]Müşteri:[/] {order.User.Name} {order.User.Surname}");

            DrawOrderDetailTable(order, isDimmed: true);
        }
    }
    private void DrawOrderDetailTable(Order order, bool isDimmed = false)
    {
        var table = new Table().RoundedBorder();

        string wrap(string value) => isDimmed ? $"[gray]{value}[/]" : value;

        table.AddColumn(wrap("Ürün Adı"));
        table.AddColumn(wrap("Adet"));
        table.AddColumn(wrap("Birim Fiyat"));
        table.AddColumn(wrap("Toplam"));

        foreach (var detail in order.OrderDetails)
        {
            var total = detail.Amount * detail.Price;

            table.AddRow(
                wrap(detail.Product.ProductName),
                wrap($"{detail.Amount}"),
                wrap($"{detail.Price:0.00}"),
                wrap($"{total:0.00}")
            );
        }

        AnsiConsole.Write(table);
    }

}