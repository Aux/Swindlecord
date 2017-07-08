using CoreTweet;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Swindlecord.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Swindlecord.Services
{
    public class TweetService
    {
        private readonly DiscordShardedClient _discord;
        private readonly IConfigurationRoot _config;
        private readonly Tokens _twitter;
        private readonly Random _random;

        private TimeSpan _postEvery = TimeSpan.FromHours(1);
        private List<SocketUserMessage> _messages;
        private SocketTextChannel _log;
        private HttpClient _http;
        
        public TweetService(
            DiscordShardedClient discord,
            IConfigurationRoot config,
            Tokens twitter,
            Random random)
        {
            _discord = discord;
            _config = config;
            _twitter = twitter;
            _random = random;
            
            _messages = new List<SocketUserMessage>();
            _http = new HttpClient();
            _discord.MessageReceived += OnMessageReceived;
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

            _messages.Add(msg);
            return Task.CompletedTask;
        }

        private async Task RunAsync()
        {
            _log = _discord.GetChannel(ulong.Parse(_config["log_channel_id"])) as SocketTextChannel;

            try
            {
                while (true)
                {
                    await Task.Delay(_postEvery);

                    int index = _random.Next(0, _messages.Count());
                    var selected = _messages.ElementAt(index);
                    
                    var response = await _twitter.Statuses.UpdateAsync(selected.Content, possibly_sensitive: true);
                    await _log.SendMessageAsync("", embed: TwitterHelper.GetPostedEmbed(selected, response.Id));
                    _messages.Clear();
                }
            }
            catch (Exception ex)
            {
                await PrettyConsole.LogAsync(LogSeverity.Error, "TweetService", ex.ToString());
            }
        }
    }
}
