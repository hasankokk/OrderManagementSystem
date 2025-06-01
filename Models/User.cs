using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementSystem.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int UserId { get; set; }
    [Required, MaxLength(50)] 
    public string Name { get; set; } = "";
    [Required, MaxLength(50)] 
    public string Surname { get; set; } = "";
    [Required, MaxLength(100)] 
    public string Email { get; set; } = "";
    [Required] 
    public string Password { get; set; } = "";
    public Role Role { get; set; } = Role.Kullanıcı;
    
}