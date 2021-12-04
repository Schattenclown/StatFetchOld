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

        public ushort Id { get; set; }
        public string? Name { get; set; }
        public string? DynDnsAddress { get; set; }
        public ushort Port { get; set; }
        public Game Game { get; set; }
        public static List<ServerInfo> ReadAll()
        {
            return DB_ServerInfo.ReadAll();
        }
        public static void CreateTable_ServerInfo()
        {
            DB_ServerInfo.CreateTable_ServerInfo();
        }
    }
}
