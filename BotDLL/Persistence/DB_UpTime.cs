using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;

namespace BotDLL.Persistence;

public class DB_UpTime
{
   public static List<UpTime> ReadAll()
   {
      var sqlCommand = "SELECT * FROM UpTime";
      List<UpTime> upTimeList = new();
      var mySqlConnection = DB_Connection.OpenDB();
      var mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

      while (mySqlDataReader.Read())
      {
         UpTime upTimeObj = new()
         {
            UpTimeId = mySqlDataReader.GetUInt16("UpTimeId"),
            ServerInfoId = mySqlDataReader.GetUInt16("ServerInfoId"),
            Successful = mySqlDataReader.GetInt32("Successful"),
            Unsuccessful = mySqlDataReader.GetInt32("Unsuccessful"),
            InPercent = mySqlDataReader.GetDouble("InPercent")
         };
         upTimeList.Add(upTimeObj);
      }

      DB_Connection.CloseDB(mySqlConnection);
      return upTimeList;
   }

   public static void Add(UpTime upTimeObj)
   {
      var sqlCommand = "INSERT INTO UpTime (ServerInfoId, Successful, Unsuccessful, InPercent) " + $"VALUES ({upTimeObj.ServerInfoId}, {upTimeObj.Successful}, {upTimeObj.Unsuccessful}, '{upTimeObj.InPercent.ToString().Replace(',', '.')}')";
      DB_Connection.ExecuteNonQuery(sqlCommand);
   }

   public static void Change(UpTime upTimeObj)
   {
      var sqlCommand = $"UPDATE UpTime SET Successful={upTimeObj.Successful}, Unsuccessful={upTimeObj.Unsuccessful}, InPercent='{upTimeObj.InPercent.ToString().Replace(',', '.')}' WHERE UpTimeId={upTimeObj.UpTimeId} AND ServerInfoId={upTimeObj.ServerInfoId}";
      DB_Connection.ExecuteNonQuery(sqlCommand);
   }

   public static void CreateTable()
   {
      var connetions = CSV_Connections.ReadAll();

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
      var database = WordCutter.RemoveUntilWord(connetions.MySqlConStr, "Database=", 9);
#if DEBUG
      database = WordCutter.RemoveUntilWord(connetions.MySqlConStrDebug, "Database=", 9);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
#endif
      database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

      var sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" + $"USE `{database}`;" + "CREATE TABLE IF NOT EXISTS `UpTime` (" + "`UpTimeId` MEDIUMINT NOT NULL AUTO_INCREMENT," + "`ServerInfoId` MEDIUMINT DEFAULT NULL," + "`Successful` INT NOT NULL," + "`Unsuccessful` INT DEFAULT NULL," + "`InPercent` DOUBLE DEFAULT NULL," + "PRIMARY KEY (UpTimeId)," + "KEY (ServerInfoId)" + ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

      DB_Connection.ExecuteNonQuery(sqlCommand);
   }
}