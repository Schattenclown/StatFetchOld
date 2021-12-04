using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The fourtytwo type choice provider.
    /// </summary>
    public class FourtytwoTypeChoiceProvider : IChoiceProvider
    {
        /// <summary>
        /// Providers the choices.
        /// </summary>
        /// <returns>choices</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            DiscordApplicationCommandOptionChoice[] discordApplicationCommmandOptionChoices = new DiscordApplicationCommandOptionChoice[3];

            discordApplicationCommmandOptionChoices[0] = new DiscordApplicationCommandOptionChoice("MINIMAL", "0");
            discordApplicationCommmandOptionChoices[1] = new DiscordApplicationCommandOptionChoice("FULL", "1");
            discordApplicationCommmandOptionChoices[2] = new DiscordApplicationCommandOptionChoice("STATISTICS", "2");

            return discordApplicationCommmandOptionChoices;
        }
    }
}
