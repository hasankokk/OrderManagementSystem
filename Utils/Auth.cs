using OrderManagementSystem.Data;
using OrderManagementSystem.Helpers;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories;

namespace OrderManagementSystem.Utils;

public enum RegisterStatus
{
    Success,
    EmailExists,
    InvalidEmail,
    InvalidPassword
}

public enum LoginStatus
{
    Success,
    InvalidEmail,
}
public class Auth
{
    private readonly UserRepository _userRepository;

    public Auth( UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public RegisterStatus Register(string email, string password, out string errorMessage)
    {
        errorMessage = "";

        if (!Validation.IsValidEmail(email, out var emailError))
        {
            errorMessage = emailError;
            return RegisterStatus.InvalidEmail;
        }

        if (!Validation.IsValidPassword(password, out var passError))
        {
            errorMessage = passError;
            return RegisterStatus.InvalidPassword;
        }

        if (_userRepository.EmailExists(email))
        {
            errorMessage = "Bu e-posta zaten kayıtlı.";
            return RegisterStatus.EmailExists;
        }

        // Gerçek kayıt işlemleri burada yapılabilir.
        errorMessage = "Kayıt başarılı!";
        return RegisterStatus.Success;
    }

    public LoginStatus Login(string email, out string errorMessage)
    {
        errorMessage = "";
        if (!Validation.IsValidEmail(email, out var emailError))
        {
            errorMessage = emailError;
            return LoginStatus.InvalidEmail;
        }
        errorMessage = "";
        return LoginStatus.Success;
    }
    public void RegistrationCompletion(string email, string password, string name, string surname, Role role)
    {
        var user = new User
        {
            Name = name,
            Surname = surname,
            Email = email,
            Password = PasswordHash.HashPassword(password),
            Role = role
        };
        _userRepository.RegisterUser(user);
        ColoredHelper.Success("Kayıt Başarılı!");
    }
}