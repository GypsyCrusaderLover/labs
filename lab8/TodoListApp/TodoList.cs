using System;
using System.Text;

namespace TodoListApp
{
    public class TodoList
    {
        private TodoItem[] _items;
        private int _count;

        public int Count => _count;

        public TodoList(int initialCapacity = 2)
        {
            _items = new TodoItem[initialCapacity];
            _count = 0;
        }

        public void Add(TodoItem item)
        {
            if (_count >= _items.Length)
            {
                IncreaseArray();
            }
            _items[_count] = item;
            _count++;
        }

        public bool Delete(int index)
        {
            if (index < 0 || index >= _count)
            {
                return false;
            }

            for (int i = index; i < _count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }

            _count--;
            _items[_count] = null!;
            return true;
        }

        public TodoItem? GetItem(int index)
        {
            if (index < 0 || index >= _count)
            {
                return null;
            }
            return _items[index];
        }

        public void View(bool showIndex = true, bool showDone = true, bool showDate = true, bool? doneFilter = null)
        {
            if (_count == 0)
            {
                Console.WriteLine("Список задач пуст.");
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                var item = _items[i];
                if (doneFilter.HasValue && item.IsDone != doneFilter.Value)
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

                if (showDone)
                {
                    sb.Append($"[{(item.IsDone ? "сделано" : "не сделано")}] ");
                }

                if (showDate)
                {
                    sb.Append(item.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                Console.WriteLine(sb.ToString().TrimEnd());
            }
        }

        private void IncreaseArray()
        {
            int newLength = _items.Length * 2;
            TodoItem[] newArray = new TodoItem[newLength];
            for (int i = 0; i < _items.Length; i++)
            {
                newArray[i] = _items[i];
            }
            _items = newArray;
        }
    }
}
