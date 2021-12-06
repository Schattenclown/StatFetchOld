using MineStatLib;
using Okolni.Source.Query;
using Okolni.Source.Query.Responses;
using System.Net;

namespace BotDLL.Model.Objects
{
    public class ServerStat
    {
        public ushort Id { get; set; }
        public string? Name { get; set; }
        public string? DynDnsAddress { get; set; }
        public IPAddress? Address { get; set; }
        public ushort Port { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public bool ServerUp { get; set; }
        public string? Version { get; set; }
        public string? Map { get; set; }
        public string? Game { get; set; }
        public uint? GameID { get; set; }
        public DateTime FetchTime { get; set; }
        public static ServerStat CreateObj(ServerInfo serverInfoObj)
        {
            bool firsttry = true;
            bool secondtry = true;
            ServerStat serverStatObj = new();
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            IPAddress ip4address = Dns.GetHostAddresses(serverInfoObj.DynDnsAddress)[0];
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
            while ((firsttry || secondtry) && (!serverStatObj.ServerUp || serverStatObj.ServerUp == null))
            {
                if (serverInfoObj.Game == Objects.Game.Minecraft)
                {
                    MineStat mineStatObj = new(ip4address.ToString(), serverInfoObj.Port, 10);
                    serverStatObj = new()
                    {
                        Id = serverInfoObj.Id,
                        Name = serverInfoObj.Name,
                        DynDnsAddress = serverInfoObj.DynDnsAddress,
                        Address = ip4address,
                        Port = serverInfoObj.Port,
                        Players = mineStatObj.CurrentPlayersInt,
                        MaxPlayers = mineStatObj.MaximumPlayersInt,
                        ServerUp = mineStatObj.ServerUp,
                        Version = mineStatObj.Version,
                        Map = "Minecraft",
                        Game = "Minecraft",
                        GameID = 0,
                        FetchTime = DateTime.Now
                    };
                }
                else if (serverInfoObj.Game == Objects.Game.SourceGame)
                {
                    IQueryConnection queryConnection = new QueryConnection
                    {
                        Host = ip4address.ToString(),
                        Port = serverInfoObj.Port
                    };

                    queryConnection.Connect();
                    try
                    {
                        InfoResponse infoResonceObj = queryConnection.GetInfo(maxRetries: 10);
                        serverStatObj = new()
                        {
                            Id = serverInfoObj.Id,
                            Name = serverInfoObj.Name,
                            DynDnsAddress = serverInfoObj.DynDnsAddress,
                            Address = ip4address,
                            Port = serverInfoObj.Port,
                            Players = infoResonceObj.Players,
                            MaxPlayers = infoResonceObj.MaxPlayers,
                            ServerUp = true,
                            Version = infoResonceObj.Version,
                            Map = infoResonceObj.Map,
                            Game = infoResonceObj.Game,
                            GameID = infoResonceObj.GameID,
                            FetchTime = DateTime.Now
                        };
                        if (infoResonceObj.Map == null)
                            infoResonceObj.Map = "Unknown";
                        if (serverStatObj.Game == "")
                            serverStatObj.Game = infoResonceObj.Folder.ToUpper();
                    }
                    catch
                    {
                        serverStatObj = new()
                        {
                            Id = serverInfoObj.Id,
                            Name = serverInfoObj.Name,
                            DynDnsAddress = serverInfoObj.DynDnsAddress,
                            Address = ip4address,
                            Port = serverInfoObj.Port,
                            ServerUp = false,
                            Game = serverInfoObj.Game.ToString(),
                            FetchTime = DateTime.Now
                        };
                    }
                }
                if (firsttry)
                    firsttry = false;
                if(!firsttry && secondtry)
                    secondtry = false;
            }

            return serverStatObj;
        }
        public override string ToString()
        {
            return $"██ {Id,5} ██ {Name,-10} ██ {DynDnsAddress,-15} ██ {Address,15}:{Port,-5} ██ {Players,3}/{MaxPlayers,-5} ██ {ServerUp,-5} ██ {Version,10} ██ {Map,-15} ██ {Game,-35} ██ {GameID,15} ██ {FetchTime,21} ██";
        }
    }
}
