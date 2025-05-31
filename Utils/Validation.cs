namespace OrderManagementSystem.Utils;

public static class Validation
{
    public static bool IsValidPassword(string password, out string error)
    {
        error = "";

        if (password.Length < 8 || password.Length > 32)
        {
            error = "Şifreniz 8 ile 32 karakter arasında olmalıdır!";
            return false;
        }

        bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false, hasWhiteSpace = false;
        string specialChars = "!@_-+?=&*#$'\"";

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (specialChars.Contains(c)) hasSpecial = true;
            if (char.IsWhiteSpace(c)) hasWhiteSpace = true;
        }

        if (!hasUpper)
        {
            error = "Şifreniz en az 1 büyük harf (A-Z) içermelidir.";
            return false;
        }

        if (!hasLower)
        {
            error = "Şifreniz en az 1 küçük harf (a-z) içermelidir.";
            return false;
        }

        if (!hasDigit)
        {
            error = "Şifreniz en az 1 rakam (0-9) içermelidir.";
            return false;
        }

        if (!hasSpecial)
        {
            error = "Şifreniz en az 1 özel karakter (!@_-+?=&*#$'\") içermelidir.";
            return false;
        }

        if (hasWhiteSpace)
        {
            error = "Şifreniz boşluk içeremez!";
            return false;
        }

        return true;
    }

    public static bool IsValidEmail(string email, out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(email))
        {
            error = "E-posta boş olamaz!";
            return false;
        }

        if (!email.Contains('@') || !email.Contains('.'))
        {
            error = "E-posta '@' ve '.' karakterlerini içermelidir.";
            return false;
        }

        var parts = email.Split('@');
        if (parts.Length != 2 || parts[0].Length < 2 || parts[1].Length < 3)
        {
            error = "E-posta formatı hatalı (örnek: kullanici@site.com)";
            return false;
        }

        if (!parts[1].Contains('.'))
        {
            error = "E-posta domain kısmı nokta içermelidir (örn: gmail.com)";
            return false;
        }

        return true;
    }
}
