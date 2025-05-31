using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using Spectre.Console;

namespace OrderManagementSystem.Helpers;

public class OrderHelper
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;
    private readonly ProductHelper _productHelper;
    private readonly OrderRepository _orderRepository;
    private readonly OrderDetail _orderDetail;

    public OrderHelper(AppDbContext context)
    {
        _context = context;
        _productRepository = new ProductRepository(_context);
        _productHelper = new ProductHelper(_context);
        _orderRepository = new OrderRepository(_context);
        _orderDetail = new OrderDetail();
    }

    public void ShoppingCartManagement(User loggedUser)
    {
        var products = _productRepository.GetProducts();
        var shoppingCart = new List<ShoppingCart>();

        while (true)
        {
            Console.Clear();
            ColoredHelper.Title("Sepet Yönetimi");

            var choice =
                Helper.AskOption(
                    new[] { 
                        "Ürün Ekle", 
                        "Ürün Çıkar",
                        "Miktar Güncelle",
                        "Sepeti Görüntüle",
                        "Siparişi Onayla" },
                    cancelOption: "İptal Et");
            if (choice == 0)
            {
                ColoredHelper.Warning("Sipariş iptal edildi!");
                return;
            }
            switch (choice)
            {
                case 1: AddToShoppingCart(shoppingCart, products);
                    Helper.WaitKey();
                    break;
                case 2: DeleteToShoppingCart(shoppingCart);
                    Helper.WaitKey();
                    break;
                case 3: UpdateQuantityToShoppingCart(shoppingCart, products);
                    Helper.WaitKey();
                    break;
                case 4: DrawProductTable(shoppingCart);
                    Helper.WaitKey();
                    break;
                case 5: SaveToShoppingCart(shoppingCart, loggedUser);
                    Helper.WaitKey();
                    break;
            }
        }
    }

    private void AddToShoppingCart(List<ShoppingCart> shoppingCart, List<Product> products)
    {
        _productHelper.DrawProductTable(products);

        var name = Helper.Ask("Sepete eklenecek ürün adı", true);
        var product = products.FirstOrDefault(p => p.ProductName.ToLower() ==  name.ToLower());

        if (product == null)
        {
            ColoredHelper.Error("Ürün bulunamadı.");
            return;
        }

        var inputQuantity = Helper.AskNumber("Kaç adet almak istiyorsun");
        if (inputQuantity > product.Stock)
        {
            ColoredHelper.Warning("Yetersiz stok.");
            return;
        }

        var existingProduct = shoppingCart.FirstOrDefault(x => x.Product.ProductId == product.ProductId);
        if (existingProduct == null)
        {
            shoppingCart.Add(new ShoppingCart
            {
                Product = product,
                Quantity = inputQuantity
            });
        }
        else
            existingProduct.Quantity += inputQuantity;
        ColoredHelper.Success("Ürün sepete eklendi!");
        Thread.Sleep(300);
    }

    private void DeleteToShoppingCart(List<ShoppingCart> shoppingCart)
    {
        DrawProductTable(shoppingCart);

        var name = Helper.Ask("Sepetten Çıkarılacak Ürün Adı Girin", true).ToLower();
        if (name != shoppingCart.FirstOrDefault().Product.ProductName.ToLower())
        {
            ColoredHelper.Warning("Sepetinizde eşleşen bir ürün bulunamadı.");
            return;
        }
        var selectOrder =  shoppingCart.FirstOrDefault(x => x.Product.ProductName.ToLower() == name.ToLower());
        shoppingCart.Remove(selectOrder);
        ColoredHelper.Success("Ürün başarıyla silindi!");
    }

    private void UpdateQuantityToShoppingCart(List<ShoppingCart> shoppingCart, List<Product> products)
    {
        DrawProductTable(shoppingCart);
        
        var name = Helper.Ask("Güncellemek İstediğiniz Ürün Adı Girin", true).ToLower();
        if (name != shoppingCart.FirstOrDefault().Product.ProductName.ToLower())
        {
            ColoredHelper.Warning("Sepetinizde eşleşen bir ürün bulunamadı.");
            return;
        }
        var selectOrder = shoppingCart.FirstOrDefault(x => x.Product.ProductName.ToLower() == name.ToLower());
        var inputQuantity = Helper.AskNumber("Kaç adet eklenecek? (negatif değerle azaltabilirsin)");
        selectOrder.Quantity += inputQuantity;
        ColoredHelper.Success("Ürün adeti başarıyla güncellendi!");
    }

    private void SaveToShoppingCart(List<ShoppingCart> shoppingCart, User loggedUser)
    {
        DrawProductTable(shoppingCart);

        var input = Helper.AskOption(["Evet"], "Sepetiniz onaylansın mı?", cancelOption: "Hayır");

        if (input == 1)
        {
            var order = new Order
            {
                User = loggedUser,
                Status = OrderStatus.Alındı,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in shoppingCart)
            {
                // Veritabanından canlı ürün çekiyoruz
                var dbProduct = _context.Products.First(p => p.ProductId == item.Product.ProductId);

                // Stok kontrolü yapıyoruz
                if (dbProduct.Stock < item.Quantity)
                {
                    ColoredHelper.Error($"'{dbProduct.ProductName}' için yeterli stok yok!");
                    return;
                }
                dbProduct.Stock -= item.Quantity;
                
                var orderDetail = new OrderDetail
                {
                    ProductId = dbProduct.ProductId,
                    Amount = item.Quantity,
                    Price = dbProduct.Price
                };

                order.OrderDetails.Add(orderDetail);
            }

            _orderRepository.AddOrder(order);

            ColoredHelper.Success($"Sayın {loggedUser.Name} {loggedUser.Surname}, siparişiniz " +
                                  $"\n{order.Created} tarihinde işleme alınmıştır.");
            return;
        }

        ColoredHelper.Error("Siparişiniz onaylanmadı!");
    }
    public void ShowOrderHistory(User user)
    {
        var orders = _orderRepository.GetOrdersByCustomer(user.Id);

        if (!orders.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Henüz hiç sipariş vermediniz.[/]");
            return;
        }

        var choice = Helper.AskOption(
            new[] { "Aktif Siparişler", "Teslim Edilen Siparişler" },
            "Hangi siparişleri görüntülemek istiyorsunuz?",
            cancelOption: "Geri Dön"
        );

        if (choice == 0)
            return;

        var filteredOrders = choice switch
        {
            1 => orders.Where(o => o.Status != OrderStatus.TeslimEdildi).ToList(),
            2 => orders.Where(o => o.Status == OrderStatus.TeslimEdildi).ToList(),
            _ => new List<Order>()
        };

        if (!filteredOrders.Any())
        {
            AnsiConsole.MarkupLine("[gray]Bu kategoride sipariş bulunamadı.[/]");
            return;
        }

        var heading = choice == 1 ? "[bold green]Aktif Siparişler[/]" : "[bold grey]Teslim Edilen Siparişler[/]";
        AnsiConsole.MarkupLine(heading);

        foreach (var order in filteredOrders)
            DrawOrderTable(order, isDimmed: (choice == 2));
    }

    private void DrawOrderTable(Order order, bool isDimmed = false)
    {
        string colorize(string label, string value, string color) =>
            isDimmed
                ? $"[gray]{label} {value}[/]"
                : $"[{color}]{label}[/] [white]{value}[/]";

        // Başlık satırı
        AnsiConsole.MarkupLine(
            colorize("Sipariş No:", $"{order.OrderId}", "blue") + "  " +
            colorize("Tarih:", $"{order.Created:dd.MM.yyyy HH:mm}", "blue") + "  " +
            colorize("Durum:", $"{order.Status}", "blue"));

        // Tablo
        var table = new Table().RoundedBorder();

        void addColumn(string text, string color) =>
            table.AddColumn(isDimmed ? $"[gray]{text}[/]" : $"[{color}]{text}[/]");

        addColumn("Ürün Adı", "aqua");
        addColumn("Adet", "yellow");
        addColumn("Birim Fiyat", "green");
        addColumn("Toplam", "white");

        foreach (var detail in order.OrderDetails)
        {
            var total = detail.Amount * detail.Price;

            string wrap(string val) => isDimmed ? $"[gray]{val}[/]" : val;

            table.AddRow(
                wrap(detail.Product.ProductName),
                wrap($"{detail.Amount}"),
                wrap($"{detail.Price:0.00}"),
                wrap($"{total:0.00}")
            );
        }

        AnsiConsole.Write(table);
    }

    private void DrawProductTable(List<ShoppingCart> products)
    {
        var table = new Table().RoundedBorder();
        table.AddColumn("[blue]Ürün Adı[/]");
        table.AddColumn("[yellow]Adet[/]");

        foreach (var product in products)
        {
            table.AddRow(
                $"[aqua]{product.Product.ProductName}[/]",
                $"[white]{product.Quantity}[/]"
            );
        }
        AnsiConsole.Write(table);
    }
}