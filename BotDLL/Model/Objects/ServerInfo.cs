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
      public ServerInfo()
      {
      }

      public ServerInfo(List<MonthStatistics> monthStatisticsList, Uri qcUri)
      {
         MonthStatisticsList = monthStatisticsList;
         QcUri = qcUri;
      }

      public ushort ServerInfoId { get; set; }
      public string? Name { get; set; }
      public string? DynDnsAddress { get; set; }
      public ushort Port { get; set; }
      public Game Game { get; set; }
      public double UpTimeInPercent { get; set; }
      public List<MonthStatistics> MonthStatisticsList { get; set; } = null!;
      public Uri QcUri { get; set; } = null!;

      public static List<ServerInfo?> ReadAll()
      {
         return DbServerInfo.ReadAll();
      }

      public static void Update(ServerInfo? serverInfoObj)
      {
         DbServerInfo.ChangeUpTime(serverInfoObj);
      }

      public static void CreateTable()
      {
         DbServerInfo.CreateTable();
      }
   }
}