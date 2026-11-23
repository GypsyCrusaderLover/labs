using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TodoListApp
{
    public class TodoList : IEnumerable<TodoItem>
    {
        private List<TodoItem> _items = new();

        public event Action<TodoItem>? OnTodoAdded;
        public event Action<TodoItem>? OnTodoDeleted;
        public event Action<TodoItem>? OnTodoUpdated;
        public event Action<TodoItem>? OnStatusChanged;

        public int Count => _items.Count;

        public TodoItem this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public void Add(TodoItem item)
        {
            _items.Add(item);
            OnTodoAdded?.Invoke(item);
        }

        public void Insert(int index, TodoItem item)
        {
            if (index < 0) index = 0;
            if (index > _items.Count) index = _items.Count;
            _items.Insert(index, item);
            OnTodoAdded?.Invoke(item);
        }

        public bool Delete(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return false;
            }
            var deletedItem = _items[index];
            _items.RemoveAt(index);
            OnTodoDeleted?.Invoke(deletedItem);
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

        public bool Update(int index, string newText)
        {
            if (index < 0 || index >= _items.Count)
            {
                return false;
            }
            _items[index].UpdateText(newText);
            OnTodoUpdated?.Invoke(_items[index]);
            return true;
        }

        public bool SetStatus(int index, TodoStatus status)
        {
            if (index < 0 || index >= _items.Count)
            {
                return false;
            }
            _items[index].SetStatus(status);
            OnStatusChanged?.Invoke(_items[index]);
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
