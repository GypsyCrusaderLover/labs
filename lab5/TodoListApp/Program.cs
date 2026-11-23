using System;
using System.Collections.Generic;
using System.Text;

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
                else if (trimmedInput.StartsWith("view", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessView(trimmedInput);
                }
                else if (trimmedInput.StartsWith("read", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessRead(trimmedInput);
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
            Console.WriteLine("  help                     - выводит справку");
            Console.WriteLine("  profile                  - выводит данные профиля");
            Console.WriteLine("  add \"текст\"              - однострочное добавление задачи");
            Console.WriteLine("  add -m / --multiline     - многострочный ввод (завершение строкой !end)");
            Console.WriteLine("  view [флаги]             - вывод задач с фильтрацией и выбором колонок");
            Console.WriteLine("    --all (-a), --done (-d), --undone (-u)");
            Console.WriteLine("    --index (-i), --status (-s), --date (-t), например: view -is, view -aut");
            Console.WriteLine("  read <idx>               - просмотр полного текста задачи с переносами строк");
            Console.WriteLine("  done <idx>               - отметка задачи выполненной");
            Console.WriteLine("  delete <idx>             - удаление задачи со сдвигом элементов");
            Console.WriteLine("  update <idx> \"текст\"     - обновление текста задачи");
            Console.WriteLine("  exit                     - завершение работы программы");
        }

        private static void ProcessProfile(string? firstName, string? lastName, int birthYear)
        {
            Console.WriteLine($"{firstName} {lastName}, {birthYear}");
        }

        private static void ProcessAdd(string command)
        {
            bool isMultiline = command.Contains("--multiline") || command.Contains("-m");

            string taskText;
            if (isMultiline)
            {
                Console.WriteLine("Введите строки задачи (для завершения введите !end):");
                var lines = new List<string>();
                while (true)
                {
                    Console.Write("> ");
                    string? line = Console.ReadLine();
                    if (line == null || line.Trim() == "!end")
                    {
                        break;
                    }
                    lines.Add(line);
                }

                if (lines.Count == 0)
                {
                    Console.WriteLine("Ошибка: многострочная задача не может быть пустой.");
                    return;
                }

                taskText = string.Join("\n", lines);
            }
            else
            {
                taskText = ExtractText(command);
                if (string.IsNullOrWhiteSpace(taskText))
                {
                    Console.WriteLine("Ошибка: укажите текст задачи в кавычках.");
                    return;
                }
            }

            EnsureCapacity();

            todos[count] = taskText;
            statuses[count] = false;
            dates[count] = DateTime.Now;
            count++;

            Console.WriteLine($"Задача добавлена: [{count - 1}]");
        }

        private static void ProcessView(string command)
        {
            if (count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            string[] tokens = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            bool filterDoneOnly = false;
            bool filterUndoneOnly = false;
            bool explicitIndex = false;
            bool explicitStatus = false;
            bool explicitDate = false;
            bool hasExplicitDisplayFlags = false;

            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i].ToLower();
                if (token == "--all" || token == "-a")
                {
                    filterDoneOnly = false;
                    filterUndoneOnly = false;
                }
                else if (token == "--done" || token == "-d")
                {
                    filterDoneOnly = true;
                    filterUndoneOnly = false;
                }
                else if (token == "--undone" || token == "-u")
                {
                    filterUndoneOnly = true;
                    filterDoneOnly = false;
                }
                else if (token == "--index")
                {
                    explicitIndex = true;
                    hasExplicitDisplayFlags = true;
                }
                else if (token == "--status")
                {
                    explicitStatus = true;
                    hasExplicitDisplayFlags = true;
                }
                else if (token == "--date")
                {
                    explicitDate = true;
                    hasExplicitDisplayFlags = true;
                }
                else if (token.StartsWith("-") && !token.StartsWith("--"))
                {
                    for (int c = 1; c < token.Length; c++)
                    {
                        char ch = token[c];
                        if (ch == 'a') { filterDoneOnly = false; filterUndoneOnly = false; }
                        else if (ch == 'd') { filterDoneOnly = true; filterUndoneOnly = false; }
                        else if (ch == 'u') { filterUndoneOnly = true; filterDoneOnly = false; }
                        else if (ch == 'i') { explicitIndex = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 's') { explicitStatus = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 't') { explicitDate = true; hasExplicitDisplayFlags = true; }
                    }
                }
            }

            bool showIndex = !hasExplicitDisplayFlags || explicitIndex;
            bool showStatus = !hasExplicitDisplayFlags || explicitStatus;
            bool showDate = !hasExplicitDisplayFlags || explicitDate;

            for (int i = 0; i < count; i++)
            {
                if (filterDoneOnly && !statuses[i]) continue;
                if (filterUndoneOnly && statuses[i]) continue;

                var sb = new StringBuilder();
                if (showIndex)
                {
                    sb.Append($"[{i}] ");
                }

                string preview = todos[i].Replace("\n", " ");
                if (preview.Length > 40)
                {
                    preview = preview.Substring(0, 37) + "...";
                }
                sb.Append($"\"{preview}\" ");

                if (showStatus)
                {
                    sb.Append($"[{(statuses[i] ? StatusDone : StatusNotDone)}] ");
                }

                if (showDate)
                {
                    sb.Append(dates[i].ToString("yyyy-MM-dd HH:mm:ss"));
                }

                Console.WriteLine(sb.ToString().TrimEnd());
            }
        }

        private static void ProcessRead(string command)
        {
            if (TryParseIndex(command, out int idx))
            {
                if (idx >= 0 && idx < count)
                {
                    string status = statuses[idx] ? StatusDone : StatusNotDone;
                    string date = dates[idx].ToString("yyyy-MM-dd HH:mm:ss");

                    Console.WriteLine($"--- Задача [{idx}] ---");
                    Console.WriteLine($"Статус: {status}");
                    Console.WriteLine($"Дата:   {date}");
                    Console.WriteLine("Текст задачи:");
                    Console.WriteLine(todos[idx]);
                    Console.WriteLine("---------------------");
                }
                else
                {
                    Console.WriteLine("Ошибка: индекс выходит за пределы диапазона.");
                }
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
