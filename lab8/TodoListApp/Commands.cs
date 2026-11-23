using System;
using System.Collections.Generic;

namespace TodoListApp
{
    public class HelpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  help                 - вывод справки");
            Console.WriteLine("  profile              - вывод информации о профиле");
            Console.WriteLine("  add \"текст\"          - добавление задачи");
            Console.WriteLine("  add -m               - многострочное добавление (!end для завершения)");
            Console.WriteLine("  view [флаги]         - табличный просмотр задач");
            Console.WriteLine("  read <idx>           - подробный просмотр задачи");
            Console.WriteLine("  done <idx>           - отметка о выполнении (с автосохранением)");
            Console.WriteLine("  delete <idx>         - удаление задачи (с автосохранением)");
            Console.WriteLine("  update <idx> \"текст\" - изменение текста задачи (с автосохранением)");
            Console.WriteLine("  exit                 - выход из программы");
        }
    }

    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Выход из программы.");
            Environment.Exit(0);
        }
    }

    public class ProfileCommand : ICommand
    {
        public Profile? Profile { get; set; }
        public string ProfileFilePath { get; set; } = "";

        public void Execute()
        {
            if (Profile != null)
            {
                Console.WriteLine(Profile.GetInfo());
                if (!string.IsNullOrEmpty(ProfileFilePath))
                {
                    FileManager.SaveProfile(Profile, ProfileFilePath);
                }
            }
        }
    }

    public class AddCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public string Text { get; set; } = "";
        public bool IsMultiline { get; set; }
        public string TodoFilePath { get; set; } = "";

        public void Execute()
        {
            if (TodoList == null) return;

            string taskText = Text;
            if (IsMultiline)
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
                    return;
                }
                taskText = string.Join("\n", lines);
            }

            if (string.IsNullOrWhiteSpace(taskText))
            {
                Console.WriteLine("Ошибка: текст задачи не может быть пустым.");
                return;
            }

            var item = new TodoItem(taskText);
            TodoList.Add(item);
            Console.WriteLine($"Задача добавлена: [{TodoList.Count - 1}]");

            if (!string.IsNullOrEmpty(TodoFilePath))
            {
                FileManager.SaveTodos(TodoList, TodoFilePath);
            }
        }
    }

    public class ViewCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public bool ShowIndex { get; set; } = true;
        public bool ShowDone { get; set; } = true;
        public bool ShowDate { get; set; } = true;
        public bool? DoneFilter { get; set; } = null;

        public void Execute()
        {
            TodoList?.View(ShowIndex, ShowDone, ShowDate, DoneFilter);
        }
    }

    public class ReadCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; }

        public void Execute()
        {
            var item = TodoList?.GetItem(Index);
            if (item != null)
            {
                Console.WriteLine($"--- Задача [{Index}] ---");
                Console.WriteLine(item.GetFullInfo());
                Console.WriteLine("---------------------");
            }
            else
            {
                Console.WriteLine("Ошибка: задача с таким индексом не найдена.");
            }
        }
    }

    public class DoneCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; }
        public string TodoFilePath { get; set; } = "";

        public void Execute()
        {
            var item = TodoList?.GetItem(Index);
            if (item != null)
            {
                item.MarkDone();
                Console.WriteLine($"Задача [{Index}] отмечена выполненной.");

                if (!string.IsNullOrEmpty(TodoFilePath))
                {
                    FileManager.SaveTodos(TodoList!, TodoFilePath);
                }
            }
            else
            {
                Console.WriteLine("Ошибка: задача не найдена.");
            }
        }
    }

    public class DeleteCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; }
        public string TodoFilePath { get; set; } = "";

        public void Execute()
        {
            if (TodoList != null && TodoList.Delete(Index))
            {
                Console.WriteLine($"Задача [{Index}] удалена.");

                if (!string.IsNullOrEmpty(TodoFilePath))
                {
                    FileManager.SaveTodos(TodoList, TodoFilePath);
                }
            }
            else
            {
                Console.WriteLine("Ошибка: индекс вне диапазона.");
            }
        }
    }

    public class UpdateCommand : ICommand
    {
        public TodoList? TodoList { get; set; }
        public int Index { get; set; }
        public string NewText { get; set; } = "";
        public string TodoFilePath { get; set; } = "";

        public void Execute()
        {
            var item = TodoList?.GetItem(Index);
            if (item != null)
            {
                item.UpdateText(NewText);
                Console.WriteLine($"Задача [{Index}] обновлена.");

                if (!string.IsNullOrEmpty(TodoFilePath))
                {
                    FileManager.SaveTodos(TodoList!, TodoFilePath);
                }
            }
            else
            {
                Console.WriteLine("Ошибка: задача не найдена.");
            }
        }
    }

    public class UnknownCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
        }
    }
}
