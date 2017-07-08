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
        private readonly SwindleManager _manager;
        private readonly Tokens _twitter;
        private readonly Random _random;

        private TimeSpan _postEvery;
        private List<SocketUserMessage> _messages;
        private SocketTextChannel _log;
        private Task _task;
        
        public SwindleService(
            DiscordSocketClient discord,
            IConfigurationRoot config,
            SwindleManager manager,
            Tokens twitter,
            Random random)
        {
            _discord = discord;
            _config = config;
            _manager = manager;
            _twitter = twitter;
            _random = random;
            
            _postEvery = TimeSpan.FromMinutes(int.Parse(_config["post_every"]));
            _messages = new List<SocketUserMessage>();

            _discord.Ready += OnReadyAsync;
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageDeleted += OnMessageDeletedAsync;
            PrettyConsole.Log(LogSeverity.Info, "Services", "Enabled SwindleService");
        }

        private Task OnReadyAsync()
        {
            var logGuild = _discord.GetGuild(ulong.Parse(_config["log_guild_id"]));
            _log = logGuild.GetTextChannel(ulong.Parse(_config["log_channel_id"]));
            _task = RunAsync();
            return Task.CompletedTask;
        }

        private Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot)
                return Task.CompletedTask;

            if (msg.Content.Length >= 140) // Exclude messages too large for a single tweet
                return Task.CompletedTask;

            if (!msg.Attachments.Any() && msg.Content.Length == 0) // Exclude empty messages that have no attachments
                return Task.CompletedTask;

            if (ContainsBlacklistedWord(msg.Content))  // Exclude messages that contain naughty words
                return Task.CompletedTask;

            _messages.Add(msg);
            return Task.CompletedTask;
        }

        private Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            var deleted = _messages.FirstOrDefault(x => x.Id == msg.Id);
            if (deleted != null)
                _messages.Remove(deleted);
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

                    var mediaIds = await GetMediaIdsAsync(selected);
                    var status = await _twitter.Statuses.UpdateAsync(selected.Resolve(), possibly_sensitive: true, media_ids: mediaIds);
                    await _log.SendMessageAsync("", embed: TwitterHelper.GetPostedEmbed(selected, status.Id));

                    _ = _manager.LogAsync(status.Id, selected, _messages);
                    _messages.Clear();
                }
            }
            catch (Exception ex)
            {
                await PrettyConsole.LogAsync(LogSeverity.Error, "SwindleService", ex.ToString());
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
                {
                    PrettyConsole.Log(LogSeverity.Info, "SwindleService", $"Skipped `{content}` for containing a blacklisted word.");
                    return true;
                }
            }

            return false;
        }

        private async Task<IEnumerable<long>> GetMediaIdsAsync(SocketUserMessage msg)
        {
            var mediaIds = new List<long>();
            var attachment = msg.Attachments.FirstOrDefault();
            if (attachment != null && IsImageFile(attachment.Filename))
            {
                using (var http = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, attachment.Url))
                {
                    var response = await request.Content.ReadAsStreamAsync();
                    var media = await _twitter.Media.UploadAsync(response);
                    mediaIds.Add(media.MediaId);
                }
            }
            return mediaIds;
        }
        
        private bool IsImageFile(string fileName)
        {
            if (fileName.EndsWith("png")) return true;
            if (fileName.EndsWith("jpg")) return true;
            if (fileName.EndsWith("jpeg")) return true;
            if (fileName.EndsWith("gif")) return true;
            if (fileName.EndsWith("bmp")) return true;
            return false;
        }
    }
}
