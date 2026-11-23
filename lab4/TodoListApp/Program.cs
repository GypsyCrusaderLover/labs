using System;

namespace TodoListApp
{
    internal class Program
    {
        private const int InitialCapacity = 2;
        private const string StatusDone = "сделано";
        private const string StatusNotDone = "не сделано";

        private static string[] todos = new string[InitialCapacity];
        private static bool[] statuses = new bool[InitialCapacity];
        private static DateTime[] dates = new DateTime[InitialCapacity];
        private static int count = 0;

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
                    ProcessHelp();
                }
                else if (trimmedInput.Equals("profile", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessProfile(firstName, lastName, birthYear);
                }
                else if (trimmedInput.StartsWith("add", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessAdd(trimmedInput);
                }
                else if (trimmedInput.Equals("view", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessView();
                }
                else if (trimmedInput.StartsWith("done", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessDone(trimmedInput);
                }
                else if (trimmedInput.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessDelete(trimmedInput);
                }
                else if (trimmedInput.StartsWith("update", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessUpdate(trimmedInput);
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
                }
            }
        }

        private static void ProcessHelp()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  help                 - выводит список всех команд");
            Console.WriteLine("  profile              - выводит данные профиля");
            Console.WriteLine("  add \"текст\"          - добавляет задачу со статусом 'не сделано'");
            Console.WriteLine("  view                 - выводит задачи (<индекс> <текст> <статус> <дата>)");
            Console.WriteLine("  done <idx>           - отмечает задачу выполненной");
            Console.WriteLine("  delete <idx>         - удаляет задачу со сдвигом элементов влево");
            Console.WriteLine("  update <idx> \"текст\" - обновляет текст задачи и дату");
            Console.WriteLine("  exit                 - завершает выполнение программы");
        }

        private static void ProcessProfile(string? firstName, string? lastName, int birthYear)
        {
            Console.WriteLine($"{firstName} {lastName}, {birthYear}");
        }

        private static void ProcessAdd(string command)
        {
            string taskText = ExtractText(command);
            if (string.IsNullOrWhiteSpace(taskText))
            {
                Console.WriteLine("Ошибка: укажите текст задачи в кавычках.");
                return;
            }

            EnsureCapacity();

            todos[count] = taskText;
            statuses[count] = false;
            dates[count] = DateTime.Now;
            count++;

            Console.WriteLine($"Задача добавлена: [{count - 1}] \"{taskText}\"");
        }

        private static void ProcessView()
        {
            if (count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                string statusText = statuses[i] ? StatusDone : StatusNotDone;
                string dateText = dates[i].ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine($"{i} \"{todos[i]}\" {statusText} {dateText}");
            }
        }

        private static void ProcessDone(string command)
        {
            if (TryParseIndex(command, out int idx))
            {
                if (idx >= 0 && idx < count)
                {
                    statuses[idx] = true;
                    dates[idx] = DateTime.Now;
                    Console.WriteLine($"Задача [{idx}] отмечена выполненной.");
                }
                else
                {
                    Console.WriteLine("Ошибка: индекс выходит за пределы диапазона.");
                }
            }
        }

        private static void ProcessDelete(string command)
        {
            if (TryParseIndex(command, out int idx))
            {
                if (idx >= 0 && idx < count)
                {
                    for (int i = idx; i < count - 1; i++)
                    {
                        todos[i] = todos[i + 1];
                        statuses[i] = statuses[i + 1];
                        dates[i] = dates[i + 1];
                    }

                    count--;
                    todos[count] = null!;
                    statuses[count] = false;
                    dates[count] = default;

                    Console.WriteLine($"Задача [{idx}] удалена.");
                }
                else
                {
                    Console.WriteLine("Ошибка: индекс выходит за пределы диапазона.");
                }
            }
        }

        private static void ProcessUpdate(string command)
        {
            string[] parts = command.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3 || !int.TryParse(parts[1], out int idx))
            {
                Console.WriteLine("Ошибка: формат команды update <idx> \"new_text\"");
                return;
            }

            if (idx < 0 || idx >= count)
            {
                Console.WriteLine("Ошибка: индекс выходит за пределы диапазона.");
                return;
            }

            string newText = parts[2].Trim('"', ' ');
            todos[idx] = newText;
            dates[idx] = DateTime.Now;
            Console.WriteLine($"Задача [{idx}] обновлена: \"{newText}\"");
        }

        private static void EnsureCapacity()
        {
            if (count >= todos.Length)
            {
                int newLength = todos.Length * 2;
                string[] newTodos = new string[newLength];
                bool[] newStatuses = new bool[newLength];
                DateTime[] newDates = new DateTime[newLength];

                for (int i = 0; i < todos.Length; i++)
                {
                    newTodos[i] = todos[i];
                    newStatuses[i] = statuses[i];
                    newDates[i] = dates[i];
                }

                todos = newTodos;
                statuses = newStatuses;
                dates = newDates;
            }
        }

        private static string ExtractText(string command)
        {
            string[] quoteParts = command.Split(new[] { '"' }, StringSplitOptions.None);
            if (quoteParts.Length >= 2)
            {
                return quoteParts[1];
            }

            string[] spaceParts = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return spaceParts.Length > 1 ? spaceParts[1].Trim('"') : "";
        }

        private static bool TryParseIndex(string command, out int index)
        {
            string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[1], out index))
            {
                return true;
            }
            Console.WriteLine("Ошибка: укажите корректный целочисленный индекс.");
            index = -1;
            return false;
        }
    }
}
