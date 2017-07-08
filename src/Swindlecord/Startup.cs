using CoreTweet;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swindlecord.Databases;
using Swindlecord.Services;
using Swindlecord.Utility;
using System;
using System.Threading.Tasks;

namespace Swindlecord
{
    public class Startup
    {
        public static Startup Instance = new Startup();

        public IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("configuration.json", false);
            Configuration = builder.Build();
        }

        public async Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.AddDbContext<TweetDatabase>(ServiceLifetime.Transient);

            services.AddSingleton<CommandHandlingService>()
                .AddSingleton<TweetService>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration);

            await ConfigureAsync(services);
        }

        private async Task ConfigureAsync(IServiceCollection services)
        {
           // Twitter
           var twitter = Tokens.Create(Configuration["twitter:consumer_key"], Configuration["twitter:consumer_secret"], Configuration["twitter:token"], Configuration["twitter:secret"]);

            // Discord
            var discord = new DiscordShardedClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                ThrowOnError = false
            });

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;

            await discord.LoginAsync(TokenType.Bot, Configuration["discord:token"]);
            await discord.StartAsync();
            
            services.AddSingleton(twitter)
                .AddSingleton(discord)
                .AddSingleton(commands);
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
