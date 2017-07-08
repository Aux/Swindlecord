using CoreTweet;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Swindlecord.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swindlecord.Services
{
    public class SwindleService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IConfigurationRoot _config;
        private readonly Tokens _twitter;
        private readonly Random _random;

        private TimeSpan _postEvery;
        private List<SocketUserMessage> _messages;
        private SocketTextChannel _log;
        private HttpClient _http;
        private Task _task;
        
        public SwindleService(
            DiscordSocketClient discord,
            IConfigurationRoot config,
            Tokens twitter,
            Random random)
        {
            _discord = discord;
            _config = config;
            _twitter = twitter;
            _random = random;
            
            _postEvery = TimeSpan.FromMinutes(int.Parse(_config["post_every"]));
            _messages = new List<SocketUserMessage>();
            _http = new HttpClient();
            _discord.Ready += OnReadyAsync;
            _discord.MessageReceived += OnMessageReceived;
            PrettyConsole.Log(LogSeverity.Info, "Services", "Enabled SwindleService");
        }

        private Task OnReadyAsync()
        {
            var logGuild = _discord.GetGuild(ulong.Parse(_config["log_guild_id"]));
            _log = logGuild.GetTextChannel(ulong.Parse(_config["log_channel_id"]));
            _task = RunAsync();
            return Task.CompletedTask;
        }

        private Task OnMessageReceived(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot)
                return Task.CompletedTask;

            if (msg.Content.Length >= 140 || msg.Content.Length == 0)
                return Task.CompletedTask;

            if (msg.Attachments.Any())
                return Task.CompletedTask;

            if (ContainsBlacklistedWord(msg.Content))
                return Task.CompletedTask;

            _messages.Add(msg);
            return Task.CompletedTask;
        }

        private async Task RunAsync()
        {
            try
            {
                while (true)
                {
                    await Task.Delay(_postEvery);

                    int index = _random.Next(0, _messages.Count());
                    var selected = _messages.ElementAtOrDefault(index);
                    
                    if (selected == null)
                    {
                        await _log.SendMessageAsync("I couldn't find any messages to swindle <:ShibeSad:231546068960018433>");
                        return;
                    }

                    var response = await _twitter.Statuses.UpdateAsync(selected.Resolve(), possibly_sensitive: true);
                    await _log.SendMessageAsync("", embed: TwitterHelper.GetPostedEmbed(selected, response.Id));

                    await PrettyConsole.LogAsync(LogSeverity.Info, "SwindleService", $"Cleared {_messages.Count} from cache");
                    _messages.Clear();
                }
            }
            catch (Exception ex)
            {
                await PrettyConsole.LogAsync(LogSeverity.Error, "TweetService", ex.ToString());
            }
        }

        private bool ContainsBlacklistedWord(string content)
        {
            var template = _config["regex"];
            var words = _config.GetSection("blacklist").GetChildren().Select(x => x.Value);

            foreach (var word in words)
            {
                string pattern = null;
                for (int i = 0; i < word.Length - 1; i++)
                    pattern += word[i] + template;

                var match = new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(content);

                if (match)
                    return true;
            }

            return false;
        }
    }
}
