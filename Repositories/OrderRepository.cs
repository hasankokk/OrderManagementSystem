using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class OrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public List<Order> GetOrdersStatus(OrderStatus status)
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.User)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.Created)
            .ToList();
    }

    public List<Order> GetActiveOrders()
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
            .Include(x => x.User)
            .Where(x => x.Status != OrderStatus.TeslimEdildi)
            .OrderBy(o => o.Created)
            .ToList();
    }
    public List<Order> GetOrdersByCustomer(int customerId)
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(o => o.Product)
            .Where(x => x.UserId == customerId)
            .ToList();
    }

    public List<Order> GetOrdersByDate(DateTime date)
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.Created == date.Date)
            .ToList();
    }

    public List<Order> GetOrdersWithProductDetails()
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(o => o.Product)
            .Include(x => x.User)
            .ToList();
    }
    public bool AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return true;
    }

    public void UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
            return;
        order.Status = status;
        _context.SaveChanges();
    }
}