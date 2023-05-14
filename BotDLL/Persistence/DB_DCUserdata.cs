using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
   public class DB_DCUserdata
   {
      public static List<DCUserData> ReadAll()
      {
         string sqlCommand = "SELECT * FROM DCUserData";
         List<DCUserData> dC_UserdataList = new();
         MySqlConnection mySqlConnection = DB_Connection.OpenDB();
         MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

         while (mySqlDataReader.Read())
         {
            DCUserData dCUserDataObj = new()
            {
                     DCUserDataID = mySqlDataReader.GetUInt16("DCUserDataID"),
                     ServerInfoId = mySqlDataReader.GetUInt16("ServerInfoId"),
                     AuthorId = mySqlDataReader.GetUInt64("AuthorId"),
                     ChannelId = mySqlDataReader.GetUInt64("ChannelId"),
                     Abo = mySqlDataReader.GetBoolean("Abo"),
                     IsMinimalAbo = mySqlDataReader.GetBoolean("IsMinimalAbo")
            };
            dC_UserdataList.Add(dCUserDataObj);
         }

         DB_Connection.CloseDB(mySqlConnection);
         return dC_UserdataList;
      }

      public static void Add(DCUserData dCUserDataObj)
      {
         string sqlCommand = "INSERT INTO DCUserData (ServerInfoId, AuthorId, ChannelId, Abo, IsMinimalAbo) " + $"VALUES ({dCUserDataObj.ServerInfoId}, {dCUserDataObj.AuthorId}, {dCUserDataObj.ChannelId}, {dCUserDataObj.Abo}, {dCUserDataObj.IsMinimalAbo})";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void Change(DCUserData dCUserDataObj)
      {
         string sqlCommand = $"UPDATE DCUserData SET Abo={dCUserDataObj.Abo}, IsMinimalAbo={dCUserDataObj.IsMinimalAbo} WHERE ServerInfoId={dCUserDataObj.ServerInfoId} AND AuthorId={dCUserDataObj.AuthorId} AND ChannelId={dCUserDataObj.ChannelId}";
         DB_Connection.ExecuteNonQuery(sqlCommand);
      }

      public static void CreateTable()
      {
         Connections connections = CSV_Connections.ReadAll();


#if RELEASE
         string? database = WordCutter.RemoveUntilWord(connections.MySqlConStr, "Database=", 9);
#elif DEBUG
         string database = WordCutter.RemoveUntilWord(connections.MySqlConStrDebug, "Database=", 9);
#endif

         database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

         string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" + $"USE `{database}`;" + "CREATE TABLE IF NOT EXISTS `DCUserData` (" + "`DCUserDataID` MEDIUMINT NOT NULL AUTO_INCREMENT," + "`ServerInfoId` MEDIUMINT DEFAULT NULL," + "`AuthorId` BIGINT NOT NULL," + "`ChannelId` BIGINT DEFAULT NULL," + "`Abo` TINYINT DEFAULT NULL," + "`IsMinimalAbo` TINYINT DEFAULT NULL," + "PRIMARY KEY (DCUserDataID)," + "KEY (ServerInfoId)" + ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

         DB_Connection.ExecuteNonQuery(sqlCommand);
      }
   }
}