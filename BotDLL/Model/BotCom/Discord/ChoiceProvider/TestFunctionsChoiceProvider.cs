using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;

namespace BotDLL.Model.BotCom.Discord.ChoiceProvider
{
   /// <summary>
   ///    The function test choice provider.
   /// </summary>
   public class TestFunctionsChoiceProvider : IChoiceProvider
   {
      /// <summary>
      ///    Providers the choices.
      /// </summary>
      /// <returns>choices</returns>
#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
      public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
      {
         DiscordApplicationCommandOptionChoice[] choices = new DiscordApplicationCommandOptionChoice[3];

         choices[0] = new DiscordApplicationCommandOptionChoice("PLAYERCOUNT", "0");
         choices[1] = new DiscordApplicationCommandOptionChoice("ONLINESTATE", "1");
         choices[2] = new DiscordApplicationCommandOptionChoice("VERSIONCHANGE", "2");

         return choices;
      }
   }
}