using System.Data.SqlClient;
using BotDLL.Model.BotCom.Discord.Main;
using BotDLL.Model.Objects;

namespace BotDLL.Persistence.MSSQL
{
   internal class DataBaseConnection
   {
      public static List<ServerUsageObj> ReadAll()
      {
         const string sqlCommand = "SELECT * FROM dbo.ServerUsage";

         List<ServerUsageObj> serverUsages = new();

         SqlConnection connection = new(Bot.Connections.MSSQLConnectionString);

         connection.Open();
         using SqlCommand command = new(sqlCommand, connection);
         using SqlDataReader reader = command.ExecuteReader();

         while (reader.Read())
         {
            ServerUsageObj serverUsageObj = new()
            {
                     ID = reader.GetInt32(reader.GetOrdinal("ID")),
                     DynDnsAddress = reader.GetString(reader.GetOrdinal("DynDnsAddress")),
                     Port = reader.GetInt32(reader.GetOrdinal("Port")),
                     CPUUsage = reader.GetDouble(reader.GetOrdinal("CPUUsage")),
                     RAMUsage = reader.GetInt32(reader.GetOrdinal("RAMUsage")),
                     UpdatedTimeStamp = reader.GetDateTime(reader.GetOrdinal("UpdatedTimeStamp"))
            };
            serverUsages.Add(serverUsageObj);
         }

         connection.Close();
         return serverUsages;
      }

      public static ServerUsageObj Read(int port)
      {
         string sqlCommand = $"SELECT * FROM dbo.ServerUsage where Port = {port}";

         SqlConnection connection = new(Bot.Connections.MSSQLConnectionString);

         connection.Open();
         using SqlCommand command = new(sqlCommand, connection);
         using SqlDataReader reader = command.ExecuteReader();
         ServerUsageObj serverUsageObj = new();
         while (reader.Read())
         {
            serverUsageObj = new ServerUsageObj
            {
                     ID = reader.GetInt32(reader.GetOrdinal("ID")),
                     DynDnsAddress = reader.GetString(reader.GetOrdinal("DynDnsAddress")),
                     Port = reader.GetInt32(reader.GetOrdinal("Port")),
                     CPUUsage = reader.GetDouble(reader.GetOrdinal("CPUUsage")),
                     RAMUsage = reader.GetInt32(reader.GetOrdinal("RAMUsage")),
                     UpdatedTimeStamp = reader.GetDateTime(reader.GetOrdinal("UpdatedTimeStamp"))
            };
         }

         return serverUsageObj;
      }
   }
}