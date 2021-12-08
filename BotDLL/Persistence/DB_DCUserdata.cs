using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
    public class DB_DCUserdata
    {
        public static List<DCUserdata> ReadAll()
        {
            string sqlCommand = "SELECT * FROM DCUserdata";
            List<DCUserdata> dC_UserdataList = new();
            MySqlConnection mySqlConnection = DB_Connection.OpenDB();
            MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

            while (mySqlDataReader.Read())
            {
                DCUserdata dC_UserdataObj = new()
                {
                    DCUserdataID = mySqlDataReader.GetUInt16("DCUserdataID"),
                    ServerInfoId = mySqlDataReader.GetUInt16("ServerInfoId"),
                    AuthorId = mySqlDataReader.GetUInt64("AuthorId"),
                    ChannelId = mySqlDataReader.GetUInt64("ChannelId"),
                    Abo = mySqlDataReader.GetBoolean("Abo"),
                    IsMinimalAbo = mySqlDataReader.GetBoolean("IsMinimalAbo")
                };
                dC_UserdataList.Add(dC_UserdataObj);
            }

            DB_Connection.CloseDB(mySqlConnection);
            return dC_UserdataList;
        }
        public static void Add(DCUserdata dC_UserdataObj)
        {
            string sqlCommand = $"INSERT INTO DCUserdata (ServerInfoId, AuthorId, ChannelId, Abo, IsMinimalAbo) " +
                                $"VALUES ({dC_UserdataObj.ServerInfoId}, {dC_UserdataObj.AuthorId}, {dC_UserdataObj.ChannelId}, {dC_UserdataObj.Abo}, {dC_UserdataObj.IsMinimalAbo})";
            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
        public static void Change(DCUserdata dC_UserdataObj)
        {
            string sqlCommand = $"UPDATE DCUserdata SET Abo={dC_UserdataObj.Abo}, IsMinimalAbo={dC_UserdataObj.IsMinimalAbo} WHERE ServerInfoId={dC_UserdataObj.ServerInfoId} AND AuthorId={dC_UserdataObj.AuthorId} AND ChannelId={dC_UserdataObj.ChannelId}";
            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
        public static void CreateTable()
        {
            Connections connetions = CSV_Connections.ReadAll();

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            string database = WordCutter.RemoveUntilWord(connetions.MySqlConStr, "Database=", 9);
#if DEBUG
            database = WordCutter.RemoveUntilWord(connetions.MySqlConStrDebug, "Database=", 9);
#endif
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
            database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

            string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" +
                            $"USE `{database}`;" +
                            "CREATE TABLE IF NOT EXISTS `DCUserdata` (" +
                            "`DCUserdataID` MEDIUMINT NOT NULL AUTO_INCREMENT," +
                            "`ServerInfoId` MEDIUMINT DEFAULT NULL," +
                            "`AuthorId` BIGINT NOT NULL," +
                            "`ChannelId` BIGINT DEFAULT NULL," +
                            "`Abo` TINYINT DEFAULT NULL," +
                            "`IsMinimalAbo` TINYINT DEFAULT NULL," +
                            "PRIMARY KEY (DCUserdataID)," +
                            "KEY (ServerInfoId)" +
                            ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
    }
}
