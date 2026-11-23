using System;

namespace TodoListApp
{
    public static class CommandParser
    {
        public static ICommand Parse(string inputString)
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
                    bool isOut = args.Contains("--out") || args.Contains("-o");
                    return new ProfileCommand { IsLogout = isOut };

                case "undo":
                    return new UndoCommand();

                case "redo":
                    return new RedoCommand();

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
                        IsMultiline = isMultiline,
                        Text = text
                    };

                case "view":
                    ParseViewFlags(args, out bool showIndex, out bool showStatus, out bool showDate, out TodoStatus? statusFilter);
                    return new ViewCommand
                    {
                        ShowIndex = showIndex,
                        ShowStatus = showStatus,
                        ShowDate = showDate,
                        StatusFilter = statusFilter
                    };

                case "read":
                    if (int.TryParse(args.Trim(), out int readIdx))
                    {
                        return new ReadCommand { Index = readIdx };
                    }
                    return new UnknownCommand();

                case "status":
                    string[] statusParts = args.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (statusParts.Length >= 2 && int.TryParse(statusParts[0], out int sIdx))
                    {
                        if (Enum.TryParse<TodoStatus>(statusParts[1], true, out var parsedStatus))
                        {
                            return new StatusCommand
                            {
                                Index = sIdx,
                                NewStatus = parsedStatus
                            };
                        }
                    }
                    return new UnknownCommand();

                case "delete":
                    if (int.TryParse(args.Trim(), out int delIdx))
                    {
                        return new DeleteCommand { Index = delIdx };
                    }
                    return new UnknownCommand();

                case "update":
                    string[] updateParts = args.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (updateParts.Length >= 2 && int.TryParse(updateParts[0], out int upIdx))
                    {
                        string newText = updateParts[1].Trim('"', ' ');
                        return new UpdateCommand { Index = upIdx, NewText = newText };
                    }
                    return new UnknownCommand();

                default:
                    return new UnknownCommand();
            }
        }

        private static void ParseViewFlags(string args, out bool showIndex, out bool showStatus, out bool showDate, out TodoStatus? statusFilter)
        {
            string[] tokens = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            statusFilter = null;
            bool explicitIndex = false;
            bool explicitStatus = false;
            bool explicitDate = false;
            bool hasExplicitDisplayFlags = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i].ToLower();
                if (token == "--all" || token == "-a") statusFilter = null;
                else if (token == "--done" || token == "-d") statusFilter = TodoStatus.Completed;
                else if (token == "--undone" || token == "-u") statusFilter = TodoStatus.NotStarted;
                else if (token == "--index") { explicitIndex = true; hasExplicitDisplayFlags = true; }
                else if (token == "--status") { explicitStatus = true; hasExplicitDisplayFlags = true; }
                else if (token == "--date") { explicitDate = true; hasExplicitDisplayFlags = true; }
                else if (token.StartsWith("-") && !token.StartsWith("--"))
                {
                    for (int c = 1; c < token.Length; c++)
                    {
                        char ch = token[c];
                        if (ch == 'a') statusFilter = null;
                        else if (ch == 'd') statusFilter = TodoStatus.Completed;
                        else if (ch == 'u') statusFilter = TodoStatus.NotStarted;
                        else if (ch == 'i') { explicitIndex = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 's') { explicitStatus = true; hasExplicitDisplayFlags = true; }
                        else if (ch == 't') { explicitDate = true; hasExplicitDisplayFlags = true; }
                    }
                }
            }

            showIndex = !hasExplicitDisplayFlags || explicitIndex;
            showStatus = !hasExplicitDisplayFlags || explicitStatus;
            showDate = !hasExplicitDisplayFlags || explicitDate;
        }
    }
}
