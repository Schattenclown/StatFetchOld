using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
    public class DC_Userdata
    {
        public ushort Id { get; set; }
        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }
        public ushort ServerInfoId { get; set; }
        public bool Abo { get; set; }
        public bool MinimalAbo { get; set; }
        public static List<DC_Userdata> ReadAll()
        {
            return DB_DC_Userdata.ReadAll();
        }
        public override string ToString()
        {
            return $"";
        }
        public static void Add(DC_Userdata dC_UserdataObj)
        {
            DB_DC_Userdata.Add(dC_UserdataObj);
        }
        public static void Change(DC_Userdata dC_UserdataObj)
        {
            DB_DC_Userdata.Change(dC_UserdataObj);
        }
        public static void CreateTable_Userdata()
        {
            DB_DC_Userdata.CreateTable_Userdata();
        }
    }
}
