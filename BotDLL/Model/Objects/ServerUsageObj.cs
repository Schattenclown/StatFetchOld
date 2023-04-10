namespace BotDLL.Model.Objects
{
   public class ServerUsageObj
   {
      public int ID { get; set; }
      public string? DynDnsAddress { get; set; }
      public int Port { get; set; }
      public double CPUUsage { get; set; }
      public long RAMUsage { get; set; }
      public DateTime UpdatedTimeStamp { get; set; }
   }
}