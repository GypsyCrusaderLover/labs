using System;

namespace TodoListApp
{
    public static class CommandParser
    {
        public static ICommand Parse(string inputString, TodoList todoList, Profile profile, string todoFilePath, string profileFilePath)
        {
            string trimmed = inputString.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return new UnknownCommand();
            }

            string[] parts = trimmed.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string commandName = parts[0].ToLower();
            string args = parts.Length > 1 ? parts[1] : "";

            switch (commandName)
            {
                case "help":
                    return new HelpCommand();

                case "exit":
                    return new ExitCommand();

                case "profile":
                    return new ProfileCommand { Profile = profile, ProfileFilePath = profileFilePath };

                case "add":
                    bool isMultiline = args.Contains("--multiline") || args.Contains("-m");
                    string text = "";
                    if (!isMultiline)
                    {
                        string[] quoteParts = args.Split(new[] { '"' }, StringSplitOptions.None);
                        if (quoteParts.Length >= 2)
                        {
                            text = quoteParts[1];
                        }
                        else
                        {
                            text = args.Trim('"');
                        }
                    }
                    return new AddCommand
                    {
                        TodoList = todoList,
                        IsMultiline = isMultiline,
                        Text = text,
                        TodoFilePath = todoFilePath
                    };

                case "view":
                    ParseViewFlags(args, out bool showIndex, out bool showDone, out bool showDate, out bool? doneFilter);
                    return new ViewCommand
                    {
                        TodoList = todoList,
                        ShowIndex = showIndex,
                        ShowDone = showDone,
                        ShowDate = showDate,
                        DoneFilter = doneFilter
                    };

                case "read":
                    if (int.TryParse(args.Trim(), out int readIdx))
                    {
                        return new ReadCommand { TodoList = todoList, Index = readIdx };
                    }
                    return new UnknownCommand();

                case "done":
                    if (int.TryParse(args.Trim(), out int doneIdx))
                    {
                        return new DoneCommand { TodoList = todoList, Index = doneIdx, TodoFilePath = todoFilePath };
                    }
                    return new UnknownCommand();

                case "delete":
                    if (int.TryParse(args.Trim(), out int delIdx))
                    {
                        return new DeleteCommand { TodoList = todoList, Index = delIdx, TodoFilePath = todoFilePath };
                    }
                    return new UnknownCommand();

                case "update":
                    string[] updateParts = args.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (updateParts.Length >= 2 && int.TryParse(updateParts[0], out int upIdx))
                    {
                        string newText = updateParts[1].Trim('"', ' ');
                        return new UpdateCommand { TodoList = todoList, Index = upIdx, NewText = newText, TodoFilePath = todoFilePath };
                    }
                    return new UnknownCommand();

                default:
                    return new UnknownCommand();
            }
        }

        private static void ParseViewFlags(string args, out bool showIndex, out bool showDone, out bool showDate, out bool? doneFilter)
        {
            string[] tokens = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            doneFilter = null;
            bool explicitIndex = false;
            bool explicitStatus = false;
            bool explicitDate = false;
            bool hasExplicitDisplayFlags = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i].ToLower();
                if (token == "--all" || token == "-a") doneFilter = null;
                else if (token == "--done" || token == "-d") doneFilter = true;
                else if (token == "--undone" || token == "-u") doneFilter = false;
                else if (token == "--index") { explicitIndex = true; hasExplicitDisplayFlags = true; }
                else if (token == "--status") { explicitStatus = true; hasExplicitDisplayFlags = true; }
                else if (token == "--date") { explicitDate = true; hasExplicitDisplayFlags = true; }
                else if (token.StartsWith("-") && !token.StartsWith("--"))
                {
                    for (int c = 1; c < token.Length; c++)
                    {
                        char ch = token[c];
                        if (ch == 'a') doneFilter = null;
                        else if (ch == 'd') doneFilter = true;
                        else if (ch == 'u') doneFilter = false;
                        else if (ch == 'i') { explicitIndex = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 's') { explicitStatus = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 't') { explicitDate = true; hasExplicitDisplayFlags = true; }
                    }
                }
            }

            showIndex = !hasExplicitDisplayFlags || explicitIndex;
            showDone = !hasExplicitDisplayFlags || explicitStatus;
            showDate = !hasExplicitDisplayFlags || explicitDate;
        }
    }
}
