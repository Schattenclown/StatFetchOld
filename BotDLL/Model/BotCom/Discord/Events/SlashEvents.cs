using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.EventArgs;
using DisCatSharp.EventArgs;
using Microsoft.Extensions.Logging;

namespace BotDLL.Model.BotCom.Discord.Events
{
   /// <summary>
   /// The slash events.
   /// </summary>
   internal class ApplicationCommandsEvents
   {
      /// <summary>
      /// Component interaction created event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
      {
         await Task.FromResult(true);
      }

      /// <summary>
      /// Application command updated event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Discord_ApplicationCommandUpdated(DiscordClient sender, ApplicationCommandEventArgs e)
      {
         sender.Logger.LogInformation($"Shard {sender.ShardId} sent application command updated: {e.Command.Name}: {e.Command.Id} for {e.Command.ApplicationId}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Application command deleted event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Discord_ApplicationCommandDeleted(DiscordClient sender, ApplicationCommandEventArgs e)
      {
         sender.Logger.LogInformation($"Shard {sender.ShardId} sent application command deleted: {e.Command.Name}: {e.Command.Id} for {e.Command.ApplicationId}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Application command created event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Discord_ApplicationCommandCreated(DiscordClient sender, ApplicationCommandEventArgs e)
      {
         sender.Logger.LogInformation($"Shard {sender.ShardId} sent application command created: {e.Command.Name}: {e.Command.Id} for {e.Command.ApplicationId}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Slash command errored event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Ac_SlashCommandExecuted(ApplicationCommandsExtension sender, SlashCommandExecutedEventArgs e)
      {
         Console.WriteLine($"Slash/Info: {e.Context.CommandName}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Slashcommand errored event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Ac_SlashCommandErrored(ApplicationCommandsExtension sender, SlashCommandErrorEventArgs e)
      {
         Console.WriteLine($"Slash/Error: {e.Exception.Message} | CN: {e.Context.CommandName} | IID: {e.Context.InteractionId}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Context menu executed event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Ac_ContextMenuExecuted(ApplicationCommandsExtension sender, ContextMenuExecutedEventArgs e)
      {
         Console.WriteLine($"Slash/Info: {e.Context.CommandName}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Context menu errored event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Ac_ContextMenuErrored(ApplicationCommandsExtension sender, ContextMenuErrorEventArgs e)
      {
         Console.WriteLine($"Slash/Error: {e.Exception.Message} | CN: {e.Context.CommandName} | IID: {e.Context.InteractionId}");
         return Task.CompletedTask;
      }

      /// <summary>
      /// Application command permissions updated event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The event args.</param>
      public static Task Client_ApplicationCommandPermissionsUpdated(DiscordClient sender, ApplicationCommandPermissionsUpdateEventArgs e)
      {
         Console.WriteLine($"Application command permission '{e.Command.Name}' in guild '{e.Guild.Name}' got updated. New permissions:");
         foreach (DisCatSharp.Entities.DiscordApplicationCommandPermission? perm in e.Permissions)
         {
            Console.WriteLine($"Permission with type {perm.Type} got updated for {perm.Id}. Command {e.Command.Name} is {(perm.Permission ? "allowed" : "denied")}.");
         }
         return Task.CompletedTask;
      }
   }
}
