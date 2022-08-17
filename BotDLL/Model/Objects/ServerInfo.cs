using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
   public enum Game
   {
      Minecraft,
      SourceGame
   }
   public class ServerInfo
   {

      public ushort ServerInfoId { get; set; }
      public string? Name { get; set; }
      public string? DynDnsAddress { get; set; }
      public ushort Port { get; set; }
      public Game Game { get; set; }
      public double UpTimeInPercent { get; set; }
      public List<MonthStatistics> MonthStatisticsList { get; set; }
      public Uri QCUri { get; set; }
      public ServerInfo()
      {

      }
      public static List<ServerInfo> ReadAll()
      {
         return DB_ServerInfo.ReadAll();
      }
      public static void Update(ServerInfo serverInfoObj)
      {
         DB_ServerInfo.ChangeUpTime(serverInfoObj);
      }
      public static void CreateTable()
      {
         DB_ServerInfo.CreateTable();
      }
   }
}
