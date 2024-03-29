﻿using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
   public class MonthStatistics
   {
      public DateTime Date { get; set; }
      public ushort MaxPlayers { get; set; }

      public static List<ServerInfo?> ReadAll(List<ServerInfo?> serverInfoList)
      {
         return DB_MonthStatistics.ReadAll(serverInfoList);
      }

      public static ServerInfo? Read(ServerInfo? serverInfoObj)
      {
         return DB_MonthStatistics.Read(serverInfoObj);
      }

      public static void Add(ServerStat serverStatObj)
      {
         DB_MonthStatistics.Add(serverStatObj);
      }

      public static void Change(ServerStat serverStatObj)
      {
         DB_MonthStatistics.Change(serverStatObj);
      }

      public static void CreateTables(List<ServerInfo?> serverInfoList)
      {
         DB_MonthStatistics.CreateTables(serverInfoList);
      }
   }
}