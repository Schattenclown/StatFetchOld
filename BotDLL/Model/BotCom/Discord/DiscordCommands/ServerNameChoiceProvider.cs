using BotDLL.Model.Objects;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The Servername choice provider.
    /// </summary>
    public class ServerNameChoiceProvider : IChoiceProvider
    {
        /// <summary>
        /// Providers the choices.
        /// </summary>
        /// <returns>choices</returns>
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            int i = 0;
            string[] serverInfoNameArray;

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoObj in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                serverStatListLive.Add(serverStatObj);
            }

            serverInfoNameArray = new string[serverStatListLive.Count];
            foreach (ServerInfo serverInfoObj in serverInfoList)
            {
                serverInfoNameArray[i] = serverInfoObj.Name.ToLower();
                i++;
            }

            DiscordApplicationCommandOptionChoice[] discordApplicationCommandOptionChoice = new DiscordApplicationCommandOptionChoice[serverInfoNameArray.Length];

            i = 0;
            foreach (string serverInfoNameString in serverInfoNameArray)
            {
                discordApplicationCommandOptionChoice[i] = new DiscordApplicationCommandOptionChoice(serverInfoNameString.ToUpper(), i.ToString());
                i++;
            }

            return discordApplicationCommandOptionChoice;
        }
    }
}
