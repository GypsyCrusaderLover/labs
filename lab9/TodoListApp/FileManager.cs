using System;
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

        public static void SaveProfile(Profile profile, string filePath)
        {
            string content = $"{profile.FirstName};{profile.LastName};{profile.BirthYear}";
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        public static Profile LoadProfile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Profile("", "", 0);
            }

            string content = File.ReadAllText(filePath, Encoding.UTF8).Trim();
            if (string.IsNullOrEmpty(content))
            {
                return new Profile("", "", 0);
            }

            string[] parts = content.Split(';');
            string firstName = parts.Length > 0 ? parts[0] : "";
            string lastName = parts.Length > 1 ? parts[1] : "";
            int birthYear = parts.Length > 2 && int.TryParse(parts[2], out int by) ? by : 0;

            return new Profile(firstName, lastName, birthYear);
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
