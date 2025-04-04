using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Classes
{
    public sealed class TerminalSettingsManager
    {
        private static readonly Lazy<TerminalSettingsManager> _instance =
            new Lazy<TerminalSettingsManager>(() => new TerminalSettingsManager());

        public static TerminalSettingsManager Instance => _instance.Value;

        private const string SettingsPath = @"C:\Users\Professional\AppData\Local\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";

        private TerminalSettingsManager() { }

        public void UpdateSettings(int fontSize, string colorScheme)
        {
            try
            {
                var json = JObject.Parse(File.ReadAllText(SettingsPath));

                json["profiles"]["defaults"]["font"]["size"] = fontSize;
                json["profiles"]["defaults"]["colorScheme"] = colorScheme;

                File.WriteAllText(SettingsPath, json.ToString());
                Console.WriteLine("Настройки терминала обновлены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public void ShowTerminalSettingsMenu()
        {
            Console.WriteLine("\nНастройки терминала:");
            Console.WriteLine("1. Изменить цветовую схему");
            Console.WriteLine("2. Изменить размер шрифта");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Доступные схемы:");
                    Console.WriteLine("1. CGA (Тёмная)");
                    Console.WriteLine("2. One Half Light (Светлая)");
                    Console.Write("Выберите схему: ");

                    var scheme = Console.ReadLine() == "1" ? "CGA" : "One Half Light";
                    UpdateSettings(GetCurrentFontSize(), scheme);
                    break;

                case "2":
                    Console.Write("Введите размер шрифта: ");
                    if (int.TryParse(Console.ReadLine(), out int size))
                    {
                        UpdateSettings(size, GetCurrentColorScheme());
                    }
                    break;
            }
        }

        public int GetCurrentFontSize()
        {
            var json = JObject.Parse(File.ReadAllText(SettingsPath));
            return (int)json["profiles"]["defaults"]["font"]["size"];
        }

        private string GetCurrentColorScheme()
        {
            var json = JObject.Parse(File.ReadAllText(SettingsPath));
            return (string)json["profiles"]["defaults"]["colorScheme"];
        }
    }
}
