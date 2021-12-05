using BotDLL.Model.BotCom.Discord;
using BotDLL.Model.Objects;

namespace StatFetch
{
    public class Program
    {
        private static DiscordBot? discordBot;
        private static List<ServerStat> serverStatListLive = new();
        private static List<ServerStat> serverStatListCompare0 = new();
        private static List<ServerStat> serverStatListCompare1 = new();
        private static string consoleString = "";
        static async Task Main()
        {
            int counter = 0;
            #region ConsoleSize
            try
            {
                Console.SetWindowSize(210, 49);
            }
            catch (Exception)
            {
                Console.SetWindowSize(100, 10);
            }
            #endregion

            ServerInfo.CreateTable_ServerInfo();
            DC_Userdata.CreateTable_Userdata();
            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();

            discordBot = new DiscordBot();
            await discordBot.RunAsync();

            while (true)
            {
                if (counter == 10)
                {
                    serverInfoList = ServerInfo.ReadAll();
                    counter = 0;
                }

                serverStatListLive.Clear();
                consoleString = "".PadLeft(207, '█') + "\n";
                foreach (ServerInfo serverInfoObj in serverInfoList)
                {
                    ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                    serverStatListLive.Add(serverStatObj);
                    consoleString += serverStatObj + "\n";
                }
                consoleString += "".PadLeft(207, '█') + "\n";
                Console.Write(consoleString);

                if (counter % 2 == 0)
                {
                    serverStatListCompare0.Clear();
                    serverStatListCompare0.AddRange(serverStatListLive);
                }
                else
                {
                    serverStatListCompare1.Clear();
                    serverStatListCompare1.AddRange(serverStatListLive);
                }

                foreach (ServerStat serverStatObjCompare0 in serverStatListCompare0)
                {
                    foreach (ServerStat serverStatObjCompare1 in serverStatListCompare1)
                    {

                        if (serverStatObjCompare0.Id == serverStatObjCompare1.Id && serverStatObjCompare0.Players != serverStatObjCompare1.Players)
                        {
                            consoleString = "";

                            if (serverStatObjCompare0.FetchTime > serverStatObjCompare1.FetchTime)
                            {
                                DiscordBot.DCChange(serverStatObjCompare0, "player");
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare0 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare1 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                            }
                            else
                            {
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare1 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare0 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                DiscordBot.DCChange(serverStatObjCompare1, "player");
                            }

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(consoleString);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        if(serverStatObjCompare0.Id == serverStatObjCompare1.Id && serverStatObjCompare0.ServerUp != serverStatObjCompare1.ServerUp)
                        {
                            consoleString = "";
                            if (serverStatObjCompare0.FetchTime > serverStatObjCompare1.FetchTime)
                            {
                                DiscordBot.DCChange(serverStatObjCompare0, "status");
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare0 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare1 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                            }
                            else
                            {
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare1 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                consoleString += serverStatObjCompare0 + "\n";
                                consoleString += "".PadLeft(207, '█') + "\n";
                                DiscordBot.DCChange(serverStatObjCompare1, "status");
                            }
                        }
                    }
                }

                counter++;
                await Task.Delay(1000);
            }
        }
    }
}