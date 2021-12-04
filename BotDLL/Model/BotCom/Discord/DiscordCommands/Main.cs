using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The MAIN.
    /// </summary>
    internal class Main : BaseCommandModule
    {
        /// <summary>
        /// prob. does nothing
        /// </summary>
        /// <param name="ctx">The command context.</param>
        [Command("ping"), Description("Ping")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.Client.Ping}ms");
        }
    }
}
