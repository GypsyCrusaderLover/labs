# Лабораторная работа №6: Объектно-ориентированная архитектура приложения TodoListApp

## Выполнили
Лукьяненко Даниил, Мироненко Данил, группа 3831.9

## Описание структуры
1. Создан класс `TodoItem` (файл `TodoItem.cs`):
   - Свойства: `Text`, `IsDone`, `LastUpdate`;
   - Методы: `MarkDone()`, `UpdateText(string newText)`, `GetShortInfo()`, `GetFullInfo()`.
2. Создан класс `TodoList` (файл `TodoList.cs`):
   - Приватный массив `_items` и приватный метод `IncreaseArray()`;
   - Публичные методы: `Add(TodoItem item)`, `Delete(int index)`, `GetItem(int index)`, `View(bool showIndex, bool showDone, bool showDate, bool? doneFilter)`.
3. Создан класс `Profile` (файл `Profile.cs`):
   - Свойства: `FirstName`, `LastName`, `BirthYear`;
   - Метод: `GetInfo()` для вывода информации о пользователе и его возрасте.
4. Выполнена инкапсуляция всех внутренних данных, доступ реализован строго через свойства и методы.
