using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Swindlecord
{
    public class SwindleDatabase : DbContext
    {
        public DbSet<Swindle> Swindles { get; private set; }
        public DbSet<SwindleStat> Stats { get; private set; }
        public DbSet<Guild> Guilds { get; private set; }
        public DbSet<Channel> Channels { get; private set; }
        public DbSet<User> Users { get; private set; }
        
        public SwindleDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlite($"Filename={Path.Combine(AppContext.BaseDirectory, "swindle.sqlite.db")}");
        }
    }
}
