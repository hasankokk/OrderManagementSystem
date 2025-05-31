namespace OrderManagementSystem.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; } // Para birimlerinde yuvarlama hatası vs. olmaması için Decimal kullanıyoruz.
    public int Stock { get; set; }
    
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}