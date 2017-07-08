using Microsoft.Extensions.DependencyInjection;
using Swindlecord.Services;
using System.Threading.Tasks;

namespace Swindlecord
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();
        
        public async Task StartAsync()
        {
            var services = new ServiceCollection();
            await Startup.Instance.ConfigureServicesAsync(services);
            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<SwindleService>();

            await provider.GetRequiredService<CommandHandlingService>().StartAsync();
            
            await Task.Delay(-1);
        }
    }
}