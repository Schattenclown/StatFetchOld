using BotDLL.Model.BotCom.Discord.Main;
using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using static System.Console;

namespace StatFetch
{
   public class Program
   {
      private static Bot? _discordBot;
      private static List<ServerInfo?> _serverInfoList = new();
      private static readonly List<ServerStat> ServerStatListLive = new();
      private static readonly List<ServerStat> ServerStatCompareList0 = new();
      private static readonly List<ServerStat> ServerStatCompareList1 = new();
      private static List<UpTime> _upTimeList = new();

      private static async Task Main()
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

      private static async Task StartAsync()
      {
         int counter = 0;

         #region ConsoleSize

         try
         {
            if (OperatingSystem.IsWindows())
            {
               SetWindowSize(225, 30);
            }
         }
         catch (Exception)
         {
            if (OperatingSystem.IsWindows())
            {
               SetWindowSize(100, 10);
            }
         }

         #endregion

         ServerInfo.CreateTable();
         UpTime.CreateTable();
         DCUserData.CreateTable();
         _serverInfoList = ServerInfo.ReadAll();
         MonthStatistics.CreateTables(_serverInfoList);
         UpTimeCheck();
         MaxPlayerCheck();

         _ = Task.Run(async () =>
         {
            while (true)
            {
               try
               {
                  ServerStatListLive.Clear();
                  foreach (ServerStat serverStatObj in _serverInfoList.Select(ServerStat.CreateObj))
                  {
                     ServerStatListLive.Add(serverStatObj);
                  }

                  if (counter % 2 == 0)
                  {
                     ServerStatCompareList0.Clear();
                     ServerStatCompareList0.AddRange(ServerStatListLive);

                     ForegroundColor = ConsoleColor.White;
                     ConsoleFormatter.WriteStatList(ServerStatListLive);
                     ForegroundColor = ConsoleColor.Gray;
                  }
                  else
                  {
                     ServerStatCompareList1.Clear();
                     ServerStatCompareList1.AddRange(ServerStatListLive);

                     ConsoleFormatter.WriteStatList(ServerStatListLive);
                  }

                  ChangeCheck();

                  if (counter == 60 / 5)
                  {
                     _serverInfoList = ServerInfo.ReadAll();
                     counter = 0;
                  }

                  counter++;
                  await Task.Delay(1000 * 5);
               }
               catch (Exception ex)
               {
                  WriteLine(ex.Message);
               }
            }
         });

         _discordBot = new Bot();
         await _discordBot.RunAsync();
      }

      private static void ChangeCheck()
      {
         foreach (ServerStat serverStatCompareItem0 in ServerStatCompareList0)
         foreach (ServerStat serverStatCompareItem1 in ServerStatCompareList1.Where(serverStatCompareItem1 => serverStatCompareItem0.ServerInfoId == serverStatCompareItem1.ServerInfoId).Where(serverStatCompareItem1 => serverStatCompareItem0.ServerUp != serverStatCompareItem1.ServerUp || serverStatCompareItem0.Players != serverStatCompareItem1.Players))
         {
            if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
            {
               ForegroundColor = ConsoleColor.Red;
               ConsoleFormatter.FillRow(1);
               ConsoleFormatter.Center(serverStatCompareItem0.ToString());
               ConsoleFormatter.Center(serverStatCompareItem1.ToString());
               ConsoleFormatter.FillRow(3);
               ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
               ForegroundColor = ConsoleColor.Red;
               ConsoleFormatter.FillRow(1);
               ConsoleFormatter.Center(serverStatCompareItem1.ToString());
               ConsoleFormatter.Center(serverStatCompareItem0.ToString());
               ConsoleFormatter.FillRow(3);
               ForegroundColor = ConsoleColor.Gray;
            }


            bool isMinimal = (serverStatCompareItem0.Players == 1 && serverStatCompareItem1.Players == 0) || (serverStatCompareItem0.Players == 0 && serverStatCompareItem1.Players == 1);

            if (serverStatCompareItem0.ServerUp != serverStatCompareItem1.ServerUp)
            {
               Bot.DC_Change(serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime ? serverStatCompareItem0 : serverStatCompareItem1, "status", isMinimal);
            }
            else if (serverStatCompareItem0.Players != serverStatCompareItem1.Players)
            {
               Bot.DC_Change(serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime ? serverStatCompareItem0 : serverStatCompareItem1, "player", isMinimal);
            }
         }
      }

      public static void UpTimeCheck()
      {
         List<ServerInfo?> serverInfoListUpTime;
         List<ServerStat> serverStatListUpTime = new();

         _ = Task.Run(async () =>
         {
            while (true)
            {
               try
               {
                  while (DateTime.Now.Second != 59)
                  {
                     await Task.Delay(500);
                  }

                  ForegroundColor = ConsoleColor.Red;
                  ConsoleFormatter.FillRow(1);
                  ForegroundColor = ConsoleColor.Yellow;
                  ConsoleFormatter.FillRow(3);
                  ForegroundColor = ConsoleColor.Gray;

                  serverInfoListUpTime = ServerInfo.ReadAll();
                  _upTimeList = UpTime.ReadAll();

                  serverStatListUpTime.Clear();
                  serverStatListUpTime.AddRange(serverInfoListUpTime.Select(ServerStat.CreateObj));

                  foreach (ServerStat serverStatItem in serverStatListUpTime)
                  {
                     bool found = false;
                     foreach (UpTime dummy in _upTimeList.Where(upTimeItem => upTimeItem.ServerInfoId == serverStatItem.ServerInfoId))
                     {
                        found = true;
                     }

                     if (found)
                     {
                        continue;
                     }

                     UpTime upTime = new()
                     {
                        ServerInfoId = serverStatItem.ServerInfoId,
                        Successful = 0,
                        Unsuccessful = 0,
                        InPercent = 0
                     };
                     UpTime.Add(upTime);
                  }

                  foreach (ServerStat serverStatItem in serverStatListUpTime)
                  foreach (UpTime upTimeItem in _upTimeList)
                  {
                     if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId && serverStatItem.ServerUp)
                     {
                        upTimeItem.Successful++;
                        upTimeItem.InPercent = upTimeItem.Unsuccessful == 0 ? 100.0 : Math.Round(Convert.ToDouble(upTimeItem.Successful) / Convert.ToDouble(upTimeItem.Successful + upTimeItem.Unsuccessful) * 100, 2);
                        serverStatItem.UpTimeInPercent = upTimeItem.InPercent;
                        UpTime.Change(upTimeItem);
                     }
                     else if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId && !serverStatItem.ServerUp)
                     {
                        upTimeItem.Unsuccessful++;
                        upTimeItem.InPercent = upTimeItem.Successful == 0 ? 0.0 : Math.Round(Convert.ToDouble(upTimeItem.Successful) / Convert.ToDouble(upTimeItem.Successful + upTimeItem.Unsuccessful) * 100, 2);
                        serverStatItem.UpTimeInPercent = upTimeItem.InPercent;
                        UpTime.Change(upTimeItem);
                     }
                  }

                  foreach (ServerInfo? serverInfoItem in serverInfoListUpTime)
                  foreach (ServerStat serverStatItem in serverStatListUpTime.Where(serverStatItem => serverInfoItem != null && serverStatItem.ServerInfoId == serverInfoItem.ServerInfoId))
                  {
                     if (serverInfoItem != null)
                     {
                        serverInfoItem.UpTimeInPercent = serverStatItem.UpTimeInPercent;
                        ServerInfo.Update(serverInfoItem);
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

      public static void MaxPlayerCheck()
      {
         List<ServerInfo?> serverInfoListMaxPlayer;
         List<ServerInfo?> serverInfoListWithMs;

         _ = Task.Run(async () =>
         {
            while (true)
            {
               while (DateTime.Now.Second != 29)
               {
                  await Task.Delay(500);
               }

               ForegroundColor = ConsoleColor.Blue;
               ConsoleFormatter.FillRow(1);
               ForegroundColor = ConsoleColor.Yellow;
               ConsoleFormatter.FillRow(3);
               ForegroundColor = ConsoleColor.Gray;

               serverInfoListMaxPlayer = ServerInfo.ReadAll();
               serverInfoListWithMs = MonthStatistics.ReadAll(serverInfoListMaxPlayer);

               foreach (ServerInfo? serverInfoItem in serverInfoListWithMs)
               {
                  bool todayFound = false;

                  foreach (MonthStatistics monthStatisticsItem in serverInfoItem?.MonthStatisticsList!)
                  {
                     if (monthStatisticsItem.Date.Date != DateTime.Now.Date)
                     {
                        continue;
                     }

                     ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                     if (serverStatObj.Players > monthStatisticsItem.MaxPlayers)
                     {
                        MonthStatistics.Change(serverStatObj);
                     }

                     todayFound = true;
                  }


                  if (todayFound)
                  {
                     continue;
                  }

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