using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
    public class DB_ServerInfo
    {

        /*INSERT INTO ServerInfo(ServerInfo.Name, ServerInfo.DynDnsAddress, ServerInfo.Port) 
        VALUES("Slate", "0x360x39.de", 22165)*/
        public static List<ServerInfo> ReadAll()
        {
            string sqlCommand = "SELECT * FROM ServerInfo";
            List<ServerInfo> serverInfosList = new();
            MySqlConnection mySqlConnection = DB_Connection.OpenDB();
            MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

            while (mySqlDataReader.Read())
            {
                ServerInfo serverInfoObj = new()
                {
                    Id = mySqlDataReader.GetUInt16("Id"),
                    Name = mySqlDataReader.GetString("Name"),
                    DynDnsAddress = mySqlDataReader.GetString("DynDnsAddress"),
                    Port = mySqlDataReader.GetUInt16("Port")
                };
                if (mySqlDataReader.GetString("Game") == "Minecraft")
                    serverInfoObj.Game = Game.Minecraft;
                else if (mySqlDataReader.GetString("Game") == "SourceGame")
                    serverInfoObj.Game = Game.SourceGame;

                serverInfosList.Add(serverInfoObj);
            }

            DB_Connection.CloseDB(mySqlConnection);
            return serverInfosList;
        }
        public static void CreateTable_ServerInfo()
        {
            CSV_Connections cSV_Connections = new();
            Connections connections = CSV_Connections.ReadAll();

            string database = WordCutter.RemoveUntilWord(connections.MySqlConStr, "Database=", 9);
#if DEBUG
            database = WordCutter.RemoveUntilWord(connections.MySqlConStrDebug, "Database=", 9);
#endif
            database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

            string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" +
                            $"USE `{database}`;" +
                            "" +
                            "CREATE TABLE IF NOT EXISTS `ServerInfo` (" +
                            "`Id` MEDIUMINT NOT NULL AUTO_INCREMENT," +
                            "`Name` varchar(69) DEFAULT NULL," +
                            "`DynDnsAddress` varchar(69) DEFAULT NULL," +
                            "`Port` int DEFAULT NULL," +
                            "`Game` varchar(69) DEFAULT 'SourceGame'," +
                            "PRIMARY KEY (Id)" +
                            ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
    }
}
