using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Swindlecord.Utility;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Swindlecord.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandlingService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            _discord.MessageReceived += OnMessageReceivedAsync;

            await PrettyConsole.LogAsync(LogSeverity.Info, "Services", $"Enabled CommandHandlingService with {_commands.Modules.Count()} modules and {_commands.Commands.Count()} commands");
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot)
                return;

            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;
            if (msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                if (result.IsSuccess)
                    return;

                if (result.Error == CommandError.UnknownCommand)
                    return;

                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
