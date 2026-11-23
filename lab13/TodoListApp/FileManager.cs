using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TodoListApp
{
    public static class FileManager
    {
        public static void EnsureDataDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public static void SaveProfiles(List<Profile> profiles, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id;Login;Password;FirstName;LastName;BirthYear");
            foreach (var p in profiles)
            {
                sb.AppendLine($"{p.Id};{p.Login};{p.Password};{p.FirstName};{p.LastName};{p.BirthYear}");
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static List<Profile> LoadProfiles(string filePath)
        {
            var list = new List<Profile>();
            if (!File.Exists(filePath))
            {
                return list;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(';');
                if (parts.Length >= 6 && Guid.TryParse(parts[0], out Guid id))
                {
                    string login = parts[1];
                    string password = parts[2];
                    string firstName = parts[3];
                    string lastName = parts[4];
                    int.TryParse(parts[5], out int birthYear);

                    list.Add(new Profile(id, login, password, firstName, lastName, birthYear));
                }
            }
            return list;
        }

        public static void SaveTodos(TodoList todos, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Index;Text;Status;LastUpdate");
            int idx = 0;
            foreach (var item in todos)
            {
                string escapedText = EscapeCsv(item.Text);
                string dateStr = item.LastUpdate.ToString("s");
                sb.AppendLine($"{idx};{escapedText};{item.Status};{dateStr}");
                idx++;
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static void SaveTodoList(TodoItem item)
        {
            if (AppInfo.CurrentProfileId.HasValue && AppInfo.CurrentTodos != null)
            {
                string path = AppInfo.GetUserTodoPath(AppInfo.CurrentProfileId.Value);
                SaveTodos(AppInfo.CurrentTodos, path);
            }
        }

        public static TodoList LoadTodos(string filePath)
        {
            var todoList = new TodoList();
            if (!File.Exists(filePath))
            {
                return todoList;
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                int firstSemi = line.IndexOf(';');
                int lastSemi = line.LastIndexOf(';');
                if (firstSemi < 0 || lastSemi <= firstSemi) continue;

                int secondSemi = line.LastIndexOf(';', lastSemi - 1);
                if (secondSemi <= firstSemi) continue;

                string textPart = line.Substring(firstSemi + 1, secondSemi - firstSemi - 1);
                string statusPart = line.Substring(secondSemi + 1, lastSemi - secondSemi - 1);
                string datePart = line.Substring(lastSemi + 1);

                string text = UnescapeCsv(textPart);
                TodoStatus status = Enum.TryParse<TodoStatus>(statusPart, true, out var parsedStatus)
                    ? parsedStatus
                    : TodoStatus.NotStarted;
                DateTime lastUpdate = DateTime.TryParse(datePart, out DateTime dt) ? dt : DateTime.Now;

                var item = new TodoItem(text, status, lastUpdate);
                todoList.Add(item);
            }

            return todoList;
        }

        public static string EscapeCsv(string text) =>
            "\"" + text.Replace("\"", "\"\"").Replace("\n", "\\n") + "\"";

        public static string UnescapeCsv(string text) =>
            text.Trim('"').Replace("\\n", "\n").Replace("\"\"", "\"");
    }
}
