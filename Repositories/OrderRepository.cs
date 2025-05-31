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
    
    public List<Order> GetOrders() //Mutfak ve Admin Rol'ü İçin
    {
        return _context.Orders.ToList();
    }

    public List<Order> GetOrdersByCustomer(int customerId)
    {
        return _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(o => o.Product)
            .Where(x => x.UserId == customerId)
            .ToList();
    }
    public bool AddOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return true;
    }
}