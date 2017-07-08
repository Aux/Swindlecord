using System;
using System.Threading.Tasks;

namespace Swindlecord.Utility
{
    public static class PrettyConsole
    {
        public static void Append(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
        }

        public static Task AppendAsync(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            return Console.Out.WriteAsync(text);
        }

        public static void NewLine(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(Environment.NewLine + text);
        }

        public static Task NewLineAsync(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            return Console.Out.WriteAsync(Environment.NewLine + text);
        }

        public static void Log(object severity, string source, string message)
        {
            NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            Append($"[{severity}] ", ConsoleColor.Red);
            Append($"{source}: ", ConsoleColor.DarkGreen);
            Append(message, ConsoleColor.White);
        }

        public static Task LogAsync(object severity, string source, string message)
        {
            NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            Append($"[{severity}] ", ConsoleColor.Red);
            Append($"{source}: ", ConsoleColor.DarkGreen);
            Append(message, ConsoleColor.White);
            return Task.CompletedTask;
        }

        public static async Task LogCommandAsync(string userName, string message)
        {
            await NewLineAsync($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);
            await AppendAsync($"[PM] ", ConsoleColor.Magenta);
            await AppendAsync($"{userName}: ", ConsoleColor.Green);
            await AppendAsync(message, ConsoleColor.White);
        }

        public static async Task LogCommandAsync(string guildName, string channelName, string userName, string message)
        {
            await NewLineAsync($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);
            await AppendAsync($"[{guildName} #{channelName}] ", ConsoleColor.DarkGreen);
            await AppendAsync($"{userName}: ", ConsoleColor.Green);
            await AppendAsync(message, ConsoleColor.White);
        }
    }
}
