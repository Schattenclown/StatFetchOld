using System.Reflection;
using BotDLL.Model.Objects;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.EventArgs;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using SchattenclownBot.Model.HelpClasses;

namespace BotDLL.Model.BotCom.Discord.Main;

public class Bot : IDisposable
{
#if DEBUG
   public const string Prefix = "%";
#else
   public const string Prefix = "%";
#endif
   public static readonly Connections Connections = Connections.GetConnections();
   public static CancellationTokenSource ShutdownRequest;
   public static DiscordClient DiscordClient;
   public static DiscordGuild EmojiDiscordGuild;
   public static DiscordChannel DebugDiscordChannel;
   public static ApplicationCommandsExtension AppCommands;
   public InteractivityExtension Extension { get; private set; }
   private CommandsNextExtension _commandsNextExtension;
   private static string _token = "";
   public static UserStatus CustomStatus = UserStatus.Online;
   public static bool Custom = false;
   public static string CustomState = "/help";
#if DEBUG
   public const string isDevBot = "";
#else
   public const string isDevBot = "";
#endif

   /// <summary>
   ///    Initializes a new instance of the <see cref="Bot" /> class.
   /// </summary>
   public Bot()
   {
      _token = Connections.DiscordBotKey;
#if DEBUG
      _token = Connections.DiscordBotDebug;
#endif
      ShutdownRequest = new CancellationTokenSource();

#if DEBUG
      const LogLevel logLevel = LogLevel.Debug;
#else
      const LogLevel logLevel = LogLevel.Debug;
#endif
      DiscordConfiguration discordConfiguration = new()
      {
         Token = _token,
         TokenType = TokenType.Bot,
         AutoReconnect = true,
         MessageCacheSize = 4096,
         MinimumLogLevel = logLevel,
         ShardCount = 1,
         ShardId = 0,
         Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.GuildPresences,
         MobileStatus = false,
         UseCanary = true,
         UsePtb = false,
         AutoRefreshChannelCache = false,
         HttpTimeout = TimeSpan.FromSeconds(60),
         ReconnectIndefinitely = true
      };

      DiscordClient = new DiscordClient(discordConfiguration);

      AppCommands = DiscordClient.UseApplicationCommands(new ApplicationCommandsConfiguration { EnableDefaultHelp = true, DebugStartup = true, ManualOverride = true, CheckAllGuilds = true });

      _commandsNextExtension = DiscordClient.UseCommandsNext(new CommandsNextConfiguration
      {
         StringPrefixes = new List<string> { Prefix },
         CaseSensitive = true,
         EnableMentionPrefix = true,
         IgnoreExtraArguments = true,
         DefaultHelpChecks = null,
         EnableDefaultHelp = true,
         EnableDms = true
      });

      Extension = DiscordClient.UseInteractivity(new InteractivityConfiguration { PaginationBehaviour = PaginationBehaviour.WrapAround, PaginationDeletion = PaginationDeletion.DeleteMessage, PollBehaviour = PollBehaviour.DeleteEmojis, ButtonBehavior = ButtonPaginationBehavior.Disable });

      RegisterEventListener(DiscordClient, AppCommands, _commandsNextExtension);
      RegisterCommands(_commandsNextExtension, AppCommands);
   }

   /// <summary>
   ///    Disposes the Bot.
   /// </summary>
   public void Dispose()
   {
      DiscordClient.Dispose();
      Extension = null;
      _commandsNextExtension = null;
      DiscordClient = null;
      AppCommands = null;
      Environment.Exit(0);
   }

   /// <summary>
   ///    Starts the Bot.
   /// </summary>
   public async Task RunAsync()
   {
      await DiscordClient.ConnectAsync();

      var levelSystemVirgin = true;
      do
      {
         if (DiscordClient.Guilds.ToList().Count != 0)
         {
            EmojiDiscordGuild = DiscordClient.Guilds.Values.FirstOrDefault(x => x.Id == 881868642600505354);
            levelSystemVirgin = false;
         }

         await Task.Delay(1000);
      } while (levelSystemVirgin);

#if RELEASE
      DebugDiscordChannel = await DiscordClient.GetChannelAsync(1042762701329412146);
#elif DEBUG
      DebugDiscordChannel = await DiscordClient.GetChannelAsync(881876137297477642);
#endif

      while (!ShutdownRequest.IsCancellationRequested) await Task.Delay(2000);

      await DiscordClient.UpdateStatusAsync(null, UserStatus.Offline);
      await DiscordClient.DisconnectAsync();
      await Task.Delay(2500);
      Dispose();
   }

   public static async void DCChange(ServerStat serverStatObj, string whatchanged, bool isminimal)
   {
      try
      {
         var dC_UserdataList = DCUserdata.ReadAll();

         List<ulong>? differentchannel = new();
         var once = false;

         DiscordEmbedBuilder discordEmbedBuilder = new() { Color = new DiscordColor(255, 0, 255) };
         discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
         discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
         discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);

         foreach (var dC_UserdataItem in dC_UserdataList)
            if (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId)
            {
               if (!once)
               {
                  if (whatchanged == "player")
                  {
                     discordEmbedBuilder.Title = $"Player count changed for {serverStatObj.Name}!";
                     discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true));
                  }
                  else if (whatchanged == "status")
                  {
                     if (serverStatObj.ServerUp)
                     {
                        discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Online", true));
                        discordEmbedBuilder.Color = DiscordColor.Green;
                     }
                     else
                     {
                        discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Offline", true));
                        discordEmbedBuilder.Color = DiscordColor.Red;
                     }

                     discordEmbedBuilder.Title = $"Status changed for {serverStatObj.Name}!";
                  }
                  else if (whatchanged == "version")
                  {
                     discordEmbedBuilder.Title = $"Version changed for {serverStatObj.Name}!";
                     discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatObj.Version}", true));
                  }

                  discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true));

                  once = true;
               }

               if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataItem.ChannelId)))
                  differentchannel.Add(Convert.ToUInt64(dC_UserdataItem.ChannelId));
            }

         var mentions = "";

         foreach (var channelIdItem in differentchannel)
         {
            foreach (var dC_UserdataItem in dC_UserdataList)
               if (!mentions.Contains(dC_UserdataItem.AuthorId.ToString()) && channelIdItem == Convert.ToUInt64(dC_UserdataItem.ChannelId))
               {
                  if (dC_UserdataItem.Abo && !dC_UserdataItem.IsMinimalAbo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && !mentions.Contains(dC_UserdataItem.AuthorId.ToString()))
                     mentions += $"<@{dC_UserdataItem.AuthorId}> \n";
                  else if (dC_UserdataItem.IsMinimalAbo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && isminimal)
                     mentions += $"<@{dC_UserdataItem.AuthorId}> \n";
                  else if ((dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && whatchanged == "version") || (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && whatchanged == "status"))
                     mentions += $"<@{dC_UserdataItem.AuthorId}> \n";
               }

            if (discordEmbedBuilder.Fields.Count == 3)
               discordEmbedBuilder.RemoveFieldAt(2);
            if (mentions != "" && mentions != " ")
               discordEmbedBuilder.AddField(new DiscordEmbedField("Mentions", mentions));

            var channel = await DiscordClient.GetChannelAsync(channelIdItem);
            if (channel != null && mentions != "")
               await channel.SendMessageAsync(discordEmbedBuilder.Build());

            mentions = "";
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }


   #region RegisterForControlPanel Commands & Events

   /// <summary>
   ///    Registers the event listener.
   /// </summary>
   /// <param name="discordClient">The discordClient.</param>
   /// <param name="applicationCommandsExtension"></param>
   /// <param name="commandsNextExtension">The commandsNext extension.</param>
   private static void RegisterEventListener(DiscordClient discordClient, ApplicationCommandsExtension applicationCommandsExtension, CommandsNextExtension commandsNextExtension)
   {
      /* DiscordClient Basic Events */
      discordClient.SocketOpened += Client_SocketOpened;
      discordClient.SocketClosed += Client_SocketClosed;
      discordClient.SocketErrored += Client_SocketError;
      discordClient.Heartbeated += Client_Heartbeat;
      discordClient.Ready += Client_Ready;
      discordClient.Resumed += Client_Resumed;
      /* DiscordClient Events */
      //discordClient.GuildUnavailable += Client_GuildUnavailable;
      //discordClient.GuildAvailable += Client_GuildAvailable;

      /* CommandsNext Error */
      commandsNextExtension.CommandErrored += CNext_CommandError;

      /* Main Infos */
      discordClient.ApplicationCommandCreated += Discord_ApplicationCommandCreated;
      discordClient.ApplicationCommandDeleted += Discord_ApplicationCommandDeleted;
      discordClient.ApplicationCommandUpdated += Discord_ApplicationCommandUpdated;
      applicationCommandsExtension.SlashCommandErrored += Slash_SlashCommandError;
      applicationCommandsExtension.SlashCommandExecuted += Slash_SlashCommandExecuted;

      //Custom Events
   }

   /// <summary>
   ///    Registers the commands.
   /// </summary>
   /// <param name="commandsNextExtension">The commandsnext extension.</param>
   /// <param name="applicationCommandsExtension">The appcommands extension.</param>
   private static void RegisterCommands(CommandsNextExtension commandsNextExtension, ApplicationCommandsExtension applicationCommandsExtension)
   {
      applicationCommandsExtension.RegisterGlobalCommands<AppCommands.Main>();
   }

   private static Task Client_Ready(DiscordClient discordClient, ReadyEventArgs readyEventArgs)
   {
      CwLogger.Write($"Starting with Prefix {Prefix} :3", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      CwLogger.Write($"Starting {DiscordClient.CurrentUser.Username}", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      CwLogger.Write("DiscordClient ready!", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      CwLogger.Write($"Shard {discordClient.ShardId}", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      CwLogger.Write("Loading Commands...", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);

      IReadOnlyDictionary<string, Command> registeredCommands = discordClient.GetCommandsNext().RegisteredCommands;
      foreach (var command in registeredCommands) CwLogger.Write($"Command {command.Value.Name} loaded.", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);

      DiscordActivity discordActivity = new() { Name = Custom ? CustomState : "/help", ActivityType = ActivityType.Competing };
      discordClient.UpdateStatusAsync(discordActivity, Custom ? CustomStatus : UserStatus.Online);
      CwLogger.Write("Bot ready!", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Green);
      return Task.CompletedTask;
   }

   private static Task Client_Resumed(DiscordClient discordClient, ReadyEventArgs readyEventArgs)
   {
      CwLogger.Write("Bot resumed!", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Green);
      return Task.CompletedTask;
   }

   private static Task Discord_ApplicationCommandUpdated(DiscordClient discordClient, ApplicationCommandEventArgs applicationCommandEventArgs)
   {
      discordClient.Logger.LogInformation($"Shard {discordClient.ShardId} sent application command updated: {applicationCommandEventArgs.Command.Name}: {applicationCommandEventArgs.Command.Id} for {applicationCommandEventArgs.Command.ApplicationId}");
      return Task.CompletedTask;
   }

   private static Task Discord_ApplicationCommandDeleted(DiscordClient discordClient, ApplicationCommandEventArgs applicationCommandEventArgs)
   {
      discordClient.Logger.LogInformation($"Shard {discordClient.ShardId} sent application command deleted: {applicationCommandEventArgs.Command.Name}: {applicationCommandEventArgs.Command.Id} for {applicationCommandEventArgs.Command.ApplicationId}");
      return Task.CompletedTask;
   }

   private static Task Discord_ApplicationCommandCreated(DiscordClient discordClient, ApplicationCommandEventArgs applicationCommandEventArgs)
   {
      discordClient.Logger.LogInformation($"Shard {discordClient.ShardId} sent application command created: {applicationCommandEventArgs.Command.Name}: {applicationCommandEventArgs.Command.Id} for {applicationCommandEventArgs.Command.ApplicationId}");
      return Task.CompletedTask;
   }

   private static Task Slash_SlashCommandExecuted(ApplicationCommandsExtension applicationCommandsExtension, SlashCommandExecutedEventArgs slashCommandExecutedEventArgs)
   {
      CwLogger.Write($"Main/Info: {slashCommandExecutedEventArgs.Context.CommandName}", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      return Task.CompletedTask;
   }

   private static Task Slash_SlashCommandError(ApplicationCommandsExtension applicationCommandsExtension, SlashCommandErrorEventArgs slashCommandErrorEventArgs)
   {
      CwLogger.Write($"Main/Error: {slashCommandErrorEventArgs.Exception.Message} | CN: {slashCommandErrorEventArgs.Context.CommandName} | IID: {slashCommandErrorEventArgs.Context.InteractionId}", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Red);
      return Task.CompletedTask;
   }

   private static Task CNext_CommandError(CommandsNextExtension commandsNextExtension, CommandErrorEventArgs commandErrorEventArgs)
   {
      CwLogger.Write(commandErrorEventArgs.Command == null ? $"{commandErrorEventArgs.Exception.Message}" : $"{commandErrorEventArgs.Command.Name}: {commandErrorEventArgs.Exception.Message}", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Red);
      return Task.CompletedTask;
   }

   private static Task Client_SocketOpened(DiscordClient discordClient, SocketEventArgs socketEventArgs)
   {
      CwLogger.Write("Socket opened", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Magenta);
      return Task.CompletedTask;
   }

   private static Task Client_SocketError(DiscordClient discordClient, SocketErrorEventArgs socketErrorEventArgs)
   {
      CwLogger.Write("Socket has an error! " + socketErrorEventArgs.Exception.Message, MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Red);
      return Task.CompletedTask;
   }

   private static Task Client_SocketClosed(DiscordClient discordClient, SocketCloseEventArgs socketCloseEventArgs)
   {
      CwLogger.Write("Socket closed: " + socketCloseEventArgs.CloseMessage, MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.Red);
      return Task.CompletedTask;
   }

   private static Task Client_Heartbeat(DiscordClient discordClient, HeartbeatEventArgs heartbeatEventArgs)
   {
      CwLogger.Write("Received Heartbeat:" + heartbeatEventArgs.Ping, MethodBase.GetCurrentMethod()?.DeclaringType?.Name, ConsoleColor.DarkRed);
      return Task.CompletedTask;
   }

   #endregion
}