using BotDLL.Persistence;

namespace BotDLL.Model.Objects
{
    public class DCUserdata
    {
        public ushort DCUserdataID { get; set; }
        public ushort ServerInfoId { get; set; }
        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }
        public bool Abo { get; set; }
        public bool IsMinimalAbo { get; set; }
        public DCUserdata()
        {

        }
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
        public static void CreateTable()
        {
            DB_DCUserdata.CreateTable();
        }
    }
}
