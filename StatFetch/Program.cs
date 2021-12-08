using BotDLL.Model.BotCom.Discord;
using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;

namespace StatFetch
{
    public class Program
    {
        private static DiscordBot? discordBot;
        private static List<ServerInfo> serverInfoList = new();
        private static List<ServerStat> serverStatListLive = new();
        private static List<ServerStat> serverStatListCompare0 = new();
        private static List<ServerStat> serverStatListCompare1 = new();
        public static List<UpTime> upTimeList = new();
        static async Task Main()
        {
            int counter = 0;
            #region ConsoleSize
            try
            {
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                Console.SetWindowSize(255, 40);
            }
            catch (Exception)
            {
                Console.SetWindowSize(100, 10);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
            }
            #endregion

            ServerInfo.CreateTable();
            UpTime.CreateTable();
            DCUserdata.CreateTable();
            serverInfoList = ServerInfo.ReadAll();
            Task upTimeCheckTask = UpTimeCheck();

            discordBot = new DiscordBot();
            await discordBot.RunAsync();

            while (true)
            {
                serverStatListLive.Clear();
                foreach (ServerInfo serverInfoObj in serverInfoList)
                {
                    ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                    serverStatListLive.Add(serverStatObj);
                }

                if (counter % 2 == 0)
                {
                    serverStatListCompare0.Clear();
                    serverStatListCompare0.AddRange(serverStatListLive);

                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleForamter.WriteStatList(serverStatListLive);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    serverStatListCompare1.Clear();
                    serverStatListCompare1.AddRange(serverStatListLive);

                    ConsoleForamter.WriteStatList(serverStatListLive);
                }

                ChangeCheck();

                if (counter == 60)
                {
                    serverInfoList = ServerInfo.ReadAll();
                    counter = 0;
                }

                counter++;
                await Task.Delay(1000);
            }
        }
        static void ChangeCheck()
        {
            foreach (ServerStat serverStatObjCompare0 in serverStatListCompare0)
            {
                foreach (ServerStat serverStatObjCompare1 in serverStatListCompare1)
                {
                    if (serverStatObjCompare0.ServerInfoId == serverStatObjCompare1.ServerInfoId)
                    {
                        if (serverStatObjCompare0.ServerUp != serverStatObjCompare1.ServerUp || serverStatObjCompare0.Players != serverStatObjCompare1.Players)
                        {
                            if (serverStatObjCompare0.FetchTime > serverStatObjCompare1.FetchTime)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ConsoleForamter.FillRow();
                                ConsoleForamter.Center(serverStatObjCompare0.ToString());
                                ConsoleForamter.FillRow();
                                ConsoleForamter.Center(serverStatObjCompare1.ToString());
                                ConsoleForamter.FillRow();
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ConsoleForamter.FillRow();
                                ConsoleForamter.Center(serverStatObjCompare1.ToString());
                                ConsoleForamter.FillRow();
                                ConsoleForamter.Center(serverStatObjCompare0.ToString());
                                ConsoleForamter.FillRow();
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }


                            bool isminimal = false;
                            if (serverStatObjCompare0.Players == 1 && serverStatObjCompare1.Players == 0 || serverStatObjCompare0.Players == 0 && serverStatObjCompare1.Players == 1)
                                isminimal = true;

                            if (serverStatObjCompare0.ServerUp != serverStatObjCompare1.ServerUp)
                            {
                                if (serverStatObjCompare0.FetchTime > serverStatObjCompare1.FetchTime)
                                    DiscordBot.DCChange(serverStatObjCompare0, "status", isminimal);
                                else
                                    DiscordBot.DCChange(serverStatObjCompare1, "status", isminimal);
                            }
                            else if (serverStatObjCompare0.Players != serverStatObjCompare1.Players)
                            {
                                if (serverStatObjCompare0.FetchTime > serverStatObjCompare1.FetchTime)
                                    DiscordBot.DCChange(serverStatObjCompare0, "player", isminimal);
                                else
                                    DiscordBot.DCChange(serverStatObjCompare1, "player", isminimal);
                            }
                        }
                    }
                }
            }
        }
        public static async Task UpTimeCheck()
        {
            List<ServerInfo> serverInfoListUpTime = new();
            List<ServerStat> serverStatListUpTime = new();

            await Task.Run(async () =>
            {
                while (true)
                {
                    while (DateTime.Now.Second != 0)
                    {
                        await Task.Delay(100);
                    }

                    serverInfoListUpTime = ServerInfo.ReadAll();

                    serverStatListUpTime.Clear();
                    foreach (ServerInfo serverInfoObj in serverInfoListUpTime)
                    {
                        ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                        serverStatListUpTime.Add(serverStatObj);
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    ConsoleForamter.FillRow();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    ConsoleForamter.FillRow();
                    Console.ForegroundColor = ConsoleColor.Gray;

                    upTimeList = UpTime.ReadAll();

                    foreach (ServerStat serverStatObjItem in serverStatListUpTime)
                    {
                        bool found = false;
                        foreach (UpTime upTimeObjItem in upTimeList)
                        {
                            if (upTimeObjItem.ServerInfoId == serverStatObjItem.ServerInfoId)
                                found = true;
                        }

                        if (!found)
                        {
                            UpTime upTime = new()
                            {
                                ServerInfoId = serverStatObjItem.ServerInfoId,
                                Successful = 0,
                                Unsuccessful = 0,
                                InPercent = 0,
                            };
                            UpTime.Add(upTime);
                        }
                    }

                    foreach (ServerStat serverStatObjItem in serverStatListUpTime)
                    {
                        foreach (UpTime upTimeObjItem in upTimeList)
                        {
                            if (upTimeObjItem.ServerInfoId == serverStatObjItem.ServerInfoId && serverStatObjItem.ServerUp)
                            {
                                upTimeObjItem.Successful++;
                                if (upTimeObjItem.Unsuccessful == 0)
                                    upTimeObjItem.InPercent = 100.0;
                                else
                                    upTimeObjItem.InPercent = Math.Round((Convert.ToDouble(upTimeObjItem.Successful) / Convert.ToDouble(upTimeObjItem.Successful + upTimeObjItem.Unsuccessful) * 100), 2);
                                serverStatObjItem.UpTimeInPercent = upTimeObjItem.InPercent;
                                UpTime.Change(upTimeObjItem);
                            }
                            else if (upTimeObjItem.ServerInfoId == serverStatObjItem.ServerInfoId && !serverStatObjItem.ServerUp)
                            {
                                upTimeObjItem.Unsuccessful++;
                                if (upTimeObjItem.Successful == 0)
                                    upTimeObjItem.InPercent = 0.0;
                                else
                                    upTimeObjItem.InPercent = Math.Round((Convert.ToDouble(upTimeObjItem.Successful) / Convert.ToDouble(upTimeObjItem.Successful + upTimeObjItem.Unsuccessful) * 100), 2);
                                serverStatObjItem.UpTimeInPercent = upTimeObjItem.InPercent;
                                UpTime.Change(upTimeObjItem);
                            }
                        }
                    }

                    foreach (ServerInfo serverInfoObjItem in serverInfoListUpTime)
                    {
                        foreach (ServerStat serverStatObjItem in serverStatListUpTime)
                        {
                            if (serverStatObjItem.ServerInfoId == serverInfoObjItem.ServerInfoId)
                            {
                                serverInfoObjItem.UpTimeInPercent = serverStatObjItem.UpTimeInPercent;
                                ServerInfo.Update(serverInfoObjItem);
                            }
                        }
                    }
                }
            });
        }
    }
}