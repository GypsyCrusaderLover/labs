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
            string profilePath = Path.Combine(dataDirPath, "profile.txt");
            string todoPath = Path.Combine(dataDirPath, "todo.csv");

            FileManager.EnsureDataDirectory(dataDirPath);

            Profile profile;
            TodoList todoList;

            if (File.Exists(profilePath) && new FileInfo(profilePath).Length > 0)
            {
                profile = FileManager.LoadProfile(profilePath);
                Console.WriteLine($"Загружен профиль: {profile.GetInfo()}");
            }
            else
            {
                Console.Write("Введите имя: ");
                string firstName = Console.ReadLine() ?? "";

                Console.Write("Введите фамилию: ");
                string lastName = Console.ReadLine() ?? "";

                Console.Write("Введите год рождения: ");
                int.TryParse(Console.ReadLine(), out int birthYear);

                profile = new Profile(firstName, lastName, birthYear);
                FileManager.SaveProfile(profile, profilePath);
            }

            if (File.Exists(todoPath))
            {
                todoList = FileManager.LoadTodos(todoPath);
                Console.WriteLine($"Загружено задач: {todoList.Count}");
            }
            else
            {
                todoList = new TodoList();
                FileManager.SaveTodos(todoList, todoPath);
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

                ICommand command = CommandParser.Parse(input, todoList, profile, todoPath, profilePath);
                command.Execute();
            }
        }
    }
}
