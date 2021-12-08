﻿using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using MySql.Data.MySqlClient;

namespace BotDLL.Persistence
{
    public class DB_DCUserdata
    {
        public static List<DCUserdata> ReadAll()
        {
            string sqlCommand = "SELECT * FROM DC_Userdata";
            List<DCUserdata> dC_UserdataList = new();
            MySqlConnection mySqlConnection = DB_Connection.OpenDB();
            MySqlDataReader mySqlDataReader = DB_Connection.ExecuteReader(sqlCommand, mySqlConnection);

            while (mySqlDataReader.Read())
            {
                DCUserdata dC_UserdataObj = new()
                {
                    DCUserdataID = mySqlDataReader.GetUInt16("DCUserdataID"),
                    AuthorId = mySqlDataReader.GetUInt64("AuthorId"),
                    ChannelId = mySqlDataReader.GetUInt64("ChannelId"),
                    ServerInfoId = mySqlDataReader.GetUInt16("ServerInfoId"),
                    Abo = mySqlDataReader.GetBoolean("Abo"),
                    MinimalAbo = mySqlDataReader.GetBoolean("MinimalAbo")
                };
                dC_UserdataList.Add(dC_UserdataObj);
            }

            DB_Connection.CloseDB(mySqlConnection);
            return dC_UserdataList;
        }
        public static void Add(DCUserdata dC_UserdataObj)
        {
            string sqlCommand = $"INSERT INTO DC_Userdata (AuthorId, ChannelId, ServerInfoId, Abo, MinimalAbo) " +
                                $"VALUES ({dC_UserdataObj.AuthorId}, {dC_UserdataObj.ChannelId}, {dC_UserdataObj.ServerInfoId}, {dC_UserdataObj.Abo}, {dC_UserdataObj.MinimalAbo})";
            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
        public static void Change(DCUserdata dC_UserdataObj)
        {
            string sqlCommand = $"UPDATE DC_Userdata SET Abo={dC_UserdataObj.Abo}, MinimalAbo={dC_UserdataObj.MinimalAbo} WHERE AuthorId={dC_UserdataObj.AuthorId} AND ChannelId={dC_UserdataObj.ChannelId} AND ServerInfoId={dC_UserdataObj.ServerInfoId}";
            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
        public static void CreateTable()
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
                            "`DCUserdataID` MEDIUMINT NOT NULL AUTO_INCREMENT," +
                            "`AuthorId` BIGINT NOT NULL," +
                            "`ChannelId` BIGINT DEFAULT NULL," +
                            "`ServerInfoId` MEDIUMINT DEFAULT NULL," +
                            "`Abo` MEDIUMINT DEFAULT NULL," +
                            "`MinimalAbo` MEDIUMINT DEFAULT NULL," +
                            "PRIMARY KEY (DCUserdataID)," +
                            "KEY (ServerInfoId)" +
                            ") ENGINE=InnoDB DEFAULT CHARSET=latin1;";

            DB_Connection.ExecuteNonQuery(sqlCommand);
        }
    }
}