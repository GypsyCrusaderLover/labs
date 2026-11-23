using System;
using System.Collections.Generic;

namespace TodoListApp
{
    public static class CommandParser
    {
        private static Dictionary<string, Func<string, ICommand>> _commandHandlers = new();

        static CommandParser()
        {
            Init();
        }

        public static void Init()
        {
            _commandHandlers["help"] = ParseHelp;
            _commandHandlers["exit"] = ParseExit;
            _commandHandlers["profile"] = ParseProfile;
            _commandHandlers["undo"] = ParseUndo;
            _commandHandlers["redo"] = ParseRedo;
            _commandHandlers["add"] = ParseAdd;
            _commandHandlers["view"] = ParseView;
            _commandHandlers["read"] = ParseRead;
            _commandHandlers["status"] = ParseStatus;
            _commandHandlers["delete"] = ParseDelete;
            _commandHandlers["update"] = ParseUpdate;
        }

        public static ICommand Parse(string input)
        {
            var parts = input.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return new HelpCommand();
            }

            var command = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1] : "";

            if (_commandHandlers.TryGetValue(command, out var handler))
            {
                return handler(args);
            }

            Console.WriteLine("Неизвестная команда.");
            return new HelpCommand();
        }

        private static ICommand ParseHelp(string args) => new HelpCommand();

        private static ICommand ParseExit(string args) => new ExitCommand();

        private static ICommand ParseProfile(string args)
        {
            bool isOut = args.Contains("--out") || args.Contains("-o");
            return new ProfileCommand { IsLogout = isOut };
        }

        private static ICommand ParseUndo(string args) => new UndoCommand();

        private static ICommand ParseRedo(string args) => new RedoCommand();

        private static ICommand ParseAdd(string args)
        {
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
        }

        private static ICommand ParseView(string args)
        {
            ParseViewFlags(args, out bool showIndex, out bool showStatus, out bool showDate, out TodoStatus? statusFilter);
            return new ViewCommand
            {
                ShowIndex = showIndex,
                ShowStatus = showStatus,
                ShowDate = showDate,
                StatusFilter = statusFilter
            };
        }

        private static ICommand ParseRead(string args)
        {
            if (int.TryParse(args.Trim(), out int readIdx))
            {
                return new ReadCommand { Index = readIdx };
            }
            return new HelpCommand();
        }

        private static ICommand ParseStatus(string args)
        {
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
            return new HelpCommand();
        }

        private static ICommand ParseDelete(string args)
        {
            if (int.TryParse(args.Trim(), out int delIdx))
            {
                return new DeleteCommand { Index = delIdx };
            }
            return new HelpCommand();
        }

        private static ICommand ParseUpdate(string args)
        {
            string[] updateParts = args.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (updateParts.Length >= 2 && int.TryParse(updateParts[0], out int upIdx))
            {
                string newText = updateParts[1].Trim('"', ' ');
                return new UpdateCommand { Index = upIdx, NewText = newText };
            }
            return new HelpCommand();
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
