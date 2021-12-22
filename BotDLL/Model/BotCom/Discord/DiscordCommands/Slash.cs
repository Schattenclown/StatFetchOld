using BotDLL.Model.Objects;
using BotDLL.Persistence;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using BotDLL.Model.QuickCharts;

namespace BotDLL.Model.BotCom.Discord.DiscordCommands
{
    /// <summary>
    /// The slash commands.
    /// </summary>
    internal class Slash : ApplicationCommandsModule
    {
        /// <summary>
        /// Send the help of this bot.
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("help", "StatFetch Help", true)]
        public static async Task HelpAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Title = "Help",
                Description = "This is the command help for the StatFetch Bot",
                Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.AddField("/42", "Show´s every Server with their informations");
            discordEmbedBuilder.AddField("/list", "Show´s the server list");
            discordEmbedBuilder.AddField("/status", "Show´s status from a singel server");
            discordEmbedBuilder.AddField("/add", "Adds you to an subscription for a server");
            discordEmbedBuilder.AddField("/addall", "Adds you to every serversubscription");
            discordEmbedBuilder.AddField("/del", "About what server do you wont get notified anymore");
            discordEmbedBuilder.AddField("/delall", "Deletes you from every serversubscription");
            discordEmbedBuilder.AddField("/abo", "Show´s about what servers you will get notified");
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor("StatFetch help");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Show´s every Server with their informations
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("42", "Show´s every Server with their informations", true)]
        public static async Task ShowServerStatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(FourtytwoTypeChoiceProvider))][Option("Type", "Type")] string fourtytwoChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                serverStatListLive.Add(serverStatObj);
            }
            var fourtytwoTypeChoiceProvider = new FourtytwoTypeChoiceProvider();
            var fourtytwoTypeChoices = await fourtytwoTypeChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            if ("STATISTICS".ToLower() == fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower())
            {
                foreach (ServerStat serverStatItem in serverStatListLive)
                {
                    DiscordEmbedBuilder discordEmbedBuilder = new();

                    foreach (ServerInfo serverInfoItem in serverInfoList)
                    {
                        if(serverInfoItem.ServerInfoId == serverStatItem.ServerInfoId)
                        {
                            serverInfoItem.QCUri = new Uri(QCUriGenerator.CreateObj(serverInfoItem).QCUri.AbsoluteUri);
                            discordEmbedBuilder.ImageUrl = serverInfoItem.QCUri.AbsoluteUri;
                        }
                    }

                    discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
                    discordEmbedBuilder.AddField($"Name", $"{serverStatItem.Name}", true);
                    discordEmbedBuilder.AddField("Game", serverStatItem.Game, true);
                    discordEmbedBuilder.AddField("Ip address", $"{serverStatItem.DynDnsAddress}:{serverStatItem.Port}", true);
                    discordEmbedBuilder.WithTimestamp(serverStatItem.FetchTime);

                    if (serverStatItem.ServerUp == true)
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Online", true);
                        discordEmbedBuilder.AddField("Players", $"{serverStatItem.Players}/{serverStatItem.MaxPlayers}", true);
                        discordEmbedBuilder.Color = DiscordColor.Green;
                        //buggy
                        //discordEmbedBuilder.AddField("Version", $"{serverStatItem.Version}", true);
                    }
                    else
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Offline", true);
                        //buggy
                        //discordEmbedBuilder.AddField("Version", "N/A", true);
                        discordEmbedBuilder.Color = DiscordColor.Red;
                        discordEmbedBuilder.AddField("Players", "N/A", true);
                        discordEmbedBuilder.AddField("UpTime", serverStatItem.UpTimeInPercent + "%", true);
                    }

                    await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
                }
            }
            else
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                bool isFull = "FULL".ToLower() == (fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower());
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

                foreach (ServerStat serverStatItem in serverStatListLive)
                {
                    DiscordEmbedBuilder discordEmbedBuilder = new();
                    discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
                    discordEmbedBuilder.AddField($"Name", $"{serverStatItem.Name}", true);
                    discordEmbedBuilder.AddField("Game", serverStatItem.Game, true);
                    discordEmbedBuilder.AddField("Ip address", $"{serverStatItem.DynDnsAddress}:{serverStatItem.Port}", true);
                    discordEmbedBuilder.WithTimestamp(serverStatItem.FetchTime);


                    if (serverStatItem.ServerUp == true)
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Online", true);
                        discordEmbedBuilder.AddField("Players", $"{serverStatItem.Players}/{serverStatItem.MaxPlayers}", true);
                        discordEmbedBuilder.Color = DiscordColor.Green;
                    }
                    else
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Offline", true);
                        discordEmbedBuilder.AddField("Version", "N/A", true);
                        discordEmbedBuilder.Color = DiscordColor.Red;
                    }

                    if (isFull)
                    {
                        if (serverStatItem.ServerUp == true)
                            discordEmbedBuilder.AddField("Version", $"{serverStatItem.Version}", true);
                        else
                            discordEmbedBuilder.AddField("Players", "N/A", true);
                        discordEmbedBuilder.AddField("UpTime", serverStatItem.UpTimeInPercent + "%", true);
                    }
                    await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
                }
            }
            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }

        /// <summary>
        /// Show´s the server list
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("list", "Show´s the server list", true)]
        public static async Task ListAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                serverStatListLive.Add(serverStatObj);
            }

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Description = "This is the list for all registered servers",
                Color = new DiscordColor(255, 0, 255)
            };

            foreach (ServerStat serverStatItem in serverStatListLive)
            {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                discordEmbedBuilder.AddField(serverStatItem.Name.ToUpper(), serverStatItem.Game.ToUpper());
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
            }

            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor("StatFetch list");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Show´s status of a singel server
        /// </summary>
        /// <param name="interactionContext">The ctx.</param>
        /// <param name="serverNameChoice">The servers.</param>
        [SlashCommand("status", "Show´s status from a singel server")]
        public static async Task StatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "status")] string serverNameChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoItem);
                if (serverInfoItem.Name.ToLower() == serverName.ToLower())
                    break;
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
            }

            DiscordEmbedBuilder discordEmbedBuilder = new();
            discordEmbedBuilder.AddField($"Name", $"{serverStatObj.Name}", true);
            discordEmbedBuilder.AddField("Game", serverStatObj.Game, true);
            discordEmbedBuilder.AddField("UpTime", serverStatObj.UpTimeInPercent + "%", true);
            discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true);
            discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");

            if (serverStatObj.ServerUp == true)
            {
                discordEmbedBuilder.AddField("ServerUp", $"Online", true);
                discordEmbedBuilder.AddField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true);
                discordEmbedBuilder.AddField("Version", $"{serverStatObj.Version}", true);
                discordEmbedBuilder.Color = DiscordColor.Green;
            }
            else
            {
                discordEmbedBuilder.AddField("ServerUp", $"Offline", true);
                discordEmbedBuilder.AddField("Players", "N/A", true);
                discordEmbedBuilder.AddField("Version", "N/A", true);
                discordEmbedBuilder.Color = DiscordColor.Red;
            }

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Show´s the playerstatistics from a singel server
        /// </summary>
        /// <param name="interactionContext">The ctx.</param>
        /// <param name="serverNameChoice">The servers.</param>
        [SlashCommand("statistics", "Show´s the playerstatistics from a singel server")]
        public static async Task StatisticsAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "statistics")] string serverNameChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();

            ServerStat serverStatObj = new();
            ServerInfo serverInfoObj = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                if (serverInfoItem.Name.ToLower() == serverName.ToLower())
                {
                    serverStatObj = ServerStat.CreateObj(serverInfoItem);
                    serverInfoObj = serverInfoItem;
                    break;
                }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
            }

            DiscordEmbedBuilder discordEmbedBuilder = new();

            serverInfoObj.QCUri = new Uri(QCUriGenerator.CreateObj(serverInfoObj).QCUri.AbsoluteUri);

            discordEmbedBuilder.ImageUrl = serverInfoObj.QCUri.AbsoluteUri;

            discordEmbedBuilder.AddField($"Name", $"{serverStatObj.Name}", true);
            discordEmbedBuilder.AddField("Game", serverStatObj.Game, true);
            discordEmbedBuilder.AddField("UpTime", serverStatObj.UpTimeInPercent + "%", true);
            discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true);
            discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");

            if (serverStatObj.ServerUp == true)
            {
                discordEmbedBuilder.AddField("ServerUp", $"Online", true);
                discordEmbedBuilder.AddField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true);
                //there is a bug somewhere
                //discordEmbedBuilder.AddField("Version", $"{serverStatObj.Version}", true);
                discordEmbedBuilder.Color = DiscordColor.Green;
            }
            else
            {
                discordEmbedBuilder.AddField("ServerUp", $"Offline", true);
                discordEmbedBuilder.AddField("Players", "N/A", true);
                //there is a bug somewhere
                //discordEmbedBuilder.AddField("Version", "N/A", true);
                discordEmbedBuilder.Color = DiscordColor.Red;
            }

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Adds you to an subscription for a server
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        /// <param name="servers">The servers.</param>
        /// <param name="type">The type of subscription.</param>
        [SlashCommand("add", "Adds you to an subscription for a server")]
        public static async Task AddAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "adding")] string serverNameChoice, [ChoiceProvider(typeof(AboTypeChoiceProvider))][Option("Type", "Type")] string aboTypeChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();

            var aboStateChoiceProvider = new AboTypeChoiceProvider();
            var aboStateChoices = await aboStateChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();
            bool isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == aboTypeChoice.ToLower()).Name.ToLower();


            DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, true, isMinimal);

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Adds you to every serversubscription
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("addall", "Adds you to every serversubscription", true)]
        public static async Task AddAllAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(AboTypeChoiceProvider))][Option("Type", "Type")] string aboTypeChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var aboStateChoiceProvider = new AboTypeChoiceProvider();
            var aboStateChoices = await aboStateChoiceProvider.Provider();
            bool isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == aboTypeChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoItem.Name, interactionContext, true, isMinimal);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }


        /// <summary>
        /// Delete´s you from an subscription for a server
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        /// <param name="serverNameChoice">The servers.</param>
        [SlashCommand("del", "Delete´s you from an subscription for a server")]
        public static async Task DelAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "deleting")] string serverNameChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

            DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, false, false);

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Delete´s you from an subscription for a server
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("delall", "Deletes you from every serversubscription", true)]
        public static async Task DelAllAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoItem.Name, interactionContext, false, false);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
                await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }

        /// <summary>
        /// ChangeSubscriptionCommand
        /// </summary>
        /// <param name="serverName">The servername.</param>
        /// <param name="interactionContext">The interaction context.</param>
        /// <param name="abo">Whether to abo or not.</param>
        /// <param name="isMinimal">Whether to low key abo or not.</param>
        public static DiscordEmbedBuilder ChangeSubscriptionCommand(string serverName, InteractionContext interactionContext, bool abo, bool isMinimal)
        {
            bool found = false;
            List<DCUserdata> dC_UserdataList = DB_DCUserdata.ReadAll();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoItem);
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
                if (serverInfoItem.Name.ToLower() == serverName.ToLower())
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                    break;
            }

            DCUserdata dC_UserdataObj = new()
            {
                ServerInfoId = serverStatObj.ServerInfoId,
                AuthorId = interactionContext.Member.Id,
                ChannelId = interactionContext.Channel.Id,
                Abo = abo,
                IsMinimalAbo = isMinimal
            };

            foreach (DCUserdata dC_UserdataItem in dC_UserdataList)
            {
                if (dC_UserdataItem.AuthorId == interactionContext.Member.Id && dC_UserdataItem.ChannelId == interactionContext.Channel.Id && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId)
                    found = true;
            }

            if (found)
                DCUserdata.Change(dC_UserdataObj);
            else
                DCUserdata.Add(dC_UserdataObj);

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Color = new DiscordColor(255, 0, 255)
            };

            if (abo && isMinimal)
                discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | MINIMAL";
            else if (abo && !isMinimal)
                discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | FULL";
            else if (!abo && isMinimal)
                discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";
            else if (!abo && !isMinimal)
                discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";

            discordEmbedBuilder.WithDescription("Who?:" + "<@" + interactionContext.Member.Id.ToString() + ">\n" + "Where?:" + "<#" + interactionContext.Channel.Id.ToString() + ">");
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor($"StatFetch {serverName.ToUpper()}");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);
            discordEmbedBuilder.Build();

            return discordEmbedBuilder;
        }


        /// <summary>
        /// Show´s about what servers you will get notified
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("abo", "Show´s about what servers you will get notified", true)]
        public static async Task ShowAboAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            List<DCUserdata> dC_UserdataList = DB_DCUserdata.ReadAll();
            List<DCUserdata> dC_UserdataListAbo = new();
            List<DCUserdata> dC_UserdataListAboSorted = new();

            bool sub2nothing = true;
            var differentchannel = new List<ulong>();
            string servers = "";
            ulong lastChannel = 0;

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
                serverStatListLive.Add(serverStatObj);
            }

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.WithDescription(interactionContext.Member.Mention);
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor("StatFetch abo");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);

            foreach (ServerStat serverStatItem in serverStatListLive)
            {
                foreach (DCUserdata dC_UserdataItem in dC_UserdataList)
                {
                    if (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatItem.ServerInfoId && Convert.ToUInt64(dC_UserdataItem.AuthorId) == interactionContext.Member.Id)
                    {
                        if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataItem.ChannelId)) && Convert.ToUInt64(dC_UserdataItem.AuthorId) == interactionContext.Member.Id)
                            differentchannel.Add(Convert.ToUInt64(dC_UserdataItem.ChannelId));

                        dC_UserdataListAbo.Add(dC_UserdataItem);

                        sub2nothing = false;
                    }
                }
            }

            if (sub2nothing)
                discordEmbedBuilder.AddField("You are unsubscribed from everything", ":(");
            else
            {
                foreach (ulong differentChannelItem in differentchannel)
                {
                    foreach (DCUserdata dC_UserdatatItem in dC_UserdataListAbo)
                    {
                        if (!dC_UserdataListAboSorted.Contains(dC_UserdatatItem) && dC_UserdatatItem.ChannelId == differentChannelItem)
                            dC_UserdataListAboSorted.Add(dC_UserdatatItem);
                    }
                }
            }

            foreach (DCUserdata dC_UserdataItem in dC_UserdataListAboSorted)
            {
                if (lastChannel == 0)
                    lastChannel = dC_UserdataItem.ChannelId;

                string serverInfoName = serverInfoList.First(firstMatch => firstMatch.ServerInfoId == dC_UserdataItem.ServerInfoId).Name;

                string aboType = "FULL";
                if (dC_UserdataItem.IsMinimalAbo)
                    aboType = "MINIMAL";

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                if (!servers.Contains(serverInfoName))
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
                    servers += $"{aboType} - {serverInfoName}\n";

                if (lastChannel != dC_UserdataItem.ChannelId)
                {
                    discordEmbedBuilder.AddField(servers, "<#" + lastChannel.ToString() + ">");
                    servers = "";
                    servers += $"{aboType} - {serverInfoName}\n";
                    lastChannel = dC_UserdataItem.ChannelId;
                }

                if (dC_UserdataListAboSorted.Last() == dC_UserdataItem)
                    discordEmbedBuilder.AddField(servers, "<#" + lastChannel.ToString() + ">");
            }

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Testst the functionality of the DCChange [player, status, version]
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("test", "Test´s the functionality of the DCChange [player, status, version]", true)]
        public static async Task TestAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "testserver")] string serverNameChoice, [ChoiceProvider(typeof(TestFunctionsChoiceProvider))][Option("Function", "function")] string testFunctionChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            DiscordEmbedBuilder discordEmbedBuilderLoading = new();
            discordEmbedBuilderLoading.WithDescription("Loading... ");
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();

            var testFunctionChoiceProvider = new TestFunctionsChoiceProvider();
            var testFunctionChoices = await testFunctionChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoItem);
                if (serverInfoItem.Name.ToLower() == serverName.ToLower())
                    break;
            }

            if ("PLAYERCOUNT".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
                DiscordBot.DCChange(serverStatObj, "player", false);
            else if ("ONLINESTATE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
                DiscordBot.DCChange(serverStatObj, "status", false);
            else if ("VERSIONCHANGE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
                DiscordBot.DCChange(serverStatObj, "version", false);

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }

        /// <summary>
        /// Generates an Invite link.
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("invite", "Invite StatFetch", true)]
        public static async Task InviteAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var botInvite = interactionContext.Client.GetInAppOAuth(Permissions.Administrator, OAuthScopes.BOT_MINIMAL);
            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent(botInvite.AbsoluteUri));
        }
    }
}
