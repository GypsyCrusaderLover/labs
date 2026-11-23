using System;

namespace TodoListApp
{
    public class TodoItem
    {
        public string Text { get; private set; }
        public bool IsDone { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public TodoItem(string text)
        {
            Text = text;
            IsDone = false;
            LastUpdate = DateTime.Now;
        }

        public void MarkDone()
        {
            IsDone = true;
            LastUpdate = DateTime.Now;
        }

        public void UpdateText(string newText)
        {
            Text = newText;
            LastUpdate = DateTime.Now;
        }

        public string GetShortInfo()
        {
            string status = IsDone ? "сделано" : "не сделано";
            string preview = Text.Replace("\n", " ");
            if (preview.Length > 30)
            {
                preview = preview.Substring(0, 27) + "...";
            }
            return $"\"{preview}\" [{status}] {LastUpdate:yyyy-MM-dd HH:mm:ss}";
        }

        public string GetFullInfo()
        {
            string status = IsDone ? "сделано" : "не сделано";
            return $"Статус: {status}\nДата последнего изменения: {LastUpdate:yyyy-MM-dd HH:mm:ss}\nТекст задачи:\n{Text}";
        }
    }
}
