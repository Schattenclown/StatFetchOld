using System.Net;
using MineStatLib;
using Okolni.Source.Query;
using Okolni.Source.Query.Responses;

namespace BotDLL.Model.Objects
{
   public class ServerStat
   {
      public ushort ServerInfoId { get; set; }
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
      public double UpTimeInPercent { get; set; }

      public static ServerStat CreateObj(ServerInfo? serverInfoObj)
      {
         ServerStat serverStatObj = new();
         IPAddress ip4Address = Dns.GetHostAddresses(serverInfoObj?.DynDnsAddress!)[0];

         if (serverInfoObj?.Game == Objects.Game.Minecraft)
         {
            MineStat mineStatObj = new(ip4Address.ToString(), serverInfoObj.Port, 1);
            serverStatObj = new ServerStat
            {
                     ServerInfoId = serverInfoObj.ServerInfoId,
                     Name = serverInfoObj.Name,
                     DynDnsAddress = serverInfoObj.DynDnsAddress,
                     Address = ip4Address,
                     Port = serverInfoObj.Port,
                     Players = mineStatObj.CurrentPlayersInt,
                     MaxPlayers = mineStatObj.MaximumPlayersInt,
                     ServerUp = mineStatObj.ServerUp,
                     Version = mineStatObj.Version,
                     Map = "Minecraft",
                     Game = "Minecraft",
                     GameID = 0,
                     FetchTime = DateTime.Now,
                     UpTimeInPercent = serverInfoObj.UpTimeInPercent
            };
         }
         else if (serverInfoObj?.Game! == Objects.Game.SourceGame)
         {
            IQueryConnection queryConnection = new QueryConnection
            {
                     Host = ip4Address.ToString(),
                     Port = serverInfoObj.Port
            };

            try
            {
               queryConnection.Connect();
               InfoResponse? infoResonanceObj = queryConnection.GetInfo();
               serverStatObj = new ServerStat
               {
                        ServerInfoId = serverInfoObj.ServerInfoId,
                        Name = serverInfoObj.Name,
                        DynDnsAddress = serverInfoObj.DynDnsAddress,
                        Address = ip4Address,
                        Port = serverInfoObj.Port,
                        Players = infoResonanceObj.Players,
                        MaxPlayers = infoResonanceObj.MaxPlayers,
                        ServerUp = true,
                        Version = infoResonanceObj.Version,
                        Map = infoResonanceObj.Map,
                        Game = infoResonanceObj.Game,
                        GameID = Convert.ToUInt32(infoResonanceObj.GameID),
                        FetchTime = DateTime.Now,
                        UpTimeInPercent = serverInfoObj.UpTimeInPercent
               };
               infoResonanceObj.Map ??= "Unknown";

               if (serverStatObj.Game == "")
               {
                  serverStatObj.Game = infoResonanceObj.Folder.ToUpper();
               }
            }
            catch
            {
               serverStatObj = new ServerStat
               {
                        ServerInfoId = serverInfoObj.ServerInfoId,
                        Name = serverInfoObj.Name,
                        DynDnsAddress = serverInfoObj.DynDnsAddress,
                        Address = ip4Address,
                        Port = serverInfoObj.Port,
                        ServerUp = false,
                        Game = serverInfoObj.Game.ToString(),
                        FetchTime = DateTime.Now,
                        UpTimeInPercent = serverInfoObj.UpTimeInPercent
               };
            }
         }

         return serverStatObj;
      }

      public override string ToString()
      {
         return $"╠ {ServerInfoId,5} ╣╠ {Name,-10} ╣╠ {DynDnsAddress,-15} ╣╠ {Address,15}:{Port,-5} ╣╠ {Players,3}/{MaxPlayers,-5} ╣╠ {ServerUp,-5} ╣╠ {Version,12} ╣╠ {Map,-20} ╣╠ {Game,-35} ╣╠ {GameID,15} ╣╠ {UpTimeInPercent,6}% ╣╠ {FetchTime,21} ╣";
      }
   }
}