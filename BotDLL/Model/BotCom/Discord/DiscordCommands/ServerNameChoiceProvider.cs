using BotDLL.Model.Objects;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.Entities;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands;

/// <summary>
///    The Servername choice provider.
/// </summary>
public class ServerNameChoiceProvider : IChoiceProvider
{
   /// <summary>
   ///    Providers the choices.
   /// </summary>
   /// <returns>choices</returns>
#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
   public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
   {
      var i = 0;
      string[] serverInfoNameArray;

      var serverInfoList = ServerInfo.ReadAll();
      /*List<ServerStat> serverStatListLive = new();
      foreach (var serverInfoObj in serverInfoList)
      {
         var serverStatObj = ServerStat.CreateObj(serverInfoObj);
         serverStatListLive.Add(serverStatObj);
      }*/

      serverInfoNameArray = new string[serverInfoList.Count];
      foreach (var serverInfoObj in serverInfoList)
      {
         serverInfoNameArray[i] = serverInfoObj.Name.ToLower();
         i++;
      }

      var discordApplicationCommandOptionChoice = new DiscordApplicationCommandOptionChoice[serverInfoNameArray.Length];

      i = 0;
      foreach (var serverInfoNameString in serverInfoNameArray)
      {
         discordApplicationCommandOptionChoice[i] = new DiscordApplicationCommandOptionChoice(serverInfoNameString.ToUpper(), i.ToString());
         i++;
      }

      return discordApplicationCommandOptionChoice;
   }
}