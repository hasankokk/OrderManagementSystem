using System.Globalization;


namespace OrderManagementSystem.Helpers;

public static class Helper
{
    public static string? Ask(string question, bool isRequired = false, string validationMsg = "Bu alanı boş bırakılamaz.")
    {
        string? response;
        do
        {
            ColoredHelper.Question($"{question}: ", false);
            response = Console.ReadLine();

            if (isRequired && string.IsNullOrWhiteSpace(response))
            {
                ColoredHelper.Error(validationMsg);
            }

        } while (isRequired && string.IsNullOrWhiteSpace(response));

        return response?.Trim();
    }

    public static string AskPassword(string question, string validationMsg = "Bu alanı boş bırakılamaz.")
    {
        string password;
        do
        {
            ColoredHelper.Question($"{question}: ", false);
            password = ReadSecretLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                ColoredHelper.Error(validationMsg);
            }

        } while (string.IsNullOrWhiteSpace(password));

        return password;
    }

    public static DateTime AskDate(string question, string validationMsg = "Geçerli bir tarih giriniz (örn: 31.12.2025)")
    {
        while (true)
        {
            var response = Ask(question, true);
            if (DateTime.TryParse(response, out var result))
                return result;

            ColoredHelper.Error(validationMsg);
        }
    }

    public static int AskNumber(string question, string validationMsg = "Bir sayı girmelisin.")
    {
        while (true)
        {
            var response = Ask(question, true);
            if (int.TryParse(response, out var result))
                return result;

            ColoredHelper.Error(validationMsg);
        }
    }

    public static int AskOption(string[] options, string? question = null, string? cancelOption = null)
    {
        if (options.Length == 0)
            throw new ArgumentException($"{nameof(options)} içinde en az bir seçenek olmalı.", nameof(options));

        if (!string.IsNullOrWhiteSpace(question))
            ColoredHelper.Question(question);

        for (int i = 0; i < options.Length; i++)
            ColoredHelper.Question($"{i + 1}. {options[i]}");

        var optionsStartFrom = 1;
        if (cancelOption != null)
        {
            ColoredHelper.Question($"0. {cancelOption}");
            optionsStartFrom = 0;
        }

        while (true)
        {
            var input = AskNumber($"Seçiminiz ({optionsStartFrom}-{options.Length})");
            if (input >= optionsStartFrom && input <= options.Length)
                return input;

            ColoredHelper.Error("Geçersiz seçim yaptınız.");
        }
    }

    public static decimal AskDecimal(string question, string validationMsg = "Geçerli bir fiyat giriniz.")
    {
        while (true)
        {
            var response = Ask(question, true);

            if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ||
                decimal.TryParse(response, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            ColoredHelper.Error(validationMsg);
        }
    }

    private static string ReadSecretLine()
    {
        var line = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && line.Length > 0)
            {
                line = line[..^1];
                Console.Write("\b \b");
                continue;
            }

            if (!IsSecureChar(key.KeyChar) || char.IsControl(key.KeyChar))
                continue;

            line += key.KeyChar;
            Console.Write("*");

        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return line;
    }

    private static bool IsSecureChar(char c) =>
        char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c);
    
    public static void WaitKey(string msg = "Menüye dönmek için bir tuşa basın...")
    {
        ColoredHelper.Info(msg);
        Console.ReadKey(true);
    }

}
