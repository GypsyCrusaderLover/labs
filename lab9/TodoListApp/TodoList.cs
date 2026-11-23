using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TodoListApp
{
    public class TodoList : IEnumerable<TodoItem>
    {
        private List<TodoItem> _items = new();

        public int Count => _items.Count;

        public TodoItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public void Add(TodoItem item)
        {
            _items.Add(item);
        }

        public bool Delete(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return false;
            }
            _items.RemoveAt(index);
            return true;
        }

        public TodoItem? GetItem(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            return _items[index];
        }

        public bool SetStatus(int index, TodoStatus status)
        {
            if (index < 0 || index >= _items.Count)
            {
                return false;
            }
            _items[index].SetStatus(status);
            return true;
        }

        public void View(bool showIndex = true, bool showStatus = true, bool showDate = true, TodoStatus? statusFilter = null)
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (statusFilter.HasValue && item.Status != statusFilter.Value)
                {
                    continue;
                }

                var sb = new StringBuilder();
                if (showIndex)
                {
                    sb.Append($"[{i}] ");
                }

                string preview = item.Text.Replace("\n", " ");
                if (preview.Length > 30)
                {
                    preview = preview.Substring(0, 27) + "...";
                }
                sb.Append($"\"{preview}\" ");

                if (showStatus)
                {
                    sb.Append($"[{item.Status}] ");
                }

                if (showDate)
                {
                    sb.Append(item.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                Console.WriteLine(sb.ToString().TrimEnd());
            }
        }

        public IEnumerator<TodoItem> GetEnumerator()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
