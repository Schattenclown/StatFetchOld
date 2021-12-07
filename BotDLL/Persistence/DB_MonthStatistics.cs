using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
    class DB_MonthStatistics
    {
        public static void CreateTable_Userdata()
        {
            Connections connetions = CSV_Connections.ReadAll();

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            string database = WordCutter.RemoveUntilWord(connetions.MySqlConStr, "Database=", 9);
#if DEBUG
            database = WordCutter.RemoveUntilWord(connetions.MySqlConStrDebug, "Database=", 9);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
#endif
            database = WordCutter.RemoveAfterWord(database, "; Uid", 0);

            string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{database}`;" +
                            $"USE `{database}`;" +
                            "CREATE TABLE IF NOT EXISTS `DC_Userdata` (" +
                            "`Id` MEDIUMINT NOT NULL AUTO_INCREMENT," +
                            "`AuthorId` BIGINT NOT NULL," +
                            "`ChannelId` BIGINT DEFAULT NULL," +
                            "`ServerInfoId` MEDIUMINT DEFAULT NULL," +
                            "`Abo` MEDIUMINT DEFAULT NULL," +
                            "`MinimalAbo` MEDIUMINT DEFAULT NULL," +
                            "PRIMARY KEY (Id)," +
                            "KEY (ServerInfoId)" +
                            ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
    }
}
