using System;

namespace TodoListApp
{
    public class TodoItem
    {
        public string Text { get; private set; }
        public TodoStatus Status { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TodoItem(string text)
        {
            Text = text;
            Status = TodoStatus.NotStarted;
            LastUpdate = DateTime.Now;
        }

        public TodoItem(string text, TodoStatus status, DateTime lastUpdate)
        {
            Text = text;
            Status = status;
            LastUpdate = lastUpdate;
        }

        public void SetStatus(TodoStatus newStatus)
        {
            Status = newStatus;
            LastUpdate = DateTime.Now;
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            LastUpdate = DateTime.Now;
        }

        public string GetShortInfo()
        {
            string preview = Text.Replace("\n", " ");
            if (preview.Length > 30)
            {
                preview = preview.Substring(0, 27) + "...";
            }
            return $"\"{preview}\" [{Status}] {LastUpdate:yyyy-MM-dd HH:mm:ss}";
        }

        public string GetFullInfo()
        {
            return $"Статус: {Status}\nДата последнего изменения: {LastUpdate:yyyy-MM-dd HH:mm:ss}\nТекст задачи:\n{Text}";
        }
    }
}
