using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;

namespace BotDLL.Model.BotCom.Discord.ChoiceProvider
{
   /// <summary>
   ///    The fourtytwo type choice provider.
   /// </summary>
   public class FourtytwoTypeChoiceProvider : IChoiceProvider
   {
      /// <summary>
      ///    Providers the choices.
      /// </summary>
      /// <returns>choices</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
      public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
      {
         DiscordApplicationCommandOptionChoice[] discordApplicationCommandOptionChoices = new DiscordApplicationCommandOptionChoice[3];

         discordApplicationCommandOptionChoices[0] = new DiscordApplicationCommandOptionChoice("MINIMAL", "0");
         discordApplicationCommandOptionChoices[1] = new DiscordApplicationCommandOptionChoice("FULL", "1");
         discordApplicationCommandOptionChoices[2] = new DiscordApplicationCommandOptionChoice("STATISTICS", "2");

         return discordApplicationCommandOptionChoices;
      }
   }
}