using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class UserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public void RegisterUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public bool UpdatePassword(string pass, User user)
    {
        var findUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        findUser.Password = pass;
        _context.SaveChanges();
        return true;
    }
    public List<User> GetUsers()
    {
        if (_context.Users.Count() == 0)
            return new List<User>();
        return _context.Users.ToList();
    }
    public bool EmailExists(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

}