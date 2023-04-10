using BotDLL.Model.BotCom.Discord.Main;
using DisCatSharp;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;

namespace BotDLL.Model.BotCom.Discord.Events
{
   /// <summary>
   ///    The client events.
   /// </summary>
   internal class ClientEvents
   {
      public static async Task Client_Ready(DiscordClient dcl, ReadyEventArgs e)
      {
         Console.WriteLine("Client ready");
         Console.WriteLine($"Shard {dcl.ShardId}");
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine("Loading Commands...");
         Console.ForegroundColor = ConsoleColor.Magenta;
         IReadOnlyDictionary<string, Command>? commandline = dcl.GetCommandsNext().RegisteredCommands;
         foreach (KeyValuePair<string, Command> command in commandline)
         {
            Console.WriteLine($"Command {command.Value.Name} loaded.");
         }

         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("Bot ready!");
         Console.ForegroundColor = ConsoleColor.Gray;
         DiscordActivity activity = new()
         {
            Name = Bot.Custom ? Bot.CustomState : "/help",
            ActivityType = ActivityType.Streaming
         };
         await dcl.UpdateStatusAsync(activity, Bot.Custom ? Bot.CustomStatus : UserStatus.Online);
         await Task.Delay(100);
      }

      public static async Task Client_Resumed(DiscordClient dcl, ReadyEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine("Bot resumed!");
         Console.ForegroundColor = ConsoleColor.Gray;
         await Task.Delay(100);
      }

      public static Task CNext_CommandErrored(CommandsNextExtension ex, CommandErrorEventArgs e)
      {
         if (e.Command == null)
         {
            Console.WriteLine($"{e.Exception.Message}");
         }
         else
         {
            Console.WriteLine($"{e.Command.Name}: {e.Exception.Message}");
         }

         return Task.CompletedTask;
      }

      public static async Task Client_SocketOpened(DiscordClient dcl, SocketEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine("Socket opened");
         Console.ForegroundColor = ConsoleColor.Gray;
         await Task.Delay(100);
      }

      public static Task Client_SocketErrored(DiscordClient dcl, SocketErrorEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.DarkRed;
         Console.WriteLine("Socket has an error! " + e.Exception.Message);
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }

      public static Task Client_SocketClosed(DiscordClient dcl, SocketCloseEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine("Socket closed: " + e.CloseMessage);
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }

      public static Task Client_Heartbeated(DiscordClient dcl, HeartbeatEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine("Received Heartbeat:" + e.Ping);
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }

      public static async Task Client_UnknownEvent(DiscordClient sender, UnknownEventArgs e)
      {
         Console.WriteLine("Unknown event: " + e.EventName);
         await Task.Delay(100);
      }
   }
}