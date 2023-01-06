using BotDLL.Model.BotCom.Discord.Main;
using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;

namespace StatFetch;

public class Program
{
   private static Bot? discordBot;
   private static List<ServerInfo> serverInfoList = new();
   private static readonly List<ServerStat> serverStatListLive = new();
   private static readonly List<ServerStat> serverStatCompareList0 = new();
   private static readonly List<ServerStat> serverStatCompareList1 = new();
   public static List<UpTime> upTimeList = new();

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
      var counter = 0;

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
      var upTimeCheckTask = UpTimeCheck();
      var maxPlayerCheck = MaxPlayerCheck();

      _ = Task.Run(async () =>
      {
         while (true)
            try
            {
               serverStatListLive.Clear();
               foreach (var serverInfoItem in serverInfoList)
               {
                  var serverStatObj = ServerStat.CreateObj(serverInfoItem);
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
      });

      discordBot = new Bot();
      await discordBot.RunAsync();
   }

   private static void ChangeCheck()
   {
      foreach (var serverStatCompareItem0 in serverStatCompareList0)
      foreach (var serverStatCompareItem1 in serverStatCompareList1)
         if (serverStatCompareItem0.ServerInfoId == serverStatCompareItem1.ServerInfoId)
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


               var isminimal = false;
               if ((serverStatCompareItem0.Players == 1 && serverStatCompareItem1.Players == 0) || (serverStatCompareItem0.Players == 0 && serverStatCompareItem1.Players == 1))
                  isminimal = true;

               if (serverStatCompareItem0.ServerUp != serverStatCompareItem1.ServerUp)
               {
                  if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
                     Bot.DCChange(serverStatCompareItem0, "status", isminimal);
                  else
                     Bot.DCChange(serverStatCompareItem1, "status", isminimal);
               }
               else if (serverStatCompareItem0.Players != serverStatCompareItem1.Players)
               {
                  if (serverStatCompareItem0.FetchTime > serverStatCompareItem1.FetchTime)
                     Bot.DCChange(serverStatCompareItem0, "player", isminimal);
                  else
                     Bot.DCChange(serverStatCompareItem1, "player", isminimal);
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
               while (DateTime.Now.Second != 59) await Task.Delay(500);

               Console.ForegroundColor = ConsoleColor.Red;
               ConsoleForamter.FillRow();
               Console.ForegroundColor = ConsoleColor.Yellow;
               ConsoleForamter.FillRow();
               Console.ForegroundColor = ConsoleColor.Gray;

               serverInfoListUpTime = ServerInfo.ReadAll();
               upTimeList = UpTime.ReadAll();

               serverStatListUpTime.Clear();
               foreach (var serverInfoItem in serverInfoListUpTime)
               {
                  var serverStatObj = ServerStat.CreateObj(serverInfoItem);
                  serverStatListUpTime.Add(serverStatObj);
               }

               foreach (var serverStatItem in serverStatListUpTime)
               {
                  var found = false;
                  foreach (var upTimeItem in upTimeList)
                     if (upTimeItem.ServerInfoId == serverStatItem.ServerInfoId)
                        found = true;

                  if (!found)
                  {
                     UpTime upTime = new() { ServerInfoId = serverStatItem.ServerInfoId, Successful = 0, Unsuccessful = 0, InPercent = 0 };
                     UpTime.Add(upTime);
                  }
               }

               foreach (var serverStatItem in serverStatListUpTime)
               foreach (var upTimeItem in upTimeList)
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

               foreach (var serverInfoItem in serverInfoListUpTime)
               foreach (var serverStatItem in serverStatListUpTime)
                  if (serverStatItem.ServerInfoId == serverInfoItem.ServerInfoId)
                  {
                     serverInfoItem.UpTimeInPercent = serverStatItem.UpTimeInPercent;
                     ServerInfo.Update(serverInfoItem);
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
            while (DateTime.Now.Second != 29) await Task.Delay(500);

            Console.ForegroundColor = ConsoleColor.Blue;
            ConsoleForamter.FillRow();
            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleForamter.FillRow();
            Console.ForegroundColor = ConsoleColor.Gray;

            serverInfoListMaxPlayer = ServerInfo.ReadAll();
            serverInfoListWithMS = MonthStatistics.ReadAll(serverInfoListMaxPlayer);

            foreach (var serverInfoItem in serverInfoListWithMS)
            {
               var todayFound = false;

               foreach (var monthStatisticsItem in serverInfoItem.MonthStatisticsList)
                  if (monthStatisticsItem.Date.Date == DateTime.Now.Date)
                  {
                     var serverStatObj = ServerStat.CreateObj(serverInfoItem);
                     if (serverStatObj.Players > monthStatisticsItem.MaxPlayers)
                        MonthStatistics.Change(serverStatObj);

                     todayFound = true;
                  }

               if (!todayFound)
               {
                  var serverStatObj = ServerStat.CreateObj(serverInfoItem);
                  MonthStatistics.Add(serverStatObj);
               }
            }

            await Task.Delay(1000);
         }
      });
   }
}