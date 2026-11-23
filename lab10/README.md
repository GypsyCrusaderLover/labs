# Лабораторная работа №10: Паттерн Command со стеками Undo и Redo в TodoListApp

## Выполнили
Лукьяненко Даниил, Мироненко Данил, группа 3831.9

## Архитектурные особенности
1. **Статический контекст `AppInfo`**:
   - `public static TodoList Todos` — глобальный доступ к текущему списку задач;
   - `public static Profile CurrentProfile` — данные текущего пользователя;
   - `public static Stack<ICommand> UndoStack` — стек выполненных команд;
   - `public static Stack<ICommand> RedoStack` — стек отменённых команд.
2. **Расширение `ICommand`**:
   - Добавлен метод `void Unexecute()` для отката изменений.
3. **Реализация `undo` и `redo`**:
   - Изменяющие состояние команды (`AddCommand`, `DeleteCommand`, `UpdateCommand`, `StatusCommand`) наследуют `UndoableCommand` и сохраняют состояние для возврата;
   - Команда `undo` снимает команду из `UndoStack`, вызывает `Unexecute()` и помещает в `RedoStack`;
   - Команда `redo` снимает команду из `RedoStack`, вызывает `Execute()` и помещает обратно в `UndoStack`.
