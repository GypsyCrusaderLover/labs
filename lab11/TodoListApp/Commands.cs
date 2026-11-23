using System;
using System.Collections.Generic;

namespace TodoListApp
{
    public abstract class UndoableCommand : ICommand
    {
        public abstract void Execute();
        public abstract void Unexecute();
    }

    public class HelpCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Доступные команды:");
            Console.WriteLine("  help                     - вывод справки");
            Console.WriteLine("  profile                  - просмотр текущего профиля");
            Console.WriteLine("  profile -o / --out       - выход из профиля (смена пользователя)");
            Console.WriteLine("  add \"текст\"              - добавление задачи");
            Console.WriteLine("  add -m                   - многострочное добавление (!end для завершения)");
            Console.WriteLine("  view [флаги]             - табличный просмотр задач");
            Console.WriteLine("  read <idx>               - подробный просмотр задачи");
            Console.WriteLine("  status <idx> <status>    - изменение статуса задачи");
            Console.WriteLine("  delete <idx>             - удаление задачи");
            Console.WriteLine("  update <idx> \"текст\"     - изменение текста задачи");
            Console.WriteLine("  undo                     - отмена последнего действия");
            Console.WriteLine("  redo                     - повтор отменённого действия");
            Console.WriteLine("  exit                     - выход из программы");
        }
        public void Unexecute() { }
    }

    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Выход из программы.");
            Environment.Exit(0);
        }
        public void Unexecute() { }
    }

    public class ProfileCommand : ICommand
    {
        public bool IsLogout { get; set; }

        public void Execute()
        {
            if (IsLogout)
            {
                Console.WriteLine("Выполнен выход из профиля.");
                AppInfo.SwitchProfile(null);
            }
            else
            {
                if (AppInfo.CurrentProfile != null)
                {
                    Console.WriteLine(AppInfo.CurrentProfile.GetInfo());
                }
                else
                {
                    Console.WriteLine("Профиль не выбран.");
                }
            }
        }
        public void Unexecute() { }
    }

    public class AddCommand : UndoableCommand
    {
        public string Text { get; set; } = "";
        public bool IsMultiline { get; set; }
        private int _addedIndex = -1;
        private TodoItem? _createdItem;

        public override void Execute()
        {
            if (AppInfo.CurrentTodos == null || !AppInfo.CurrentProfileId.HasValue)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }

            if (_createdItem == null)
            {
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

                _createdItem = new TodoItem(taskText);
            }

            AppInfo.CurrentTodos.Add(_createdItem);
            _addedIndex = AppInfo.CurrentTodos.Count - 1;
            Console.WriteLine($"Задача добавлена: [{_addedIndex}]");

            string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
            FileManager.SaveTodos(AppInfo.CurrentTodos, path);
        }

        public override void Unexecute()
        {
            if (AppInfo.CurrentTodos != null && AppInfo.CurrentProfileId.HasValue &&
                _addedIndex >= 0 && _addedIndex < AppInfo.CurrentTodos.Count)
            {
                AppInfo.CurrentTodos.Delete(_addedIndex);
                Console.WriteLine($"Отмена: удалена задача [{_addedIndex}].");
                string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                FileManager.SaveTodos(AppInfo.CurrentTodos, path);
            }
        }
    }

    public class ViewCommand : ICommand
    {
        public bool ShowIndex { get; set; } = true;
        public bool ShowStatus { get; set; } = true;
        public bool ShowDate { get; set; } = true;
        public TodoStatus? StatusFilter { get; set; } = null;

        public void Execute()
        {
            if (AppInfo.CurrentTodos == null)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }
            AppInfo.CurrentTodos.View(ShowIndex, ShowStatus, ShowDate, StatusFilter);
        }
        public void Unexecute() { }
    }

    public class ReadCommand : ICommand
    {
        public int Index { get; set; }

        public void Execute()
        {
            if (AppInfo.CurrentTodos == null)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }

            var item = AppInfo.CurrentTodos.GetItem(Index);
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
        public void Unexecute() { }
    }

    public class StatusCommand : UndoableCommand
    {
        public int Index { get; set; }
        public TodoStatus NewStatus { get; set; }
        private TodoStatus _oldStatus;
        private bool _success;

        public override void Execute()
        {
            if (AppInfo.CurrentTodos == null || !AppInfo.CurrentProfileId.HasValue)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }

            var item = AppInfo.CurrentTodos.GetItem(Index);
            if (item != null)
            {
                _oldStatus = item.Status;
                item.SetStatus(NewStatus);
                _success = true;
                Console.WriteLine($"Статус задачи [{Index}] изменён на: {NewStatus}.");
                string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                FileManager.SaveTodos(AppInfo.CurrentTodos, path);
            }
            else
            {
                _success = false;
                Console.WriteLine("Ошибка: задача с указанным индексом не найдена.");
            }
        }

        public override void Unexecute()
        {
            if (_success && AppInfo.CurrentTodos != null && AppInfo.CurrentProfileId.HasValue)
            {
                var item = AppInfo.CurrentTodos.GetItem(Index);
                if (item != null)
                {
                    item.SetStatus(_oldStatus);
                    Console.WriteLine($"Отмена: возвращён статус {_oldStatus} для задачи [{Index}].");
                    string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                    FileManager.SaveTodos(AppInfo.CurrentTodos, path);
                }
            }
        }
    }

    public class DeleteCommand : UndoableCommand
    {
        public int Index { get; set; }
        private TodoItem? _deletedItem;
        private bool _success;

        public override void Execute()
        {
            if (AppInfo.CurrentTodos == null || !AppInfo.CurrentProfileId.HasValue)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }

            var item = AppInfo.CurrentTodos.GetItem(Index);
            if (item != null)
            {
                _deletedItem = item;
                _success = AppInfo.CurrentTodos.Delete(Index);
                if (_success)
                {
                    Console.WriteLine($"Задача [{Index}] удалена.");
                    string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                    FileManager.SaveTodos(AppInfo.CurrentTodos, path);
                }
            }
            else
            {
                _success = false;
                Console.WriteLine("Ошибка: индекс вне диапазона.");
            }
        }

        public override void Unexecute()
        {
            if (_success && _deletedItem != null && AppInfo.CurrentTodos != null && AppInfo.CurrentProfileId.HasValue)
            {
                AppInfo.CurrentTodos.Insert(Index, _deletedItem);
                Console.WriteLine($"Отмена: восстановлена задача [{Index}]: \"{_deletedItem.Text}\".");
                string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                FileManager.SaveTodos(AppInfo.CurrentTodos, path);
            }
        }
    }

    public class UpdateCommand : UndoableCommand
    {
        public int Index { get; set; }
        public string NewText { get; set; } = "";
        private string _oldText = "";
        private bool _success;

        public override void Execute()
        {
            if (AppInfo.CurrentTodos == null || !AppInfo.CurrentProfileId.HasValue)
            {
                Console.WriteLine("Ошибка: пользователь не авторизован.");
                return;
            }

            var item = AppInfo.CurrentTodos.GetItem(Index);
            if (item != null)
            {
                _oldText = item.Text;
                item.UpdateText(NewText);
                _success = true;
                Console.WriteLine($"Задача [{Index}] обновлена.");
                string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                FileManager.SaveTodos(AppInfo.CurrentTodos, path);
            }
            else
            {
                _success = false;
                Console.WriteLine("Ошибка: задача не найдена.");
            }
        }

        public override void Unexecute()
        {
            if (_success && AppInfo.CurrentTodos != null && AppInfo.CurrentProfileId.HasValue)
            {
                var item = AppInfo.CurrentTodos.GetItem(Index);
                if (item != null)
                {
                    item.UpdateText(_oldText);
                    Console.WriteLine($"Отмена: восстановлен прежний текст задачи [{Index}].");
                    string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                    FileManager.SaveTodos(AppInfo.CurrentTodos, path);
                }
            }
        }
    }

    public class UndoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.UndoStack.Count > 0)
            {
                var cmd = AppInfo.UndoStack.Pop();
                cmd.Unexecute();
                AppInfo.RedoStack.Push(cmd);
            }
            else
            {
                Console.WriteLine("Стек отмены пуст.");
            }
        }
        public void Unexecute() { }
    }

    public class RedoCommand : ICommand
    {
        public void Execute()
        {
            if (AppInfo.RedoStack.Count > 0)
            {
                var cmd = AppInfo.RedoStack.Pop();
                cmd.Execute();
                AppInfo.UndoStack.Push(cmd);
            }
            else
            {
                Console.WriteLine("Стек повтора пуст.");
            }
        }
        public void Unexecute() { }
    }

    public class UnknownCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Неизвестная команда. Введите help для списка команд.");
        }
        public void Unexecute() { }
    }
}
