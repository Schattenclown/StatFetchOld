using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
    public class MonthStatistics
    {
        public DateTime Date { get; set; }
        public ushort MaxPlayers { get; set; }
        public MonthStatistics()
        {

        }
        public static List<ServerInfo> ReadAll(List<ServerInfo> serverInfoList)
        {
            return DB_MonthStatistics.ReadAll(serverInfoList);
        }
        public static void Add(ServerStat serverStatObj)
        {
            DB_MonthStatistics.Add(serverStatObj);
        }
        public static void Change(ServerStat serverStatObj)
        {
            DB_MonthStatistics.Change(serverStatObj);
        }
        public static void CreateTables(List<ServerInfo> serverInfoList)
        {
            DB_MonthStatistics.CreateTables(serverInfoList);
        }
    }
}
