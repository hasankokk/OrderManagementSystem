namespace OrderManagementSystem.Helpers;

public class ConsoleMenu(string title)
{
    public string Title { get; } = title;
    private readonly List<MenuOption> _options = [];

    public ConsoleMenu AddOption(string title, Action action)
    {
        _options.Add(new MenuOption(title, action));
        return this;
    }

    public ConsoleMenu AddMenu(string title, Action action)
    {
        _options.Add(new MenuOption(title, action, true));
        return this;
    }

    public void Show(bool isRoot = false)
    {
        while (true)
        {
            Console.Clear();
            ColoredHelper.Title($"\n{Title.ToUpper()}");
            Console.WriteLine();

            var inputMenuNumber = Helper.AskOption(
                _options.Select(x => x.Title).ToArray(),
                cancelOption: isRoot ? "Çıkış" : "Üst Menü"
            );

            Console.Clear();

            if (inputMenuNumber == 0)
            {
                if (isRoot)
                {
                    ColoredHelper.Info("Güle güle...");
                    Thread.Sleep(1000);
                }
                return;
            }

            var selectedOption = _options[inputMenuNumber - 1];
            selectedOption.Action.Invoke();

            if (!selectedOption.SkipWait)
            {
                ColoredHelper.Info("\nMenüye dönmek için bir tuşa basın.");
                Console.ReadKey(true);
            }
        }
    }
}

public class MenuOption(string title, Action action, bool skipWait = false)
{
    public string Title { get; } = title;
    public Action Action { get; } = action;
    public bool SkipWait { get; } = skipWait;
}