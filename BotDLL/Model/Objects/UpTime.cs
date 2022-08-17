using BotDLL.Persistence;
namespace BotDLL.Model.Objects
{
   public class UpTime
   {
      public ushort UpTimeId { get; set; }
      public ushort ServerInfoId { get; set; }
      public int Successful { get; set; }
      public int Unsuccessful { get; set; }
      public double InPercent { get; set; }
      public static List<UpTime> ReadAll()
      {
         return DB_UpTime.ReadAll();
      }
      public static void Add(UpTime upTimeObj)
      {
         DB_UpTime.Add(upTimeObj);
      }
      public static void Change(UpTime upTimeObj)
      {
         DB_UpTime.Change(upTimeObj);
      }
      public static void CreateTable()
      {
         DB_UpTime.CreateTable();
      }
   }
}
