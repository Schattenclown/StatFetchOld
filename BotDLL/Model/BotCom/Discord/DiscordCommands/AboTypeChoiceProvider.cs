using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The Abotype choice provider.
    /// </summary>
    public class AboTypeChoiceProvider : IChoiceProvider
    {
        /// <summary>
        /// Providers the choices.
        /// </summary>
        /// <returns>choices</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            DiscordApplicationCommandOptionChoice[] choices = new DiscordApplicationCommandOptionChoice[2];

            choices[0] = new DiscordApplicationCommandOptionChoice("FULL", "0");
            choices[1] = new DiscordApplicationCommandOptionChoice("MINIMAL", "1");

            return choices;
        }
    }
}
