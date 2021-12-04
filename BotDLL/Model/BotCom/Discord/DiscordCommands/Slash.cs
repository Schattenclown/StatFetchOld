using BotDLL.Model.Objects;
using BotDLL.Persistence;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.Entities;
using DisCatSharp.Enums;

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
        /// <param name="inertactionContext">The interaction context.</param>
        [SlashCommand("help", "StatFetch Help", true)]
        public static async Task HelpAsync(InteractionContext inertactionContext)
        {
            await inertactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

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

            await inertactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Show´s every Server with their informations
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("42", "Show´s every Server with their informations", true)]
        public static async Task ShowServerStatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(FourtytwoTypeChoiceProvider))][Option("Type", "Type")] string fourtytwoChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoObj in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                serverStatListLive.Add(serverStatObj);
            }
            var fourtytwoTypeChoiceProvider = new FourtytwoTypeChoiceProvider();
            var fourtytwoTypeChoices = await fourtytwoTypeChoiceProvider.Provider();

            if ("STATISTICS".ToLower() == fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower())
            {
                DiscordEmbedBuilder discordEmbedBuilder = new();
                discordEmbedBuilder.Title = "Statistics N/A";
                await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }
            else
            {
                bool isFull = "FULL".ToLower() == (fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower());

                foreach (ServerStat serverStatObj in serverStatListLive)
                {
                    DiscordEmbedBuilder discordEmbedBuilder = new();
                    discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
                    discordEmbedBuilder.Title = "Status";
                    discordEmbedBuilder.AddField($"Name", $"{serverStatObj.Name}");
                    discordEmbedBuilder.AddField("Game", serverStatObj.Game, false);
                    discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true);
                    discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);


                    if (serverStatObj.ServerUp == true)
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Online", false);
                        discordEmbedBuilder.AddField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true);
                        discordEmbedBuilder.Color = DiscordColor.Green;
                    }
                    else
                    {
                        discordEmbedBuilder.AddField("ServerUp", $"Offline", false);
                        discordEmbedBuilder.AddField("Version", "N/A", true);
                        discordEmbedBuilder.Color = DiscordColor.Red;
                    }

                    if (isFull)
                    {
                        if (serverStatObj.ServerUp == true)
                            discordEmbedBuilder.AddField("Version", $"{serverStatObj.Version}", true);
                        else
                            discordEmbedBuilder.AddField("Players", "N/A", true);
                    }
                    await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));

                }
            }
            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }

        /// <summary>
        /// Show´s the server list
        /// </summary>
        /// <param name="interationContext">The interaction context.</param>
        [SlashCommand("list", "Show´s the server list", true)]
        public static async Task ListAsync(InteractionContext interationContext)
        {
            await interationContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoObj in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                serverStatListLive.Add(serverStatObj);
            }

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Description = "This is the list for all registered servers",
                Color = new DiscordColor(255, 0, 255)
            };

            foreach (ServerStat serverStatObj in serverStatListLive)
            {
                discordEmbedBuilder.AddField(serverStatObj.Name.ToUpper(), serverStatObj.Game.ToUpper());
            }

            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor("StatFetch list");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);

            await interationContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Show´s status of a singel server
        /// </summary>
        /// <param name="interactionContext">The ctx.</param>
        /// <param name="serverNameChoice">The servers.</param>
        [SlashCommand("status", "Show´s status from a singel server")]
        public static async Task StatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))][Option("Server", "status")] string serverNameChoice)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoObjItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoObjItem);
                if (serverInfoObjItem.Name.ToLower() == serverName.ToLower())
                    break;
            }

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Title = "Status"
            };
            discordEmbedBuilder.AddField($"Name", $"{serverStatObj.Name}");
            discordEmbedBuilder.AddField("Game", serverStatObj.Game, false);
            discordEmbedBuilder.AddField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true);
            discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");

            if (serverStatObj.ServerUp == true)
            {
                discordEmbedBuilder.AddField("ServerUp", $"Online", false);
                discordEmbedBuilder.AddField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true);
                discordEmbedBuilder.AddField("Version", $"{serverStatObj.Version}", true);
                discordEmbedBuilder.Color = DiscordColor.Green;
            }
            else
            {
                discordEmbedBuilder.AddField("ServerUp", $"Offline", false);
                discordEmbedBuilder.AddField("Players", "N/A", true);
                discordEmbedBuilder.AddField("Version", "N/A", true);
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
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();

            var aboStateChoiceProvider = new AboTypeChoiceProvider();
            var aboStateChoices = await aboStateChoiceProvider.Provider();

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
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            var aboStateChoiceProvider = new AboTypeChoiceProvider();
            var aboStateChoices = await aboStateChoiceProvider.Provider();
            bool isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == aboTypeChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            foreach (ServerInfo serverInfoObjItem in serverInfoList)
            {
                DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoObjItem.Name, interactionContext, true, isMinimal);
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
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();
            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

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
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            foreach (ServerInfo serverInfoObjItem in serverInfoList)
            {
                DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoObjItem.Name, interactionContext, false, false);
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
            List<DC_Userdata> dC_UserdataList = DB_DC_Userdata.ReadAll();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoObjItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoObjItem);
                if (serverInfoObjItem.Name.ToLower() == serverName.ToLower())
                    break;
            }

            DC_Userdata dC_UserdataObj = new()
            {
                AuthorId = interactionContext.Member.Id,
                ChannelId = interactionContext.Channel.Id,
                ServerInfoId = serverStatObj.Id,
                Abo = abo,
                MinimalAbo = isMinimal
            };

            foreach (DC_Userdata dC_UserdataObjItem in dC_UserdataList)
            {
                if (dC_UserdataObjItem.AuthorId == interactionContext.Member.Id && dC_UserdataObjItem.ChannelId == interactionContext.Channel.Id && dC_UserdataObjItem.ServerInfoId == serverStatObj.Id)
                    found = true;
            }

            if (found)
                DC_Userdata.Change(dC_UserdataObj);
            else
                DC_Userdata.Add(dC_UserdataObj);

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Color = new DiscordColor(255, 0, 255)
            };

            if (abo && isMinimal)
                discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | MINIMAL";
            else if (abo && !isMinimal)
                discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | FULL";
            else if (!abo && isMinimal)
                discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore! | MINIMAL";
            else if (!abo && !isMinimal)
                discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore! | FULL";

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

            List<DC_Userdata> dC_UserdataList = DB_DC_Userdata.ReadAll();
            List<DC_Userdata> dC_UserdataListAbo = new();
            List<DC_Userdata> dC_UserdataListAboSorted = new();

            bool sub2nothing = true;
            var differentchannel = new List<ulong>();
            string servers = "";
            ulong lastChannel = 0;

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            List<ServerStat> serverStatListLive = new();
            foreach (ServerInfo serverInfoObj in serverInfoList)
            {
                ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);
                serverStatListLive.Add(serverStatObj);
            }

            DiscordEmbedBuilder discordEmbedBuilder = new()
            {
                Title = "/abo",
                Color = new DiscordColor(255, 0, 255)
            };
            discordEmbedBuilder.WithDescription(interactionContext.Member.Mention);
            discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
            discordEmbedBuilder.WithAuthor("StatFetch abo");
            discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
            discordEmbedBuilder.WithTimestamp(DateTime.Now);


            foreach (ServerStat serverStatObjItem in serverStatListLive)
            {
                foreach (DC_Userdata dC_UserdataObjItem in dC_UserdataList)
                {
                    if (dC_UserdataObjItem.Abo && dC_UserdataObjItem.ServerInfoId == serverStatObjItem.Id && Convert.ToUInt64(dC_UserdataObjItem.AuthorId) == interactionContext.Member.Id)
                    {
                        if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataObjItem.ChannelId)) && Convert.ToUInt64(dC_UserdataObjItem.AuthorId) == interactionContext.Member.Id)
                            differentchannel.Add(Convert.ToUInt64(dC_UserdataObjItem.ChannelId));

                        dC_UserdataListAbo.Add(dC_UserdataObjItem);

                        sub2nothing = false;
                    }
                }
            }

            if (sub2nothing)
                discordEmbedBuilder.AddField("You are unsubscribed to everything", ":(");
            else
            {
                foreach (ulong diffchn in differentchannel)
                {
                    foreach (DC_Userdata dC_UserdatatObjItem in dC_UserdataListAbo)
                    {
                        if (!dC_UserdataListAboSorted.Contains(dC_UserdatatObjItem) && dC_UserdatatObjItem.ChannelId == diffchn)
                            dC_UserdataListAboSorted.Add(dC_UserdatatObjItem);
                    }
                }
            }


            foreach (DC_Userdata dC_UserdataObjItem in dC_UserdataListAboSorted)
            {
                if (lastChannel == 0)
                    lastChannel = dC_UserdataObjItem.ChannelId;

                string serverInfoName = serverInfoList.First(firstMatch => firstMatch.Id == dC_UserdataObjItem.ServerInfoId).Name;

                if (!servers.Contains(serverInfoName))
                {
                    string aboType = "FULL";
                    if (dC_UserdataObjItem.MinimalAbo)
                        aboType = "MINIMAL";

                    servers += $"{serverInfoName,-15} {aboType,7} \n";
                }

                if (lastChannel != dC_UserdataObjItem.ChannelId)
                {
                    discordEmbedBuilder.AddField(servers, "<#" + lastChannel.ToString() + ">");
                    servers = "";
                    servers += serverInfoName + "\n";
                    lastChannel = dC_UserdataObjItem.ChannelId;
                }

                if (dC_UserdataListAboSorted.Last() == dC_UserdataObjItem)
                {
                    discordEmbedBuilder.AddField(servers, "<#" + lastChannel.ToString() + ">");
                }
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
            await interactionContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Loading..."));

            var serverNameChoiceProvider = new ServerNameChoiceProvider();
            var serverNameChoices = await serverNameChoiceProvider.Provider();

            var testFunctionChoiceProvider = new TestFunctionsChoiceProvider();
            var testFunctionChoices = await testFunctionChoiceProvider.Provider();

            string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

            List<ServerInfo> serverInfoList = ServerInfo.ReadAll();
            ServerStat serverStatObj = new();
            foreach (ServerInfo serverInfoObjItem in serverInfoList)
            {
                serverStatObj = ServerStat.CreateObj(serverInfoObjItem);
                if (serverInfoObjItem.Name.ToLower() == serverName.ToLower())
                    break;
            }

            if ("PLAYERCOUNT".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
                DiscordBot.DCChange(serverStatObj, "player");
            else if ("ONLINESTATE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
                DiscordBot.DCChange(serverStatObj, "status");
            else if ("VERSIONCHANGE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
                DiscordBot.DCChange(serverStatObj, "version");

            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
        }

        /// <summary>
        /// Gets the user's avatar & banner.
        /// </summary>
        /// <param name="contentMenuContext">The contextmenu context.</param>
        [ContextMenu(ApplicationCommandType.User, "Get avatar & banner")]
        public static async Task GetUserBannerAsync(ContextMenuContext contentMenuContext)
        {
            var user = await contentMenuContext.Client.GetUserAsync(contentMenuContext.TargetUser.Id, true);

            var discordEmbedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Avatar & Banner of {user.Username}",
                ImageUrl = user.BannerHash != null ? user.BannerUrl : null
            }.
            WithThumbnail(user.AvatarUrl).
            WithColor(user.BannerColor ?? DiscordColor.Aquamarine).
            WithFooter($"Requested by {contentMenuContext.Member.DisplayName}", contentMenuContext.Member.AvatarUrl).
            WithAuthor($"{user.Username}", user.AvatarUrl, user.AvatarUrl);
            await contentMenuContext.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(discordEmbedBuilder.Build()));
        }

        /// <summary>
        /// Generates an Invite link.
        /// </summary>
        /// <param name="interactionContext">The interaction context.</param>
        [SlashCommand("invite", "Invite StatFetch", true)]
        public static async Task InviteAsync(InteractionContext interactionContext)
        {
            await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var botInvite = interactionContext.Client.GetInAppOAuth(Permissions.Administrator);
            await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent(botInvite.AbsoluteUri));
        }
    }
}
