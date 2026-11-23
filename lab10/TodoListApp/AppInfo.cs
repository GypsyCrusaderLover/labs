using System.Collections.Generic;

namespace TodoListApp
{
    public static class AppInfo
    {
        public static TodoList Todos { get; set; } = null!;
        public static Profile CurrentProfile { get; set; } = null!;
        public static Stack<ICommand> UndoStack { get; } = new();
        public static Stack<ICommand> RedoStack { get; } = new();
        public static string TodoFilePath { get; set; } = "";
        public static string ProfileFilePath { get; set; } = "";
    }
}
