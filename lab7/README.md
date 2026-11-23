# Лабораторная работа №7: Паттерн Command и парсер команд в TodoListApp

## Выполнили
Лукьяненко Даниил, Мироненко Данил, группа 3831.9

## Архитектурные изменения
1. **Интерфейс `ICommand`**:
   - Метод `void Execute()`.
2. **Конкретные команды**:
   - `AddCommand`, `ViewCommand`, `DoneCommand`, `DeleteCommand`, `UpdateCommand`, `ReadCommand`, `ProfileCommand`, `HelpCommand`, `ExitCommand`.
   - Флаги команд оформлены как свойства `bool`.
   - Параметры и зависимости (`TodoList`, `Profile`) передаются как свойства команды.
3. **Парсер `CommandParser`**:
   - Статический метод `public static ICommand Parse(string inputString, TodoList todoList, Profile profile)`.
   - Разбирает команду и аргументы/флаги, инкапсулирует создание объектов команд.
4. **Главный цикл `Main`**:
   - Делегирует обработку: считывание ввода -> `CommandParser.Parse()` -> `command.Execute()`.
