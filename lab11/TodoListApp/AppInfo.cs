using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TodoListApp
{
    public static class AppInfo
    {
        public static List<Profile> Profiles { get; set; } = new();
        public static Guid? CurrentProfileId { get; set; }
        public static Dictionary<Guid, TodoList> UserTodos { get; set; } = new();

        public static Profile? CurrentProfile =>
            CurrentProfileId.HasValue ? Profiles.FirstOrDefault(p => p.Id == CurrentProfileId.Value) : null;

        public static TodoList? CurrentTodos =>
            CurrentProfileId.HasValue && UserTodos.ContainsKey(CurrentProfileId.Value)
                ? UserTodos[CurrentProfileId.Value]
                : null;

        public static Stack<ICommand> UndoStack { get; } = new();
        public static Stack<ICommand> RedoStack { get; } = new();

        public static string DataDirPath { get; set; } = "";
        public static string ProfileCsvPath { get; set; } = "";

        public static string GetUserTodoPath(Guid id) =>
            Path.Combine(DataDirPath, $"todos_{id}.csv");

        public static void SwitchProfile(Guid? newProfileId)
        {
            CurrentProfileId = newProfileId;
            UndoStack.Clear();
            RedoStack.Clear();

            if (newProfileId.HasValue && !UserTodos.ContainsKey(newProfileId.Value))
            {
                string path = GetUserTodoPath(newProfileId.Value);
                UserTodos[newProfileId.Value] = FileManager.LoadTodos(path);
            }
        }
    }
}
