using System.Reflection;
using BotDLL.Model.HelpClasses;
using BotDLL.Model.Objects;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.EventArgs;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

namespace BotDLL.Model.BotCom.Discord.Main
{
   public class Bot : IDisposable
   {
      public const string Prefix = "%";
      public static readonly Connections Connections = Connections.GetConnections();
      private static CancellationTokenSource? _shutdownRequest;
      public static DiscordClient? Client { get; set; }
      public static DiscordGuild? EmojiDiscordGuild { get; private set; }
      public static DiscordChannel? DebugDiscordChannel { get; private set; }
      private static ApplicationCommandsExtension? _appCommands;
      public InteractivityExtension? Extension { get; private set; }
      private CommandsNextExtension? _commandsNextExtension;
      public static UserStatus CustomStatus { get; } = UserStatus.Online;
      public static bool Custom { get; } = false;
      public static string CustomState { get; } = "/help";

#if RELEASE
      public const string IsDevBot = "";
#elif DEBUG
      public const string IsDevBot = "";
#endif

      /// <summary>
      ///    Initializes a new instance of the <see cref="Bot" /> class.
      /// </summary>
      public Bot()
      {
#if RELEASE
         string token = Connections.DiscordBotKey;
#elif DEBUG
         string token = Connections.DiscordBotDebug;
#endif

         _shutdownRequest = new CancellationTokenSource();

#if RELEASE
         const LogLevel logLevel = LogLevel.None;
#elif DEBUG
         const LogLevel logLevel = LogLevel.Debug;
#endif

         DiscordConfiguration discordConfiguration = new()
         {
            Token = token,
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

         Client = new DiscordClient(discordConfiguration);

         _appCommands = Client.UseApplicationCommands(new ApplicationCommandsConfiguration
         {
            EnableDefaultHelp = true,
            DebugStartup = true,
            ManualOverride = true,
            CheckAllGuilds = true
         });

         _commandsNextExtension = Client.UseCommandsNext(new CommandsNextConfiguration
         {
            StringPrefixes = new List<string>
            {
               Prefix
            },
            CaseSensitive = true,
            EnableMentionPrefix = true,
            IgnoreExtraArguments = true,
            DefaultHelpChecks = null,
            EnableDefaultHelp = true,
            EnableDms = true
         });

         Extension = Client.UseInteractivity(new InteractivityConfiguration
         {
            PaginationBehaviour = PaginationBehaviour.WrapAround,
            PaginationDeletion = PaginationDeletion.DeleteMessage,
            PollBehaviour = PollBehaviour.DeleteEmojis,
            ButtonBehavior = ButtonPaginationBehavior.Disable
         });

         RegisterEventListener(Client, _appCommands, _commandsNextExtension);
         RegisterCommands(_appCommands);
      }

      /// <summary>
      ///    Disposes the Bot.
      /// </summary>
      public void Dispose()
      {
         Client?.Dispose();
         Extension = null;
         _commandsNextExtension = null;
         Client = null;
         _appCommands = null;
         Environment.Exit(0);
      }

      /// <summary>
      ///    Starts the Bot.
      /// </summary>
      public async Task RunAsync()
      {
         await Client?.ConnectAsync()!;

         bool levelSystemVirgin = true;
         do
         {
            if (Client.Guilds.ToList().Count != 0)
            {
               EmojiDiscordGuild = Client.Guilds.Values.FirstOrDefault(x => x.Id == 881868642600505354);
               levelSystemVirgin = false;
            }

            await Task.Delay(1000);
         } while (levelSystemVirgin);

#if RELEASE
         DebugDiscordChannel = await Client.GetChannelAsync(1042762701329412146);
#elif DEBUG
         DebugDiscordChannel = await Client.GetChannelAsync(881876137297477642);
#endif

         while (_shutdownRequest is
                {
                   IsCancellationRequested: false
                })
         {
            await Task.Delay(2000);
         }

         await Client.UpdateStatusAsync(null, UserStatus.Offline);
         await Client.DisconnectAsync();
         await Task.Delay(2500);
         Dispose();
      }

      public static async void DC_Change(ServerStat serverStatObj, string whatChanged, bool isMinimal)
      {
         try
         {
            List<DCUserData> dCUserDataList = DCUserData.ReadAll();

            List<ulong> differentChannel = new();
            bool once = false;

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
               Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);

            foreach (DCUserData dCUserDataItem in dCUserDataList.Where(dCUserDataItem => dCUserDataItem.Abo && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId))
            {
               if (!once)
               {
                  switch (whatChanged)
                  {
                     case "player":
                        discordEmbedBuilder.Title = $"Player count changed for {serverStatObj.Name}!";
                        discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true));
                        break;
                     case "status":
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
                        break;
                     }
                     case "version":
                        discordEmbedBuilder.Title = $"Version changed for {serverStatObj.Name}!";
                        discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatObj.Version}", true));
                        break;
                  }

                  discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true));

                  once = true;
               }

               if (!differentChannel.Contains(Convert.ToUInt64(dCUserDataItem.ChannelId)))
               {
                  differentChannel.Add(Convert.ToUInt64(dCUserDataItem.ChannelId));
               }
            }

            string mentions = "";

            foreach (ulong channelIdItem in differentChannel)
            {
               foreach (DCUserData dCUserDataItem in dCUserDataList.Where(dCUserDataItem => !mentions.Contains(dCUserDataItem.AuthorId.ToString()) && channelIdItem == Convert.ToUInt64(dCUserDataItem.ChannelId)))
               {
                  if (dCUserDataItem.Abo && !dCUserDataItem.IsMinimalAbo && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId && !mentions.Contains(dCUserDataItem.AuthorId.ToString()))
                  {
                     mentions += $"<@{dCUserDataItem.AuthorId}> \n";
                  }
                  else if (dCUserDataItem.IsMinimalAbo && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId && isMinimal)
                  {
                     mentions += $"<@{dCUserDataItem.AuthorId}> \n";
                  }
                  else if ((dCUserDataItem.Abo && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId && whatChanged == "version") || (dCUserDataItem.Abo && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId && whatChanged == "status"))
                  {
                     mentions += $"<@{dCUserDataItem.AuthorId}> \n";
                  }
               }

               if (discordEmbedBuilder.Fields.Count == 3)
               {
                  discordEmbedBuilder.RemoveFieldAt(2);
               }

               if (mentions != "" && mentions != " ")
               {
                  discordEmbedBuilder.AddField(new DiscordEmbedField("Mentions", mentions));
               }

               DiscordChannel? channel = await Client?.GetChannelAsync(channelIdItem)!;
               if (channel != null && mentions != "")
               {
                  await channel.SendMessageAsync(discordEmbedBuilder.Build());
               }

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
      private static void RegisterEventListener(DiscordClient? discordClient, ApplicationCommandsExtension? applicationCommandsExtension, CommandsNextExtension? commandsNextExtension)
      {
         /* DiscordClient Basic Events */
         if (discordClient != null)
         {
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
            if (commandsNextExtension != null)
            {
               commandsNextExtension.CommandErrored += CNext_CommandError;
            }

            /* Main Infos */
            discordClient.ApplicationCommandCreated += Discord_ApplicationCommandCreated;
            discordClient.ApplicationCommandDeleted += Discord_ApplicationCommandDeleted;
            discordClient.ApplicationCommandUpdated += Discord_ApplicationCommandUpdated;
         }

         if (applicationCommandsExtension != null)
         {
            applicationCommandsExtension.SlashCommandErrored += Slash_SlashCommandError;
            applicationCommandsExtension.SlashCommandExecuted += Slash_SlashCommandExecuted;
         }

         //Custom Events
      }

      /// <summary>
      ///    Registers the commands.
      /// </summary>
      /// <param name="applicationCommandsExtension">The appcommands extension.</param>
      private static void RegisterCommands(ApplicationCommandsExtension? applicationCommandsExtension)
      {
         applicationCommandsExtension?.RegisterGlobalCommands<AppCommands.Main>();
      }

      private static Task Client_Ready(DiscordClient discordClient, ReadyEventArgs readyEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName == null)
         {
            return Task.CompletedTask;
         }

         CwLogger.Write($"Starting with Prefix {Prefix} :3", declaringTypeName, ConsoleColor.Magenta);
         CwLogger.Write($"Starting {Client?.CurrentUser.Username}", declaringTypeName, ConsoleColor.Magenta);
         CwLogger.Write("DiscordClient ready!", declaringTypeName, ConsoleColor.Magenta);
         CwLogger.Write($"Shard {discordClient.ShardId}", declaringTypeName, ConsoleColor.Magenta);
         CwLogger.Write("Loading Commands...", declaringTypeName, ConsoleColor.Magenta);

         IReadOnlyDictionary<string, Command> registeredCommands = discordClient.GetCommandsNext().RegisteredCommands;
         foreach (KeyValuePair<string, Command> command in registeredCommands)
         {
            CwLogger.Write($"Command {command.Value.Name} loaded.", declaringTypeName, ConsoleColor.Magenta);
         }

         DiscordActivity discordActivity = new()
         {
            Name = Custom ? CustomState : "/help",
            ActivityType = ActivityType.Competing
         };
         discordClient.UpdateStatusAsync(discordActivity, Custom ? CustomStatus : UserStatus.Online);
         CwLogger.Write("Bot ready!", declaringTypeName, ConsoleColor.Green);

         return Task.CompletedTask;
      }

      private static Task Client_Resumed(DiscordClient discordClient, ReadyEventArgs readyEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write("Bot resumed!", declaringTypeName, ConsoleColor.Green);
         }

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
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write($"Main/Info: {slashCommandExecutedEventArgs.Context.CommandName}", declaringTypeName, ConsoleColor.Magenta);
         }

         return Task.CompletedTask;
      }

      private static Task Slash_SlashCommandError(ApplicationCommandsExtension applicationCommandsExtension, SlashCommandErrorEventArgs slashCommandErrorEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write($"Main/Error: {slashCommandErrorEventArgs.Exception.Message} | CN: {slashCommandErrorEventArgs.Context.CommandName} | IID: {slashCommandErrorEventArgs.Context.InteractionId}", declaringTypeName, ConsoleColor.Red);
         }

         return Task.CompletedTask;
      }

      private static Task CNext_CommandError(CommandsNextExtension commandsNextExtension, CommandErrorEventArgs commandErrorEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write(commandErrorEventArgs.Command == null ? $"{commandErrorEventArgs.Exception.Message}" : $"{commandErrorEventArgs.Command.Name}: {commandErrorEventArgs.Exception.Message}", declaringTypeName, ConsoleColor.Red);
         }

         return Task.CompletedTask;
      }

      private static Task Client_SocketOpened(DiscordClient discordClient, SocketEventArgs socketEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write("Socket opened", declaringTypeName, ConsoleColor.Magenta);
         }

         return Task.CompletedTask;
      }

      private static Task Client_SocketError(DiscordClient discordClient, SocketErrorEventArgs socketErrorEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write("Socket has an error! " + socketErrorEventArgs.Exception.Message, declaringTypeName, ConsoleColor.Red);
         }

         return Task.CompletedTask;
      }

      private static Task Client_SocketClosed(DiscordClient discordClient, SocketCloseEventArgs socketCloseEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write("Socket closed: " + socketCloseEventArgs.CloseMessage, declaringTypeName, ConsoleColor.Red);
         }

         return Task.CompletedTask;
      }

      private static Task Client_Heartbeat(DiscordClient discordClient, HeartbeatEventArgs heartbeatEventArgs)
      {
         string? declaringTypeName = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
         if (declaringTypeName != null)
         {
            CwLogger.Write("Received Heartbeat:" + heartbeatEventArgs.Ping, declaringTypeName, ConsoleColor.DarkRed);
         }

         return Task.CompletedTask;
      }

      #endregion
   }
}