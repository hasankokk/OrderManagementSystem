using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class ProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public bool UpdateStock(string name, int stock)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductName.ToLower() == name.ToLower());
        if (product == null)
            return false;
        product.Stock += stock;
        
        if (product.Stock < 0)
            product.Stock = 0;
        _context.SaveChanges();
        return true;
    }

    public bool DeleteProduct(string name)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductName.ToLower() == name.ToLower());
        if (product == null)
            return false;
        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }
    public List<Product> GetProducts()
    {
        return _context.Products.ToList();
    }

    public bool ProductExists(string name)
    {
        return _context.Products.Any(p => p.ProductName.ToLower() == name.ToLower());
    }
}