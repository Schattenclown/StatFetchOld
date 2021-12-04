using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.EventHandling;
using DisCatSharp.Interactivity.Extensions;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BotDLL.Model.Objects;

using static BotDLL.Model.BotCom.Discord.Events.ClientEvents;
using static BotDLL.Model.BotCom.Discord.Events.GuildEvents;
using static BotDLL.Model.BotCom.Discord.Events.ApplicationCommandsEvents;

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
        private readonly Dictionary<TKey, List<TValue>> _data = new Dictionary<TKey, List<TValue>>();

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
        public Dictionary<TKey, List<TValue>> Get()
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
        /*private static List<LF_ServerInfo> lstlive = new List<LF_ServerInfo>();
        static List<DC_Userdata> lstud = new List<DC_Userdata>();*/
        /// <summary>
        /// The db.
        /// </summary>
        public static string token = "";
        public static int virgin = 0;
        /// <summary>
        /// Gets the client.
        /// </summary>
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
        public static readonly ulong testguild = 881868642600505354;
        public static string prefix = "sf/";

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
                token = connections.DiscordBotKey;
#if DEBUG
                token = connections.DiscordBotDebug;
#endif
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
            ApplicationCommands = Client.UseApplicationCommands();
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
            INext = null;
            CNext = null;
            Client = null;
            ApplicationCommands = null;
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
            
            /*while (!ShutdownRequest.IsCancellationRequested)
            {
                await Task.Delay(2000);
            }
            await Client.UpdateStatusAsync(activity: null, userStatus: UserStatus.Offline, idleSince: null);
            await Client.DisconnectAsync();
            await Task.Delay(2500);
            Dispose();*/
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
            cnext.RegisterCommands<DiscordCommands.Main>();
#if DEBUG
            ac.RegisterCommands<DiscordCommands.Slash>(testguild, perms =>
            {
                perms.AddRole(889266812267663380, true);
            });
#else
            ac.RegisterCommands<DiscordCommands.Slash>();
#endif
        }
        #endregion

        public static async void DCChange(ServerStat serverStatObj, string whatchanged)
        {
            List<DC_Userdata> dC_UserdataList = DC_Userdata.ReadAll();

            var differentchannel = new List<ulong>();
            bool once = false;

            DiscordEmbedBuilder discordEmbedBuilder = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor($"StatFetch");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);

            foreach (DC_Userdata dC_UserdataObjItem in dC_UserdataList)
            {
                if (dC_UserdataObjItem.Abo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id)
                {
                    if (!once)
                    {
                        discordEmbedBuilder.AddField($"Name", $"{serverStatObj.Name}");
                        discordEmbedBuilder.AddField("Game", serverStatObj.Game, false);
                        discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}");
                        if (whatchanged == "player")
                        {
                            discordEmbedBuilder.WithAuthor($"StatFetch Player count changed!");
                            discordEmbedBuilder.Title = $"{serverStatObj.Name}";
                            discordEmbedBuilder.Color = DiscordColor.Gold;
                            discordEmbedBuilder.AddField("Player count changed to ", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}");
                        }
                        else if (whatchanged == "status")
                        {
                            discordEmbedBuilder.WithAuthor($"StatFetch Status changed!");
                            discordEmbedBuilder.Title = $"{serverStatObj.Name}";
                            string serverUp = "Offline";
                            discordEmbedBuilder.Color = DiscordColor.Red;
                            if (serverStatObj.ServerUp)
                            {
                                serverUp = "Online";
                                discordEmbedBuilder.Color = DiscordColor.Green;
                            }
                            discordEmbedBuilder.AddField("Status changed to ", $"{serverUp}");
                        }
                        else if (whatchanged == "version")
                        {
                            discordEmbedBuilder.WithAuthor($"StatFetch Version changed!");
                            discordEmbedBuilder.Title = $"{serverStatObj.Name}";
                            discordEmbedBuilder.Color = DiscordColor.Gray;
                            discordEmbedBuilder.AddField("Serverversion changed to ", $"{serverStatObj.Version}");
                        }

                        once = true;
                    }
                    if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataObjItem.ChannelId)))
                        differentchannel.Add(Convert.ToUInt64(dC_UserdataObjItem.ChannelId));
                }
            }

            string tags = "";

            foreach (ulong channelId in differentchannel)
            {
                foreach (DC_Userdata dC_UserdataObjItem in dC_UserdataList)
                {
                    if (!tags.Contains(dC_UserdataObjItem.AuthorId.ToString()) && channelId == Convert.ToUInt64(dC_UserdataObjItem.ChannelId) && dC_UserdataObjItem.Abo && !dC_UserdataObjItem.MinimalAbo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id && !tags.Contains(dC_UserdataObjItem.AuthorId.ToString()))
                        tags += "<@" + dC_UserdataObjItem.AuthorId + ">" + "\n";
                    else if (!tags.Contains(dC_UserdataObjItem.AuthorId.ToString()) && channelId == Convert.ToUInt64(dC_UserdataObjItem.ChannelId) && dC_UserdataObjItem.MinimalAbo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id && serverStatObj.Players == 0 ||
                             !tags.Contains(dC_UserdataObjItem.AuthorId.ToString()) && channelId == Convert.ToUInt64(dC_UserdataObjItem.ChannelId) && dC_UserdataObjItem.MinimalAbo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id && serverStatObj.Players == 1)
                        tags += "<@" + dC_UserdataObjItem.AuthorId + ">" + "\n";
                    else if (!tags.Contains(dC_UserdataObjItem.AuthorId.ToString()) && channelId == Convert.ToUInt64(dC_UserdataObjItem.ChannelId) && dC_UserdataObjItem.Abo && dC_UserdataObjItem.MinimalAbo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id && whatchanged == "version" ||
                            !tags.Contains(dC_UserdataObjItem.AuthorId.ToString()) && channelId == Convert.ToUInt64(dC_UserdataObjItem.ChannelId) && dC_UserdataObjItem.Abo && dC_UserdataObjItem.MinimalAbo && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id && whatchanged == "status")
                        tags += "<@" + dC_UserdataObjItem.AuthorId + ">" + "\n";
                }

                discordEmbedBuilder.WithDescription(tags);

                var chn = await Client.GetChannelAsync(channelId);

                if (chn != null && tags != "")
                    await chn.SendMessageAsync(discordEmbedBuilder.Build());

                tags = "";
            }
        }
    }
}
