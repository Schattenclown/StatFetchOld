using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands;

/// <summary>
///    The Abotype choice provider.
/// </summary>
public class AboTypeChoiceProvider : IChoiceProvider
{
   /// <summary>
   ///    Providers the choices.
   /// </summary>
   /// <returns>choices</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
   public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
   {
      var choices = new DiscordApplicationCommandOptionChoice[2];

      choices[0] = new DiscordApplicationCommandOptionChoice("MINIMAL", "0");
      choices[1] = new DiscordApplicationCommandOptionChoice("FULL", "2");

      return choices;
   }
}