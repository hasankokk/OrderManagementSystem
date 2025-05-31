using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;
using Spectre.Console;

namespace OrderManagementSystem.Helpers;

public class ProductHelper
{
    private readonly AppDbContext _context;
    private readonly ProductRepository  _productRepository;

    public ProductHelper(AppDbContext context)
    {
        _context = context;
        _productRepository = new ProductRepository(_context);
    }

    public void AddProduct()
    {
        ColoredHelper.Title("Yeni Ürün Ekle");
        
        var input = Helper.Ask("Ürün Adı", true);
        var price = Helper.AskDecimal("Ürün Fiyatı");
        var stock = Helper.AskNumber("Stok Adeti");
        var product = new Product
        {
            ProductName = input,
            Price = price,
            Stock = stock,
        };

        if (!_productRepository.ProductExists(input))
        {
            _productRepository.AddProduct(product);
            ColoredHelper.Success("Ürün başarıyla eklendi.");
            return;
        }
        ColoredHelper.Error("Ürün zaten mevcut.");
    }

    public void UpdateStock()
    {
        var products = _productRepository.GetProducts();

        if (!products.Any())
        {
            AnsiConsole.MarkupLine("[red]Güncellenecek ürün bulunamadı![/]");
            return;
        }

        DrawProductTable(products);
        // Güncellenecek ürün adı sor
        var name = Helper.Ask("Stok güncellemek istediğiniz ürünün adı", true);

        // Eklenecek/azaltılacak miktar (pozitif veya negatif olabilir)
        var amount = Helper.AskNumber("Kaç adet eklenecek? (negatif değerle azaltabilirsin)");

        var success = _productRepository.UpdateStock(name, amount);

        if (success)
            ColoredHelper.Success("Stok güncellendi.");
        else
            ColoredHelper.Error("Ürün bulunamadı.");
    }

    public void DeleteProduct()
    {
        var products = _productRepository.GetProducts();

        if (!products.Any())
        {
            AnsiConsole.MarkupLine("[red]Güncellenecek ürün bulunamadı![/]");
            return;
        }

        DrawProductTable(products);

        var name = Helper.Ask("Silmek istediğiniz ürünün adı", true);
        var success = _productRepository.DeleteProduct(name);
        if (success)
        {
            ColoredHelper.Success("Ürün silindi!");
            return;
        }
        ColoredHelper.Error("Ürün silinemedi!");
    }

    public void StockReport()
    {
        var products = _productRepository.GetProducts();

        if (!products.Any())
        {
            ColoredHelper.Error("Ürün bulunamadı!");
            return;
        }
        DrawProductTable(products);
    }
    public void ListProducts()
    {
        var products = _productRepository.GetProducts();

        if (!products.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Hiç ürün bulunamadı![/]");
            return;
        }

        // Tablo oluşturuluyor
       DrawProductTable(products);
    }
    public void DrawProductTable(List<Product> products)
    {
        var table = new Table().RoundedBorder();
        table.AddColumn("[blue]Ürün Adı[/]");
        table.AddColumn("[green]Fiyat (₺)[/]");
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
    }

}