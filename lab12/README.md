# Лабораторная работа №12: Делегаты и события в TodoListApp

## Выполнили
Лукьяненко Даниил, Мироненко Данил, группа 3831.9

## Архитектурные особенности

### Часть 1. Делегаты-обработчики команд в `CommandParser`
- В класс `CommandParser` добавлен словарь делегатов:
  `private static Dictionary<string, Func<string, ICommand>> _commandHandlers = new();`
- В статическом конструкторе/методе `Init()` зарегистрированы обработчики всех команд:
  `_commandHandlers["add"] = ParseAdd;`, `_commandHandlers["delete"] = ParseDelete;` и т.д.
- Метод `Parse(string input)` выделяет имя команды и аргументы, извлекает делегат из словаря по ключу и вызывает его.

### Часть 2. События и разделение обязанностей в `TodoList`
- В класс `TodoList` добавлены события:
  - `public event Action<TodoItem>? OnTodoAdded;`
  - `public event Action<TodoItem>? OnTodoDeleted;`
  - `public event Action<TodoItem>? OnTodoUpdated;`
  - `public event Action<TodoItem>? OnStatusChanged;`
- Прямые вызовы сохранения из команд удалены. При операциях с задачами вызываются события через `?.Invoke(item)`.
- В `AppInfo.SwitchProfile` выполнена подписка `FileManager.SaveTodoList` на события `TodoList`:
  - `todos.OnTodoAdded += FileManager.SaveTodoList;`
  - `todos.OnTodoDeleted += FileManager.SaveTodoList;`
  - `todos.OnTodoUpdated += FileManager.SaveTodoList;`
  - `todos.OnStatusChanged += FileManager.SaveTodoList;`
- Обеспечено автоматическое сохранение данных в CSV через механизм событий без прямой зависимости доменной модели от файла.
