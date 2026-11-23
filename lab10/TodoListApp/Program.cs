using System;
using System.IO;

namespace TodoListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили: Лукьяненко Даниил, Мироненко Данил, гр. 3831.9");

            string dataDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            AppInfo.ProfileFilePath = Path.Combine(dataDirPath, "profile.txt");
            AppInfo.TodoFilePath = Path.Combine(dataDirPath, "todo.csv");

            FileManager.EnsureDataDirectory(dataDirPath);

            if (File.Exists(AppInfo.ProfileFilePath) && new FileInfo(AppInfo.ProfileFilePath).Length > 0)
            {
                AppInfo.CurrentProfile = FileManager.LoadProfile(AppInfo.ProfileFilePath);
                Console.WriteLine($"Загружен профиль: {AppInfo.CurrentProfile.GetInfo()}");
            }
            else
            {
                Console.Write("Введите имя: ");
                string firstName = Console.ReadLine() ?? "";

                Console.Write("Введите фамилию: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Введите год рождения: ");
                int.TryParse(Console.ReadLine(), out int birthYear);

                AppInfo.CurrentProfile = new Profile(firstName, lastName, birthYear);
                FileManager.SaveProfile(AppInfo.CurrentProfile, AppInfo.ProfileFilePath);
            }

            if (File.Exists(AppInfo.TodoFilePath))
            {
                AppInfo.Todos = FileManager.LoadTodos(AppInfo.TodoFilePath);
                Console.WriteLine($"Загружено задач: {AppInfo.Todos.Count}");
            }
            else
            {
                AppInfo.Todos = new TodoList();
                FileManager.SaveTodos(AppInfo.Todos, AppInfo.TodoFilePath);
            }

            Console.WriteLine("Программа TodoList запущена. Введите команду (help для справки):");

            while (true)
            {
                Console.Write("> ");
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
    }
}
