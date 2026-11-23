using System;
using System.IO;
using System.Linq;

namespace TodoListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили: Лукьяненко Даниил, Мироненко Данил, гр. 3831.9");

            AppInfo.DataDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            AppInfo.ProfileCsvPath = Path.Combine(AppInfo.DataDirPath, "profile.csv");

            FileManager.EnsureDataDirectory(AppInfo.DataDirPath);
            AppInfo.Profiles = FileManager.LoadProfiles(AppInfo.ProfileCsvPath);

            while (true)
            {
                if (!AppInfo.CurrentProfileId.HasValue)
                {
                    HandleProfileSelection();
                    continue;
                }

                Console.Write($"[{AppInfo.CurrentProfile?.Login}]> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                ICommand command = CommandParser.Parse(input);
                command.Execute();

                if (command is UndoableCommand)
                {
                    AppInfo.UndoStack.Push(command);
                    AppInfo.RedoStack.Clear();
                }
            }
        }

        private static void HandleProfileSelection()
        {
            Console.WriteLine("\n--- Управление профилями ---");
            Console.Write("Войти в существующий профиль? [y/n]: ");
            string? choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y")
            {
                if (AppInfo.Profiles.Count == 0)
                {
                    Console.WriteLine("Нет зарегистрированных профилей. Создайте новый профиль.");
                    CreateNewProfile();
                    return;
                }

                Console.Write("Введите логин: ");
                string login = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine()?.Trim() ?? "";

                var profile = AppInfo.Profiles.FirstOrDefault(p =>
                    p.Login.Equals(login, StringComparison.OrdinalIgnoreCase) && p.Password == password);

                if (profile != null)
                {
                    AppInfo.SwitchProfile(profile.Id);
                    Console.WriteLine($"Успешный вход! Текущий пользователь: {profile.GetInfo()}");
                }
                else
                {
                    Console.WriteLine("Ошибка: неверный логин или пароль.");
                }
            }
            else
            {
                CreateNewProfile();
            }
        }

        private static void CreateNewProfile()
        {
            Console.WriteLine("--- Регистрация нового профиля ---");
            Console.Write("Введите логин: ");
            string login = Console.ReadLine()?.Trim() ?? "";

            if (AppInfo.Profiles.Any(p => p.Login.Equals(login, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Ошибка: пользователь с таким логином уже существует.");
                return;
            }

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Введите имя: ");
            string firstName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Введите фамилию: ");
            string lastName = Console.ReadLine()?.Trim() ?? "";

            Console.Write("Введите год рождения: ");
            int.TryParse(Console.ReadLine(), out int birthYear);

            Guid newId = Guid.NewGuid();
            var newProfile = new Profile(newId, login, password, firstName, lastName, birthYear);

            AppInfo.Profiles.Add(newProfile);
            FileManager.SaveProfiles(AppInfo.Profiles, AppInfo.ProfileCsvPath);

            AppInfo.SwitchProfile(newId);
            string userTodoPath = AppInfo.GetUserTodoPath(newId);
            FileManager.SaveTodos(AppInfo.CurrentTodos!, userTodoPath);

            Console.WriteLine($"Профиль успешно создан! ID: {newId}");
        }
    }
}
