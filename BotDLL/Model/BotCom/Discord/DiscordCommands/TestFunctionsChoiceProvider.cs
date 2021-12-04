using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The function test choice provider.
    /// </summary>
    public class TestFunctionsChoiceProvider : IChoiceProvider
    {
        /// <summary>
        /// Providers the choices.
        /// </summary>
        /// <returns>choices</returns>
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            DiscordApplicationCommandOptionChoice[] choices = new DiscordApplicationCommandOptionChoice[3];

            choices[0] = new DiscordApplicationCommandOptionChoice("PLAYERCOUNT", "0");
            choices[1] = new DiscordApplicationCommandOptionChoice("ONLINESTATE", "1");
            choices[2] = new DiscordApplicationCommandOptionChoice("VERSIONCHANGE", "2");

            return choices;
        }
    }
}
