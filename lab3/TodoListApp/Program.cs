using System;

namespace TodoListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили: Лукьяненко Даниил, Мироненко Данил, гр. 3831.9");

            Console.Write("Введите имя: ");
            string? firstName = Console.ReadLine();

            Console.Write("Введите фамилию: ");
            string? lastName = Console.ReadLine();

            Console.Write("Введите год рождения: ");
            string? birthYearInput = Console.ReadLine();
            int.TryParse(birthYearInput, out int birthYear);

            string[] todos = new string[2];
            int todoCount = 0;

            Console.WriteLine("Программа TodoList запущена. Введите команду (help для справки):");

            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                string trimmedInput = input.Trim();

                if (trimmedInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Выход из программы.");
                    break;
                }
                else if (trimmedInput.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Доступные команды:");
                    Console.WriteLine("  help    - выводит список всех доступных команд с кратким описанием");
                    Console.WriteLine("  profile - выводит данные пользователя (<Имя> <Фамилия>, <Год рождения>)");
                    Console.WriteLine("  add \"текст задачи\" - добавляет новую задачу");
                    Console.WriteLine("  view    - выводит все непустые задачи из массива");
                    Console.WriteLine("  exit    - завершает выполнение программы");
                }
                else if (trimmedInput.Equals("profile", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{firstName} {lastName}, {birthYear}");
                }
                else if (trimmedInput.StartsWith("add ", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = trimmedInput.Split(new[] { '"' }, StringSplitOptions.None);
                    string taskText = "";
                    if (parts.Length >= 2)
                    {
                        taskText = parts[1];
                    }
                    else
                    {
                        string[] spaceParts = trimmedInput.Split(new[] { ' ' }, 2);
                        if (spaceParts.Length > 1)
                        {
                            taskText = spaceParts[1].Trim('"');
                        }
                    }

                    if (string.IsNullOrWhiteSpace(taskText))
                    {
                        Console.WriteLine("Ошибка: пустой текст задачи.");
                        continue;
                    }

                    if (todoCount >= todos.Length)
                    {
                        string[] newTodos = new string[todos.Length * 2];
                        for (int i = 0; i < todos.Length; i++)
                        {
                            newTodos[i] = todos[i];
                        }
                        todos = newTodos;
                    }

                    todos[todoCount] = taskText;
                    todoCount++;
                    Console.WriteLine($"Задача добавлена: \"{taskText}\"");
                }
                else if (trimmedInput.Equals("view", StringComparison.OrdinalIgnoreCase))
                {
                    bool hasItems = false;
                    for (int i = 0; i < todos.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(todos[i]))
                        {
                            Console.WriteLine($"[{i}] {todos[i]}");
                            hasItems = true;
                        }
                    }

                    if (!hasItems)
                    {
                        Console.WriteLine("Список задач пуст.");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
                }
            }
        }
    }
}
