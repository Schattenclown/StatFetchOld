using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
   public class DCUserData
   {
      public ushort DCUserDataID { get; set; }
      public ushort ServerInfoId { get; set; }
      public ulong AuthorId { get; set; }
      public ulong ChannelId { get; set; }
      public bool Abo { get; set; }
      public bool IsMinimalAbo { get; set; }

      public static List<DCUserData> ReadAll()
      {
         return DB_DCUserdata.ReadAll();
      }

      public override string ToString()
      {
         return "";
      }

      public static void Add(DCUserData dCUserDataObj)
      {
         DB_DCUserdata.Add(dCUserDataObj);
      }

      public static void Change(DCUserData dCUserDataObj)
      {
         DB_DCUserdata.Change(dCUserDataObj);
      }

      public static void CreateTable()
      {
         DB_DCUserdata.CreateTable();
      }
   }
}