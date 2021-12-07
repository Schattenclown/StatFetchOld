using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
    public class DCUserdata
    {
        public ushort Id { get; set; }
        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }
        public ushort ServerInfoId { get; set; }
        public bool Abo { get; set; }
        public bool MinimalAbo { get; set; }
        public static List<DCUserdata> ReadAll()
        {
            return DB_DCUserdata.ReadAll();
        }
        public override string ToString()
        {
            return $"";
        }
        public static void Add(DCUserdata dC_UserdataObj)
        {
            DB_DCUserdata.Add(dC_UserdataObj);
        }
        public static void Change(DCUserdata dC_UserdataObj)
        {
            DB_DCUserdata.Change(dC_UserdataObj);
        }
        public static void CreateTable_Userdata()
        {
            DB_DCUserdata.CreateTable_Userdata();
        }
    }
}
