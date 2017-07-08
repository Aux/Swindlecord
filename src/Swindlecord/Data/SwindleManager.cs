using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swindlecord
{
    public class SwindleManager
    {
        private readonly SwindleDatabase _db;

        public SwindleManager(SwindleDatabase db)
        {
            _db = db;
        }

        public Task<Swindle> GetSwindleAsync(long statusId)
            => _db.Swindles.FirstOrDefaultAsync(x => x.Id == statusId);
        
        public async Task LogAsync(long statusId, SocketUserMessage msg, IEnumerable<SocketUserMessage> msgs)
        {
            await CreateSwindleAsync(statusId, msg);
            await AddStatsAsync(statusId, msgs);
        }

        public async Task CreateSwindleAsync(long statusId, SocketUserMessage msg)
        {
            await CreateAllAsync(msg);

            var swindle = new Swindle(statusId, msg);

            await _db.Swindles.AddAsync(swindle);
            await _db.SaveChangesAsync();
        }

        public async Task AddStatsAsync(long statusId, IEnumerable<SocketUserMessage> msgs)
        {
            var authors = msgs.GroupBy(x => x.Author.Id);
            foreach (var author in authors)
            {
                var stat = new SwindleStat(statusId, author.Key, author.Count());
                await _db.Stats.AddAsync(stat);
            }
            await _db.SaveChangesAsync();
        }

        #region Entity junk

        public async Task CreateGuildAsync(IGuild guild)
        {
            var newGuild = new Guild(guild.Id, guild.ToString());

            await _db.Guilds.AddAsync(newGuild);
            await _db.SaveChangesAsync();
        }

        public async Task CreateChannelAsync(IChannel channel)
        {
            var newChannel = new Channel(channel.Id, channel.ToString());

            await _db.Channels.AddAsync(newChannel);
            await _db.SaveChangesAsync();
        }

        public async Task CreateUserAsync(IUser user)
        {
            var newUser = new User(user.Id, user.ToString());

            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAllAsync(SocketUserMessage msg)
        {
            await TryCreateUserAsync(msg.Author);
            var channel = msg.Channel as ITextChannel;
            await TryCreateChannelAsync(channel);
            await TryCreateGuildAsync(channel.Guild);
        }

        public async Task<bool> TryCreateGuildAsync(IGuild guild)
        {
            var exists = await _db.Guilds.AnyAsync(x => x.Id == guild.Id);
            if (exists)
                return false;
            else
                await CreateGuildAsync(guild);
            return true;
        }

        public async Task<bool> TryCreateChannelAsync(IChannel channel)
        {
            var exists = await _db.Channels.AnyAsync(x => x.Id == channel.Id);
            if (exists)
                return false;
            else
                await CreateChannelAsync(channel);
            return true;
        }

        public async Task<bool> TryCreateUserAsync(IUser user)
        {
            var exists = await _db.Users.AnyAsync(x => x.Id == user.Id);
            if (exists)
                return false;
            else
                await CreateUserAsync(user);
            return true;
        }

        #endregion
    }
}
