﻿using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
   internal class DB_MonthStatistics
   {
      public static List<ServerInfo?> ReadAll(List<ServerInfo?> serverInfoList)
      {
         List<ServerInfo?> serverInfoListWithMS = new();

         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            string serverInfoIdString = $"{serverInfoItem.ServerInfoId:#0000000}MonthStatistics";
            string sqlCommand = $"SELECT * FROM {serverInfoIdString}";

            MySqlConnection mySqlConnection = DB_Connection.OpenDB();
            MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);
            List<MonthStatistics> monthStatisticsList = new();

            while (mySqlDataReader.Read())
            {
               MonthStatistics monthStatisticsObj = new()
               {
                        Date = mySqlDataReader.GetDateTime("Date"),
                        MaxPlayers = mySqlDataReader.GetUInt16("MaxPlayers")
               };
               monthStatisticsList.Add(monthStatisticsObj);
            }

            ServerInfo? serverInfoObj = new()
            {
                     ServerInfoId = serverInfoItem.ServerInfoId,
                     Name = serverInfoItem.Name,
                     DynDnsAddress = serverInfoItem.DynDnsAddress,
                     Port = serverInfoItem.Port,
                     Game = serverInfoItem.Game,
                     UpTimeInPercent = serverInfoItem.UpTimeInPercent,
                     MonthStatisticsList = monthStatisticsList
            };
            serverInfoListWithMS.Add(serverInfoObj);
            DB_Connection.CloseDB(mySqlConnection);
         }

         return serverInfoListWithMS;
      }

      public static ServerInfo? Read(ServerInfo? serverInfoObj)
      {
         string serverInfoIdString = $"{serverInfoObj.ServerInfoId:#0000000}MonthStatistics";
         string sqlCommand = $"SELECT * FROM {serverInfoIdString} order by date desc limit 60";

         MySqlConnection mySqlConnection = DB_Connection.OpenDB();
         MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);
         List<MonthStatistics> monthStatisticsList = new();

         while (mySqlDataReader.Read())
         {
            MonthStatistics monthStatisticsObj = new()
            {
                     Date = mySqlDataReader.GetDateTime("Date"),
                     MaxPlayers = mySqlDataReader.GetUInt16("MaxPlayers")
            };
            monthStatisticsList.Add(monthStatisticsObj);
         }

         serverInfoObj.MonthStatisticsList = monthStatisticsList;

         DB_Connection.CloseDB(mySqlConnection);

         return serverInfoObj;
      }

      public static void Add(ServerStat serverStatObj)
      {
         string serverInfoIdString = $"{serverStatObj.ServerInfoId:#0000000}MonthStatistics";

         string sqlCommand = $"INSERT INTO {serverInfoIdString} (Date, MaxPlayers) " + $"VALUES ('{DateTime.Now:yyyy-MM-dd 04:20:00}', {serverStatObj.Players})";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void Change(ServerStat serverStatObj)
      {
         string serverInfoIdString = $"{serverStatObj.ServerInfoId:#0000000}MonthStatistics";

         string sqlCommand = $"UPDATE {serverInfoIdString} SET MaxPlayers={serverStatObj.Players} WHERE Date BETWEEN '{DateTime.Now:yyyy-MM-dd 00:00:00}' AND '{DateTime.Now:yyyy-MM-dd 23:59:59}'";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void CreateTables(List<ServerInfo?> serverInfoList)
      {
         Connections connections = CSV_Connections.ReadAll();


#if RELEASE
         string? database = WordCutter.RemoveUntilWord(connections.MySqlConStr, "Database=", 9);
#elif DEBUG
         string database = WordCutter.RemoveUntilWord(connections.MySqlConStrDebug, "Database=", 9);
#endif

         database = WordCutter.RemoveAfterWord(database, "; Uid", 0);


         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            string serverInfoIdString = $"{serverInfoItem.ServerInfoId:#0000000}MonthStatistics";

            string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" + $"USE `{database}`;" + $"CREATE TABLE IF NOT EXISTS `{serverInfoIdString}` (" + "`Date` DATETIME NOT NULL," + "`MaxPlayers` MEDIUMINT NOT NULL," + "PRIMARY KEY (Date)" + ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            DB_Connection.ExecuteNonQuery(sqlCommand);
         }
      }
   }
}