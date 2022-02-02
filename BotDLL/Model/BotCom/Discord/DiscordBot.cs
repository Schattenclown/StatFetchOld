using BotDLL.Model.Objects;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.EventHandling;
using DisCatSharp.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using System.Text;
using static BotDLL.Model.BotCom.Discord.Events.ApplicationCommandsEvents;
using static BotDLL.Model.BotCom.Discord.Events.ClientEvents;
using static BotDLL.Model.BotCom.Discord.Events.GuildEvents;

namespace BotDLL.Model.BotCom.Discord
{
    #region MultiDict
    /// <summary>
    /// Multidictionary
    /// </summary>
    /// <typeparam name="TKey">Key</typeparam>
    /// <typeparam name="TValue">Value</typeparam>
    public class MultiDict<TKey, TValue>
    {
#pragma warning disable CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.
        private readonly Dictionary<TKey, List<TValue>> _data = new Dictionary<TKey, List<TValue>>();
#pragma warning restore CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.

        /// <summary>
        /// Adds a <see cref="List{T}"/> to an <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="k">Key</param>
        /// <param name="v">Value</param>
        public void Add(TKey k, TValue v)
        {
            if (_data.ContainsKey(k))
                _data[k].Add(v);
            else
                _data.Add(k, new List<TValue>() { v });
        }

        /// <summary>
        /// Deletes a <see cref="List{T}"/> from  an <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="k">Key</param>
        /// <param name="v">Value</param>
        public void Del(TKey k, TValue v)
        {
            if (_data.ContainsKey(k))
                _data[k].Remove(v);
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <returns>Dictionary</returns>
#pragma warning disable CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.
        public Dictionary<TKey, List<TValue>> Get()
#pragma warning restore CS8714 // Der Typ kann nicht als Typparameter im generischen Typ oder in der generischen Methode verwendet werden. Die NULL-Zulässigkeit des Typarguments entspricht nicht der notnull-Einschränkung.
        {
            return _data;
        }
    }
    #endregion

    /// <summary>
    /// The discord bot.
    /// </summary>
    public class DiscordBot : IDisposable
    {
        /// <summary>
        /// The db.
        /// </summary>
        public static string token = "";
        public static int virgin = 0;
        /// <summary>
        /// Gets the client.
        /// </summary>
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public static DiscordClient Client { get; internal set; }
        /// <summary>
        /// Gets the application commands extension.
        /// </summary>
        public static ApplicationCommandsExtension ApplicationCommands { get; internal set; }
        /// <summary>
        /// Gets the commands next extension.
        /// </summary>
        public static CommandsNextExtension CNext { get; internal set; }
        /// <summary>
        /// Gets the interactivity extension.
        /// </summary>
        public static InteractivityExtension INext { get; internal set; }
        public static CancellationTokenSource ShutdownRequest;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        public static readonly ulong testguild = 881868642600505354;
        public static string prefix = "sf/";
        public static bool custom = false;
        public static UserStatus customstatus = UserStatus.Streaming;
        public static string customstate = $"/help";

        /// <summary>
        /// Binarie to text.
        /// </summary>
        /// <param name="data">The binary data.</param>
        public static string BinaryToText(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordBot"/> class.
        /// </summary>
        public DiscordBot()
        {
            if (virgin == 0)
            {
                Connections connections = Connections.GetConnections();
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
                token = connections.DiscordBotKey;
#if DEBUG
                token = connections.DiscordBotDebug;
#endif
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
                virgin = 69;
            }
            ShutdownRequest = new CancellationTokenSource();

            LogLevel logLevel;
#if DEBUG
            logLevel = LogLevel.Debug;
#else
            logLevel = LogLevel.Error;
#endif
            var cfg = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MessageCacheSize = 2048,
                MinimumLogLevel = logLevel,
                ShardCount = 1,
                ShardId = 0,
                Intents = DiscordIntents.AllUnprivileged,
                MobileStatus = false,
                UseCanary = false,
                AutoRefreshChannelCache = false
            };

            Client = new DiscordClient(cfg);
            ApplicationCommands = Client.UseApplicationCommands(new ApplicationCommandsConfiguration()
            {
                EnableDefaultHelp = false,
                DebugStartup = true,
                ManualOverride = true
            });

            CNext = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { prefix },
                CaseSensitive = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                DefaultHelpChecks = null,
                EnableDefaultHelp = true,
                EnableDms = true
            });

            INext = Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2),
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
                PollBehaviour = PollBehaviour.DeleteEmojis,
                AckPaginationButtons = true,
                ButtonBehavior = ButtonPaginationBehavior.Disable,
                PaginationButtons = new PaginationButtons()
                {
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-skip-left", "First", false, new DiscordComponentEmoji("⏮️")),
                    Left = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-left", "Previous", false, new DiscordComponentEmoji("◀️")),
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, "pgb-stop", "Cancel", false, new DiscordComponentEmoji("⏹️")),
                    Right = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-right", "Next", false, new DiscordComponentEmoji("▶️")),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, "pgb-skip-right", "Last", false, new DiscordComponentEmoji("⏭️"))
                },
                ResponseMessage = "Something went wrong.",
                ResponseBehavior = InteractionResponseBehavior.Respond
            });

            RegisterEventListener(Client, ApplicationCommands, CNext);
            RegisterCommands(CNext, ApplicationCommands);

        }
        /// <summary>
        /// Disposes the bot.
        /// </summary>
        public void Dispose()
        {
            Client.Dispose();
#pragma warning disable CS8625 // Ein NULL-Literal kann nicht in einen Non-Nullable-Verweistyp konvertiert werden.
            INext = null;
            CNext = null;
            Client = null;
            ApplicationCommands = null;
#pragma warning restore CS8625 // Ein NULL-Literal kann nicht in einen Non-Nullable-Verweistyp konvertiert werden.
        }

        /// <summary>
        /// Runs the bot.
        /// </summary>
        public async Task RunAsync()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            await Client.ConnectAsync();
            Console.WriteLine($"Starting with Prefix {prefix}");
            Console.WriteLine($"Starting {Client.CurrentUser.Username}");

            /*
            while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(2000);
            }
            await Client.UpdateStatusAsync(activity: null, userStatus: UserStatus.Offline, idleSince: null);
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();
            */
        }


        #region Register Commands & Events
        /// <summary>
        /// Registers the event listener.
        /// </summary>
        /// <param name="client">The discord client.</param>
        /// <param name="ac">The application commands extension.</param>
        /// <param name="cnext">The commands next extension.</param>
        private void RegisterEventListener(DiscordClient client, ApplicationCommandsExtension ac, CommandsNextExtension cnext)
        {
            client.Ready += Client_Ready;
            client.Resumed += Client_Resumed;
            client.SocketOpened += Client_SocketOpened;
            client.SocketClosed += Client_SocketClosed;
            client.SocketErrored += Client_SocketErrored;
            client.Heartbeated += Client_Heartbeated;
            client.GuildUnavailable += Client_GuildUnavailable;
            client.GuildAvailable += Client_GuildAvailable;
            client.GuildCreated += Client_GuildCreated;
            client.GuildDeleted += Client_GuildDeleted;
            client.ApplicationCommandCreated += Discord_ApplicationCommandCreated;
            client.ApplicationCommandDeleted += Discord_ApplicationCommandDeleted;
            client.ApplicationCommandUpdated += Discord_ApplicationCommandUpdated;
            client.ComponentInteractionCreated += Client_ComponentInteractionCreated;
            client.ApplicationCommandPermissionsUpdated += Client_ApplicationCommandPermissionsUpdated;

#if DEBUG
            client.UnknownEvent += Client_UnknownEvent;
#endif
            ac.SlashCommandErrored += Ac_SlashCommandErrored;
            ac.SlashCommandExecuted += Ac_SlashCommandExecuted;
            ac.ContextMenuErrored += Ac_ContextMenuErrored;
            ac.ContextMenuExecuted += Ac_ContextMenuExecuted;
            cnext.CommandErrored += CNext_CommandErrored;
        }

        /// <summary>
        /// Registers the commands.
        /// </summary>
        /// <param name="cnext">The commands next extension.</param>
        /// <param name="ac">The application commands extensions.</param>
        private static void RegisterCommands(CommandsNextExtension cnext, ApplicationCommandsExtension ac)
        {
            cnext.RegisterCommands<Discord.DiscordCommands.Main>();
#if DEBUG
            ac.RegisterGuildCommands<DiscordCommands.Slash>(testguild, perms =>
            {
                perms.AddRole(889266812267663380, true);
            });
#else
            ac.RegisterGuildCommands<DiscordCommands.Slash>(928930967140331590);
            ac.RegisterGuildCommands<DiscordCommands.Slash>(881868642600505354);
#endif
        }
        #endregion

        public static async void DCChange(ServerStat serverStatObj, string whatchanged, bool isminimal)
        {
            List<DCUserdata> dC_UserdataList = DCUserdata.ReadAll();

            var differentchannel = new List<ulong>();
            bool once = false;

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);

            foreach (DCUserdata dC_UserdataItem in dC_UserdataList)
            {
                if (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId)
                {
                    if (!once)
                    {
                        if (whatchanged == "player")
                        {
                            discordEmbedBuilder.Title = $"Player count changed for {serverStatObj.Name}!";
                            discordEmbedBuilder.AddField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true);
                        }
                        else if (whatchanged == "status")
                        {
                            if (serverStatObj.ServerUp)
                            {
                                discordEmbedBuilder.AddField("ServerUp", $"Online", true);
                                discordEmbedBuilder.Color = DiscordColor.Green;
                            }
                            else
                            {
                                discordEmbedBuilder.AddField("ServerUp", $"Offline", true);
                                discordEmbedBuilder.Color = DiscordColor.Red;
                            }
                            discordEmbedBuilder.Title = $"Status changed for {serverStatObj.Name}!";
                        }
                        else if (whatchanged == "version")
                        {
                            discordEmbedBuilder.Title = $"Version changed for {serverStatObj.Name}!";
                            discordEmbedBuilder.AddField("Version", $"{serverStatObj.Version}", true);
                        }
                        discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true);

                        once = true;
                    }
                    if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataItem.ChannelId)))
                        differentchannel.Add(Convert.ToUInt64(dC_UserdataItem.ChannelId));
                }
            }

            string mensions = "";

            foreach (ulong channelIdItem in differentchannel)
            {
                foreach (DCUserdata dC_UserdataItem in dC_UserdataList)
                {
                    if (!mensions.Contains(dC_UserdataItem.AuthorId.ToString()) && channelIdItem == Convert.ToUInt64(dC_UserdataItem.ChannelId))
                    {
                        if (dC_UserdataItem.Abo && !dC_UserdataItem.IsMinimalAbo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && !mensions.Contains(dC_UserdataItem.AuthorId.ToString()))
                            mensions += $"<@{dC_UserdataItem.AuthorId}> \n";
                        else if (dC_UserdataItem.IsMinimalAbo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && isminimal)
                            mensions += $"<@{dC_UserdataItem.AuthorId}> \n";
                        else if (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && whatchanged == "version" ||
                                 dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId && whatchanged == "status")
                            mensions += $"<@{dC_UserdataItem.AuthorId}> \n";
                    }
                }

                if (discordEmbedBuilder.Fields.Count == 3)
                    discordEmbedBuilder.RemoveFieldAt(2);
                if (mensions != "" && mensions != " ")
                    discordEmbedBuilder.AddField("Mensions", mensions);

                var channel = await Client.GetChannelAsync(channelIdItem);
                if (channel != null && mensions != "")
                    await channel.SendMessageAsync(discordEmbedBuilder.Build());

                mensions = "";
            }
        }
    }
}
