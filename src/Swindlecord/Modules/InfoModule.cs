using Discord.Commands;
using System.Threading.Tasks;

namespace Swindlecord.Modules
{
    [Group("info")]
    public class InfoModule : ModuleBase
    {
        [Command]
        public async Task InfoAsync()
        {
            await Task.Delay(0);
        }
    }
}
