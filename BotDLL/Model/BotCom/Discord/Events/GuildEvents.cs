using DisCatSharp;
using DisCatSharp.EventArgs;

namespace BotDLL.Model.BotCom.Discord.Events
{
   /// <summary>
   /// The guild events.
   /// </summary>
   internal class GuildEvents
   {
      /// <summary>
      /// Guild unavailable event.
      /// </summary>
      /// <param name="dcl">The discord client.</param>
      /// <param name="e">The eventargs.</param>
      public static Task Client_GuildUnavailable(DiscordClient dcl, GuildDeleteEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine("Attention! Guild " + e.Guild.Name + " is unavailable");
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }

      /// <summary>
      /// Guild available event.
      /// </summary>
      /// <param name="dcl">The discord client.</param>
      /// <param name="e">The eventargs.</param>
      public static Task Client_GuildAvailable(DiscordClient dcl, GuildCreateEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("Information: Guild " + e.Guild.Name + " is available (" + e.Guild.Id + ")");
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }
      /// <summary>
      /// Guild deleted event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The eventargs.</param>
      public static Task Client_GuildDeleted(DiscordClient sender, GuildDeleteEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         Console.WriteLine("Left Guild: " + e.Guild.Name);
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }

      /// <summary>
      /// Guild created event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The eventargs.</param>
      public static Task Client_GuildCreated(DiscordClient sender, GuildCreateEventArgs e)
      {
         Console.ForegroundColor = ConsoleColor.Cyan;
         Console.WriteLine("Joined Guild: " + e.Guild.Name);
         Console.WriteLine("Reloading cache");
         //sender.ReconnectAsync(true);
         Console.ForegroundColor = ConsoleColor.Gray;
         return Task.CompletedTask;
      }
   }
}
