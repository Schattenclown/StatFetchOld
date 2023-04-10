using System.Globalization;
using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
   public class DbServerInfo
   {
      public static List<ServerInfo?> ReadAll()
      {
         string sqlCommand = "SELECT * FROM ServerInfo";
         List<ServerInfo?> serverInfosList = new();
         MySqlConnection mySqlConnection = DB_Connection.OpenDB();
         MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

         while (mySqlDataReader.Read())
         {
            ServerInfo? serverInfoObj = new()
            {
               ServerInfoId = mySqlDataReader.GetUInt16("ServerInfoId"),
               Name = mySqlDataReader.GetString("Name"),
               DynDnsAddress = mySqlDataReader.GetString("DynDnsAddress"),
               Port = mySqlDataReader.GetUInt16("Port"),
               UpTimeInPercent = mySqlDataReader.GetDouble("UpTimeInPercent")
            };
            if (mySqlDataReader.GetString("Game") == "Minecraft")
            {
               serverInfoObj.Game = Game.Minecraft;
            }
            else if (mySqlDataReader.GetString("Game") == "SourceGame")
            {
               serverInfoObj.Game = Game.SourceGame;
            }

            serverInfosList.Add(serverInfoObj);
         }

         DB_Connection.CloseDB(mySqlConnection);
         return serverInfosList;
      }

      public static void ChangeUpTime(ServerInfo? serverInfoObj)
      {
         string sqlCommand = $"UPDATE ServerInfo SET UpTimeInPercent={serverInfoObj?.UpTimeInPercent.ToString(CultureInfo.InvariantCulture).Replace(',', '.')} WHERE ServerInfoId={serverInfoObj!.ServerInfoId}";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void ChangeQCUri(ServerInfo serverInfoObj)
      {
         string sqlCommand = $"UPDATE ServerInfo SET QCUri={serverInfoObj.QcUri} WHERE ServerInfoId={serverInfoObj.ServerInfoId}";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void CreateTable()
      {
         CSV_Connections cSV_Connections = new();
         Connections connections = CSV_Connections.ReadAll();


#if RELEASE
         string? database = WordCutter.RemoveUntilWord(connections.MySqlConStr, "Database=", 9);
#elif DEBUG
         string database = WordCutter.RemoveUntilWord(connections.MySqlConStrDebug, "Database=", 9);
#endif

         database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

         string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" + $"USE `{database}`;" + "" + "CREATE TABLE IF NOT EXISTS `ServerInfo` (" + "`ServerInfoId` MEDIUMINT NOT NULL AUTO_INCREMENT," + "`Name` varchar(69) DEFAULT NULL," + "`DynDnsAddress` varchar(69) DEFAULT NULL," + "`Port` MEDIUMINT DEFAULT NULL," + "`Game` varchar(69) DEFAULT 'SourceGame'," + "`UpTimeInPercent` DOUBLE DEFAULT 100," + "`QCUri` varchar(420) DEFAULT NULL," + "PRIMARY KEY (ServerInfoId)" + ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

         DB_Connection.ExecuteNonQuery(sqlCommand);
      }
   }
}