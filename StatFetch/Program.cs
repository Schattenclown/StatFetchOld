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
      private static List<ServerStat> serverStatCompareList0 = new();
      private static List<ServerStat> serverStatCompareList1 = new();
      public static List<UpTime> upTimeList = new();
      static async Task Main()
      {
         try
         {
            await StartAsync();
         }
         catch (Exception ex)
         {
            Reset.RestartProgram(ex);
         }
      }
      static async Task StartAsync()
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
         MonthStatistics.CreateTables(serverInfoList);
         Task upTimeCheckTask = UpTimeCheck();
         Task maxPlayerCheck = MaxPlayerCheck();

         discordBot = new DiscordBot();
         await discordBot.RunAsync();

         while (true)
         {
            try
            {
               serverStatListLive.Clear();
               foreach (ServerInfo serverInfoItem in serverInfoList)
               {
                  ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                  serverStatListLive.Add(serverStatObj);
               }

               if (counter % 2 == 0)
               {
                  serverStatCompareList0.Clear();
                  serverStatCompareList0.AddRange(serverStatListLive);

                  Console.ForegroundColor = ConsoleColor.White;
                  ConsoleForamter.WriteStatList(serverStatListLive);
                  Console.ForegroundColor = ConsoleColor.Gray;
               }
               else
               {
                  serverStatCompareList1.Clear();
                  serverStatCompareList1.AddRange(serverStatListLive);

                  ConsoleForamter.WriteStatList(serverStatListLive);
               }

               ChangeCheck();

               if (counter == 60 / 5)
               {
                  serverInfoList = ServerInfo.ReadAll();
                  counter = 0;
               }

               counter++;
               await Task.Delay(1000 * 5);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
         }
      }
      static void ChangeCheck()
      {
         foreach (ServerStat serverStatCompareItem0 in serverStatCompareList0)
         {
            foreach (ServerStat serverStatCompareItem1 in serverStatCompareList1)
            {
               if (serverStatCompareItem0.ServerInfoId == serverStatCompareItem1.ServerInfoId)
               {
                  if (serverStatCompareItem0.ServerUp != serverStatCompareItem1.ServerUp || serverStatCompareItem0.Players != serverStatCompareItem1.Players)
                  {
                     if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
                     {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleForamter.FillRow();
                        ConsoleForamter.Center(serverStatCompareItem0.ToString());
                        ConsoleForamter.FillRow();
                        ConsoleForamter.Center(serverStatCompareItem1.ToString());
                        ConsoleForamter.FillRow();
                        Console.ForegroundColor = ConsoleColor.Gray;
                     }
                     else
                     {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleForamter.FillRow();
                        ConsoleForamter.Center(serverStatCompareItem1.ToString());
                        ConsoleForamter.FillRow();
                        ConsoleForamter.Center(serverStatCompareItem0.ToString());
                        ConsoleForamter.FillRow();
                        Console.ForegroundColor = ConsoleColor.Gray;
                     }


                     bool isminimal = false;
                     if ((serverStatCompareItem0.Players == 1 && serverStatCompareItem1.Players == 0) || (serverStatCompareItem0.Players == 0 && serverStatCompareItem1.Players == 1))
                        isminimal = true;

                     if (serverStatCompareItem0.ServerUp != serverStatCompareItem1.ServerUp)
                     {
                        if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
                           DiscordBot.DCChange(serverStatCompareItem0, "status", isminimal);
                        else
                           DiscordBot.DCChange(serverStatCompareItem1, "status", isminimal);
                     }
                     else if (serverStatCompareItem0.Players != serverStatCompareItem1.Players)
                     {
                        if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
                           DiscordBot.DCChange(serverStatCompareItem0, "player", isminimal);
                        else
                           DiscordBot.DCChange(serverStatCompareItem1, "player", isminimal);
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
               try
               {
                  while (DateTime.Now.Second != 59)
                  {
                     await Task.Delay(500);
                  }

                  Console.ForegroundColor = ConsoleColor.Red;
                  ConsoleForamter.FillRow();
                  Console.ForegroundColor = ConsoleColor.Yellow;
                  ConsoleForamter.FillRow();
                  Console.ForegroundColor = ConsoleColor.Gray;

                  serverInfoListUpTime = ServerInfo.ReadAll();
                  upTimeList = UpTime.ReadAll();

                  serverStatListUpTime.Clear();
                  foreach (ServerInfo serverInfoItem in serverInfoListUpTime)
                  {
                     ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                     serverStatListUpTime.Add(serverStatObj);
                  }

                  foreach (ServerStat serverStatItem in serverStatListUpTime)
                  {
                     bool found = false;
                     foreach (UpTime upTimeItem in upTimeList)
                     {
                        if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId)
                           found = true;
                     }

                     if (!found)
                     {
                        UpTime upTime = new()
                        {
                           ServerInfoId = serverStatItem.ServerInfoId,
                           Successful = 0,
                           Unsuccessful = 0,
                           InPercent = 0,
                        };
                        UpTime.Add(upTime);
                     }
                  }

                  foreach (ServerStat serverStatItem in serverStatListUpTime)
                  {
                     foreach (UpTime upTimeItem in upTimeList)
                     {
                        if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId && serverStatItem.ServerUp)
                        {
                           upTimeItem.Successful++;
                           if (upTimeItem.Unsuccessful == 0)
                              upTimeItem.InPercent = 100.0;
                           else
                              upTimeItem.InPercent = Math.Round(Convert.ToDouble(upTimeItem.Successful) / Convert.ToDouble(upTimeItem.Successful + upTimeItem.Unsuccessful) * 100, 2);
                           serverStatItem.UpTimeInPercent = upTimeItem.InPercent;
                           UpTime.Change(upTimeItem);
                        }
                        else if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId && !serverStatItem.ServerUp)
                        {
                           upTimeItem.Unsuccessful++;
                           if (upTimeItem.Successful == 0)
                              upTimeItem.InPercent = 0.0;
                           else
                              upTimeItem.InPercent = Math.Round(Convert.ToDouble(upTimeItem.Successful) / Convert.ToDouble(upTimeItem.Successful + upTimeItem.Unsuccessful) * 100, 2);
                           serverStatItem.UpTimeInPercent = upTimeItem.InPercent;
                           UpTime.Change(upTimeItem);
                        }
                     }
                  }

                  foreach (ServerInfo serverInfoItem in serverInfoListUpTime)
                  {
                     foreach (ServerStat serverStatItem in serverStatListUpTime)
                     {
                        if (serverStatItem.ServerInfoId == serverInfoItem.ServerInfoId)
                        {
                           serverInfoItem.UpTimeInPercent = serverStatItem.UpTimeInPercent;
                           ServerInfo.Update(serverInfoItem);
                        }
                     }
                  }
               }
               catch
               {
                  //ignored
               }

               await Task.Delay(1000);
            }
         });
      }
      public static async Task MaxPlayerCheck()
      {
         List<ServerInfo> serverInfoListMaxPlayer = new();
         List<ServerInfo> serverInfoListWithMS = new();

         await Task.Run(async () =>
         {
            while (true)
            {
               while (DateTime.Now.Second != 29)
               {
                  await Task.Delay(500);
               }

               Console.ForegroundColor = ConsoleColor.Blue;
               ConsoleForamter.FillRow();
               Console.ForegroundColor = ConsoleColor.Yellow;
               ConsoleForamter.FillRow();
               Console.ForegroundColor = ConsoleColor.Gray;

               serverInfoListMaxPlayer = ServerInfo.ReadAll();
               serverInfoListWithMS = MonthStatistics.ReadAll(serverInfoListMaxPlayer);

               foreach (ServerInfo serverInfoItem in serverInfoListWithMS)
               {
                  bool todayFound = false;

                  foreach (MonthStatistics monthStatisticsItem in serverInfoItem.MonthStatisticsList)
                  {
                     if (monthStatisticsItem.Date.Date == DateTime.Now.Date)
                     {
                        ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                        if (serverStatObj.Players > monthStatisticsItem.MaxPlayers)
                           MonthStatistics.Change(serverStatObj);

                        todayFound = true;
                     }
                  }

                  if (!todayFound)
                  {
                     ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                     MonthStatistics.Add(serverStatObj);
                  }
               }
               await Task.Delay(1000);
            }
         });
      }
   }
}