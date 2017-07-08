using CoreTweet;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Swindlecord.Modules
{
    [RequireOwner]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private readonly Tokens _twitter;

        public AdminModule(IConfigurationRoot config, Tokens twitter)
        {
            _config = config;
            _twitter = twitter;
        }

        [Command("force")]
        public async Task ForceAsync(ulong messageId)
        {
            var logGuild = Context.Client.GetGuild(ulong.Parse(_config["log_guild_id"]));
            var log = logGuild.GetTextChannel(ulong.Parse(_config["log_channel_id"]));
            var msg = (await Context.Channel.GetMessageAsync(messageId)) as IUserMessage;
            
            var response = await _twitter.Statuses.UpdateAsync(msg.Resolve(), possibly_sensitive: true);
            await log.SendMessageAsync("", embed: TwitterHelper.GetPostedEmbed(msg, response.Id));
        }
    }
}
