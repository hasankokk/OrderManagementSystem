namespace OrderManagementSystem.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Alındı;
    
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}