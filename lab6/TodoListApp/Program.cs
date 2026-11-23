using System;
using System.Collections.Generic;

namespace TodoListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Работу выполнили: Лукьяненко Даниил, Мироненко Данил, гр. 3831.9");

            Console.Write("Введите имя: ");
            string firstName = Console.ReadLine() ?? "";

            Console.Write("Введите фамилию: ");
            string lastName = Console.ReadLine() ?? "";

            Console.Write("Введите год рождения: ");
            int.TryParse(Console.ReadLine(), out int birthYear);

            Profile profile = new Profile(firstName, lastName, birthYear);
            TodoList todoList = new TodoList();

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
                    Console.WriteLine("  help                 - вывод справки");
                    Console.WriteLine("  profile              - вывод информации о профиле");
                    Console.WriteLine("  add \"текст\"          - добавление задачи");
                    Console.WriteLine("  add -m               - многострочное добавление задачи (!end для завершения)");
                    Console.WriteLine("  view [флаги]         - табличный просмотр задач");
                    Console.WriteLine("  read <idx>           - подробный просмотр задачи");
                    Console.WriteLine("  done <idx>           - отметка о выполнении");
                    Console.WriteLine("  delete <idx>         - удаление задачи");
                    Console.WriteLine("  update <idx> \"текст\" - изменение текста задачи");
                    Console.WriteLine("  exit                 - выход из программы");
                }
                else if (trimmedInput.Equals("profile", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(profile.GetInfo());
                }
                else if (trimmedInput.StartsWith("add", StringComparison.OrdinalIgnoreCase))
                {
                    bool isMultiline = trimmedInput.Contains("--multiline") || trimmedInput.Contains("-m");
                    string text;
                    if (isMultiline)
                    {
                        Console.WriteLine("Введите строки задачи (!end для завершения):");
                        var lines = new List<string>();
                        while (true)
                        {
                            Console.Write("> ");
                            string? line = Console.ReadLine();
                            if (line == null || line.Trim() == "!end") break;
                            lines.Add(line);
                        }
                        if (lines.Count == 0)
                        {
                            Console.WriteLine("Ошибка: пустая задача.");
                            continue;
                        }
                        text = string.Join("\n", lines);
                    }
                    else
                    {
                        text = ExtractText(trimmedInput);
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            Console.WriteLine("Ошибка: укажите текст задачи в кавычках.");
                            continue;
                        }
                    }

                    TodoItem item = new TodoItem(text);
                    todoList.Add(item);
                    Console.WriteLine($"Задача добавлена: [{todoList.Count - 1}]");
                }
                else if (trimmedInput.StartsWith("view", StringComparison.OrdinalIgnoreCase))
                {
                    ParseViewFlags(trimmedInput, out bool showIdx, out bool showDone, out bool showDate, out bool? doneFilter);
                    todoList.View(showIdx, showDone, showDate, doneFilter);
                }
                else if (trimmedInput.StartsWith("read", StringComparison.OrdinalIgnoreCase))
                {
                    if (TryParseIndex(trimmedInput, out int idx))
                    {
                        var item = todoList.GetItem(idx);
                        if (item != null)
                        {
                            Console.WriteLine($"--- Задача [{idx}] ---");
                            Console.WriteLine(item.GetFullInfo());
                            Console.WriteLine("---------------------");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: задача с таким индексом не найдена.");
                        }
                    }
                }
                else if (trimmedInput.StartsWith("done", StringComparison.OrdinalIgnoreCase))
                {
                    if (TryParseIndex(trimmedInput, out int idx))
                    {
                        var item = todoList.GetItem(idx);
                        if (item != null)
                        {
                            item.MarkDone();
                            Console.WriteLine($"Задача [{idx}] отмечена выполненной.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: задача с таким индексом не найдена.");
                        }
                    }
                }
                else if (trimmedInput.StartsWith("delete", StringComparison.OrdinalIgnoreCase))
                {
                    if (TryParseIndex(trimmedInput, out int idx))
                    {
                        if (todoList.Delete(idx))
                        {
                            Console.WriteLine($"Задача [{idx}] удалена.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: индекс вне диапазона.");
                        }
                    }
                }
                else if (trimmedInput.StartsWith("update", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = trimmedInput.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3 && int.TryParse(parts[1], out int idx))
                    {
                        var item = todoList.GetItem(idx);
                        if (item != null)
                        {
                            string newText = parts[2].Trim('"', ' ');
                            item.UpdateText(newText);
                            Console.WriteLine($"Задача [{idx}] обновлена.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: задача не найдена.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: формат команды update <idx> \"новый текст\"");
                    }
                }
                else
                {
                    Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
                }
            }
        }

        private static void ParseViewFlags(string command, out bool showIndex, out bool showDone, out bool showDate, out bool? doneFilter)
        {
            string[] tokens = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            doneFilter = null;
            bool explicitIndex = false;
            bool explicitStatus = false;
            bool explicitDate = false;
            bool hasExplicitDisplayFlags = false;

            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i].ToLower();
                if (token == "--all" || token == "-a") doneFilter = null;
                else if (token == "--done" || token == "-d") doneFilter = true;
                else if (token == "--undone" || token == "-u") doneFilter = false;
                else if (token == "--index") { explicitIndex = true; hasExplicitDisplayFlags = true; }
                else if (token == "--status") { explicitStatus = true; hasExplicitDisplayFlags = true; }
                else if (token == "--date") { explicitDate = true; hasExplicitDisplayFlags = true; }
                else if (token.StartsWith("-") && !token.StartsWith("--"))
                {
                    for (int c = 1; c < token.Length; c++)
                    {
                        char ch = token[c];
                        if (ch == 'a') doneFilter = null;
                        else if (ch == 'd') doneFilter = true;
                        else if (ch == 'u') doneFilter = false;
                        else if (ch == 'i') { explicitIndex = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 's') { explicitStatus = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 't') { explicitDate = true; hasExplicitDisplayFlags = true; }
                    }
                }
            }

            showIndex = !hasExplicitDisplayFlags || explicitIndex;
            showDone = !hasExplicitDisplayFlags || explicitStatus;
            showDate = !hasExplicitDisplayFlags || explicitDate;
        }

        private static string ExtractText(string command)
        {
            string[] quoteParts = command.Split(new[] { '"' }, StringSplitOptions.None);
            if (quoteParts.Length >= 2) return quoteParts[1];
            string[] spaceParts = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return spaceParts.Length > 1 ? spaceParts[1].Trim('"') : "";
        }

        private static bool TryParseIndex(string command, out int index)
        {
            string[] parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && int.TryParse(parts[1], out index)) return true;
            Console.WriteLine("Ошибка: укажите корректный целочисленный индекс.");
            index = -1;
            return false;
        }
    }
}
