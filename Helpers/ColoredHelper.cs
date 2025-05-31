namespace OrderManagementSystem.Helpers;

public static class ColoredHelper
{
    public static void Show(string msg, ConsoleColor color = ConsoleColor.White, bool newLine = true)
    {
        Console.ForegroundColor = color;
        if (newLine)
            Console.WriteLine(msg);
        else
            Console.Write(msg);
        Console.ResetColor();
    }

    public static void Info(string msg, bool newLine = true) => Show(msg, ConsoleColor.Cyan, newLine);
    public static void Success(string msg, bool newLine = true) => Show(msg, ConsoleColor.Green, newLine);
    public static void Error(string msg, bool newLine = true) => Show(msg, ConsoleColor.Red, newLine);
    public static void Warning(string msg, bool newLine = true) => Show(msg, ConsoleColor.Yellow, newLine);
    public static void Menu(string msg, bool newLine = true) => Show(msg, ConsoleColor.DarkMagenta, newLine);
    public static void Title(string msg, bool newLine = true) => Show(msg, ConsoleColor.DarkCyan, newLine);
    public static void Question(string msg, bool newLine = true) => Show(msg, ConsoleColor.Blue, newLine);
    public static void Highlight(string msg, bool newLine = true) => Show(msg, ConsoleColor.DarkYellow, newLine);
}