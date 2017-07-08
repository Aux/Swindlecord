using Discord;

namespace Swindlecord
{
    public static class TwitterHelper
    {
        public const string StatusUrl = "https://twitter.com/swindlecord/status/";

        public static string GetStatusUrl(long statusId)
            => StatusUrl + statusId;

        public static Embed GetPostedEmbed(IUserMessage msg, long statusId)
            => GetPostedEmbed((msg.Channel as IGuildChannel).Guild, msg, statusId);
        public static Embed GetPostedEmbed(IGuild guild, IUserMessage msg, long statusId)
        {
            var channel = msg.Channel;
            string statusUrl = GetStatusUrl(statusId);

            var builder = new EmbedBuilder()
                .WithDescription($"I swindled a message in {channel}. [Click here to view!]({statusUrl})")
                .WithCurrentTimestamp();

            builder.WithAuthor(x =>
            {
                x.Name = msg.Author.ToString();
                x.IconUrl = msg.Author.GetAvatarUrl();
                x.Url = statusUrl;
            });

            builder.WithFooter(x =>
            {
                x.IconUrl = guild.IconUrl;
                x.Text = $"{guild} ({guild.Id})";
            });

            return builder;
        }
    }
}
