using System.Net;
using BotDLL.Model.BotCom.Discord.Main;
using MineStatLib;
using Okolni.Source.Query;

namespace BotDLL.Model.Objects;

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

   public static ServerStat CreateObj(ServerInfo serverInfoObj)
   {
      /*bool falsePositive = true;
      int falsePositiveInt = 1;*/
      ServerStat serverStatObj = new();
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
      var ip4address = Dns.GetHostAddresses(serverInfoObj.DynDnsAddress)[0];
#pragma warning restore CS8604 // Mögliches Nullverweisargument.

      /*do
      {*/
      if (serverInfoObj.Game == Objects.Game.Minecraft)
      {
         MineStat mineStatObj = new(ip4address.ToString(), serverInfoObj.Port, 1);
         serverStatObj = new ServerStat
         {
            ServerInfoId = serverInfoObj.ServerInfoId,
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
            FetchTime = DateTime.Now,
            UpTimeInPercent = serverInfoObj.UpTimeInPercent
         };
      }
      else if (serverInfoObj.Game == Objects.Game.SourceGame)
      {
         IQueryConnection queryConnection = new QueryConnection { Host = ip4address.ToString(), Port = serverInfoObj.Port };

         try
         {
            queryConnection.Connect();
            var infoResonceObj = queryConnection.GetInfo();
            serverStatObj = new ServerStat
            {
               ServerInfoId = serverInfoObj.ServerInfoId,
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
               GameID = Convert.ToUInt32(infoResonceObj.GameID),
               FetchTime = DateTime.Now,
               UpTimeInPercent = serverInfoObj.UpTimeInPercent
            };
            if (infoResonceObj.Map == null)
               infoResonceObj.Map = "Unknown";
            if (serverStatObj.Game == "")
               serverStatObj.Game = infoResonceObj.Folder.ToUpper();
         }
         catch
         {
            serverStatObj = new ServerStat
            {
               ServerInfoId = serverInfoObj.ServerInfoId,
               Name = serverInfoObj.Name,
               DynDnsAddress = serverInfoObj.DynDnsAddress,
               Address = ip4address,
               Port = serverInfoObj.Port,
               ServerUp = false,
               Game = serverInfoObj.Game.ToString(),
               FetchTime = DateTime.Now,
               UpTimeInPercent = serverInfoObj.UpTimeInPercent
            };
         }
      }

      /*    if (serverStatObj.ServerUp)
              falsePositive = false;

          if (falsePositiveInt % 3 == 0)
              falsePositive = false;
          falsePositiveInt++;

      } while (falsePositive);*/

      return serverStatObj;
   }

   public override string ToString()
   {
      return $"╠ {ServerInfoId,5} ╣╠ {Name,-10} ╣╠ {DynDnsAddress,-15} ╣╠ {Address,15}:{Port,-5} ╣╠ {Players,3}/{MaxPlayers,-5} ╣╠ {ServerUp,-5} ╣╠ {Version,12} ╣╠ {Map,-20} ╣╠ {Game,-35} ╣╠ {GameID,15} ╣╠ {UpTimeInPercent,6}% ╣╠ {FetchTime,21} ╣";

      Bot.counter++;
      if (Bot.counter % 2 == 0)
         return $"╠ {ServerInfoId,5} ╔╝ {Name,-10} ╔╝ {DynDnsAddress,-15} ╔╝ {Address,15}:{Port,-5} ╔╝ {Players,3}/{MaxPlayers,-5} ╔╝ {ServerUp,-5} ╔╝ {Version,12} ╔╝ {Map,-20} ╔╝ {Game,-35} ╔╝ {GameID,15} ╔╝ {UpTimeInPercent,6}% ╔╝ {FetchTime,21} ╣";
      return $"╠ {ServerInfoId,5} ╚╗ {Name,-10} ╚╗ {DynDnsAddress,-15} ╚╗ {Address,15}:{Port,-5} ╚╗ {Players,3}/{MaxPlayers,-5} ╚╗ {ServerUp,-5} ╚╗ {Version,12} ╚╗ {Map,-20} ╚╗ {Game,-35} ╚╗ {GameID,15} ╚╗ {UpTimeInPercent,6}% ╚╗ {FetchTime,21} ╣";
   }
}