using BotDLL.Model.BotCom.Discord.ChoiceProvider;
using BotDLL.Model.BotCom.Discord.Main;
using BotDLL.Model.Objects;
using BotDLL.Model.QuickCharts;
using BotDLL.Persistence;
using BotDLL.Persistence.MSSQL;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;

namespace BotDLL.Model.BotCom.Discord.AppCommands
{
   /// <summary>
   ///    The slash commands.
   /// </summary>
   internal class Main : ApplicationCommandsModule
   {
      /// <summary>
      ///    Delete´s you from an subscription for a server
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="serverNameChoice">The servers.</param>
      [SlashCommand("del", "Delete´s you from an subscription for a server")]
      public static async Task DelAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider)), Option("Server", "deleting")] string serverNameChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         ServerNameChoiceProvider serverNameChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> serverNameChoices = await serverNameChoiceProvider.Provider();

         string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == serverNameChoice.ToLower()).Name.ToLower();


         DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, false, false);

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Delete´s you from an subscription for a server
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      [SlashCommand("delall", "Deletes you from every server-subscription")]
      public static async Task DelAllAsync(InteractionContext interactionContext)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         foreach (DiscordEmbedBuilder discordEmbedBuilder in from serverInfoItem in serverInfoList let serverName = serverInfoItem?.Name where serverName != null where serverName != null select ChangeSubscriptionCommand(serverName, interactionContext, false, false))
         {
            await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
      }

      /// <summary>
      ///    ChangeSubscriptionCommand
      /// </summary>
      /// <param name="serverName">The server name.</param>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="abo">Whether to abo or not.</param>
      /// <param name="isMinimal">Whether to low key abo or not.</param>
      public static DiscordEmbedBuilder ChangeSubscriptionCommand(string serverName, InteractionContext interactionContext, bool abo, bool isMinimal)
      {
         bool found = false;
         List<DCUserData> dCUserDataList = DB_DCUserdata.ReadAll();

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         ServerStat serverStatObj = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            serverStatObj = ServerStat.CreateObj(serverInfoItem);

            if (serverInfoItem?.Name?.ToLower() == serverName.ToLower())

            {
               break;
            }
         }

         DCUserData dCUserDataObj = new()
         {
            ServerInfoId = serverStatObj.ServerInfoId,
            AuthorId = interactionContext.Member.Id,
            ChannelId = interactionContext.Channel.Id,
            Abo = abo,
            IsMinimalAbo = isMinimal
         };

         foreach (DCUserData dCUserDataItem in dCUserDataList)
         {
            if (dCUserDataItem.AuthorId == interactionContext.Member.Id && dCUserDataItem.ChannelId == interactionContext.Channel.Id && dCUserDataItem.ServerInfoId == serverStatObj.ServerInfoId)
            {
               found = true;
            }
         }

         if (found)
         {
            DCUserData.Change(dCUserDataObj);
         }
         else
         {
            DCUserData.Add(dCUserDataObj);
         }

         DiscordEmbedBuilder discordEmbedBuilder = new()
         {
            Color = new DiscordColor(255, 0, 255)
         };

         if (abo && isMinimal)
         {
            discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | MINIMAL";
         }
         else if (abo && !isMinimal)
         {
            discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | FULL";
         }
         else if (!abo && isMinimal)
         {
            discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";
         }
         else if (!abo && !isMinimal)
         {
            discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";
         }

         discordEmbedBuilder.WithDescription("Who?:" + "<@" + interactionContext.Member.Id + ">\n" + "Where?:" + "<#" + interactionContext.Channel.Id + ">");
         discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
         discordEmbedBuilder.WithAuthor($"StatFetch {serverName.ToUpper()}");
         discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
         discordEmbedBuilder.WithTimestamp(DateTime.Now);
         discordEmbedBuilder.Build();

         return discordEmbedBuilder;
      }


      /// <summary>
      ///    Show´s about what servers you will get notified
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      [SlashCommand("abo", "Show´s about what servers you will get notified")]
      public static async Task ShowAboAsync(InteractionContext interactionContext)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         List<DCUserData> dCUserDataList = DB_DCUserdata.ReadAll();
         List<DCUserData> dCUserDataListAbo = new();
         List<DCUserData> dCUserDataListAboSorted = new();
         if (dCUserDataListAboSorted == null)
         {
            throw new ArgumentNullException(nameof(dCUserDataListAboSorted));
         }

         bool sub2Nothing = true;
         List<ulong> differentChannel = new();
         string servers = "";
         ulong lastChannel = 0;

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         List<ServerStat> serverStatListLive = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
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
         foreach (DCUserData dCUserDataItem in dCUserDataList)
         {
            if (dCUserDataItem.Abo && dCUserDataItem.ServerInfoId == serverStatItem.ServerInfoId && Convert.ToUInt64(dCUserDataItem.AuthorId) == interactionContext.Member.Id)
            {
               if (!differentChannel.Contains(Convert.ToUInt64(dCUserDataItem.ChannelId)) && Convert.ToUInt64(dCUserDataItem.AuthorId) == interactionContext.Member.Id)
               {
                  differentChannel.Add(Convert.ToUInt64(dCUserDataItem.ChannelId));
               }

               dCUserDataListAbo.Add(dCUserDataItem);

               sub2Nothing = false;
            }
         }

         if (sub2Nothing)
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField("You are unsubscribed from everything", ":("));
         }
         else
         {
            foreach (ulong differentChannelItem in differentChannel)
            foreach (DCUserData dCUserDataItem in dCUserDataListAbo)
            {
               if (!dCUserDataListAboSorted.Contains(dCUserDataItem) && dCUserDataItem.ChannelId == differentChannelItem)
               {
                  dCUserDataListAboSorted.Add(dCUserDataItem);
               }
            }
         }

         foreach (DCUserData dCUserDataItem in dCUserDataListAboSorted)
         {
            if (lastChannel == 0)
            {
               lastChannel = dCUserDataItem.ChannelId;
            }

            string? serverInfoName = serverInfoList.First(firstMatch => firstMatch != null && firstMatch.ServerInfoId == dCUserDataItem.ServerInfoId)?.Name;

            string aboType = "FULL";
            if (dCUserDataItem.IsMinimalAbo)
            {
               aboType = "MINIMAL";
            }


            if (serverInfoName != null && !servers.Contains(serverInfoName))

            {
               servers += $"{aboType} - {serverInfoName}\n";
            }

            if (lastChannel != dCUserDataItem.ChannelId)
            {
               discordEmbedBuilder.AddField(new DiscordEmbedField(servers, "<#" + lastChannel + ">"));
               servers = "";
               servers += $"{aboType} - {serverInfoName}\n";
               lastChannel = dCUserDataItem.ChannelId;
            }

            if (dCUserDataListAboSorted.Last() == dCUserDataItem)
            {
               discordEmbedBuilder.AddField(new DiscordEmbedField(servers, "<#" + lastChannel + ">"));
            }
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Tester for the functionality of the DCChange [player, status, version]
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="serverNameChoice"></param>
      /// <param name="testFunctionChoice"></param>
      [SlashCommand("test", "Test´s the functionality of the DCChange [player, status, version]", true)]
      public static async Task TestAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider)), Option("Server", "test server")] string serverNameChoice, [ChoiceProvider(typeof(TestFunctionsChoiceProvider)), Option("Function", "function")] string testFunctionChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         ServerNameChoiceProvider serverNameChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> serverNameChoices = await serverNameChoiceProvider.Provider();

         TestFunctionsChoiceProvider testFunctionChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> testFunctionChoices = await testFunctionChoiceProvider.Provider();

         string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == serverNameChoice.ToLower()).Name.ToLower();

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         ServerStat serverStatObj = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            serverStatObj = ServerStat.CreateObj(serverInfoItem);
            if (serverInfoItem?.Name?.ToLower() == serverName.ToLower())
            {
               break;
            }
         }

         IEnumerable<DiscordApplicationCommandOptionChoice> discordApplicationCommandOptionChoices = testFunctionChoices as DiscordApplicationCommandOptionChoice[] ?? testFunctionChoices.ToArray();
         if (string.Equals("PLAYERCOUNT", discordApplicationCommandOptionChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == testFunctionChoice.ToLower()).Name, StringComparison.CurrentCultureIgnoreCase))
         {
            Bot.DC_Change(serverStatObj, "player", false);
         }
         else if (string.Equals("ONLINESTATE", discordApplicationCommandOptionChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == testFunctionChoice.ToLower()).Name, StringComparison.CurrentCultureIgnoreCase))
         {
            Bot.DC_Change(serverStatObj, "status", false);
         }
         else if (string.Equals("VERSIONCHANGE", discordApplicationCommandOptionChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == testFunctionChoice.ToLower()).Name, StringComparison.CurrentCultureIgnoreCase))
         {
            Bot.DC_Change(serverStatObj, "version", false);
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
      }

      /// <summary>
      ///    Generates an Invite link.
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      [SlashCommand("invite", "Invite StatFetch")]
      public static async Task InviteAsync(InteractionContext interactionContext)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         Uri? botInvite = interactionContext.Client.GetInAppOAuth(Permissions.Administrator, OAuthScopes.BOT_MINIMAL);
         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent(botInvite.AbsoluteUri));
      }

      /// <summary>
      ///    Show´s every Server with their information
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="fourtytwoChoice"></param>
      [SlashCommand("42", "Show´s every Server with their information")]
      public static async Task ShowServerStatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(FourtytwoTypeChoiceProvider)), Option("Type", "Type")] string fourtytwoChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         List<ServerStat> serverStatListLive = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            ServerStat serverStatObj = ServerStat.CreateObj(serverInfoItem);
            serverStatListLive.Add(serverStatObj);
         }

         FourtytwoTypeChoiceProvider fourtytwoTypeChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> fortytwoTypeChoices = await fourtytwoTypeChoiceProvider.Provider();

         IEnumerable<DiscordApplicationCommandOptionChoice> discordApplicationCommandOptionChoices = fortytwoTypeChoices as DiscordApplicationCommandOptionChoice[] ?? fortytwoTypeChoices.ToArray();
         if ("STATISTICS".ToLower() == discordApplicationCommandOptionChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == fourtytwoChoice.ToLower()).Name.ToLower())
         {
            foreach (ServerStat serverStatItem in serverStatListLive)
            {
               DiscordEmbedBuilder discordEmbedBuilder = new();

               foreach (ServerInfo? serverInfoItem in serverInfoList)
               {
                  if (serverInfoItem != null && serverInfoItem.ServerInfoId == serverStatItem.ServerInfoId)
                  {
                     string qcUriAbsoluteUri = QC_UriGenerator.CreateObj(serverInfoItem).QcUri?.AbsoluteUri;
                     if (qcUriAbsoluteUri != null)
                     {
                        serverInfoItem.QcUri = new Uri(qcUriAbsoluteUri);
                        discordEmbedBuilder.ImageUrl = serverInfoItem.QcUri.AbsoluteUri;
                     }
                  }
               }

               discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
               discordEmbedBuilder.AddField(new DiscordEmbedField("Name", $"{serverStatItem.Name}", true));
               discordEmbedBuilder.AddField(new DiscordEmbedField("Game", serverStatItem.Game, true));
               discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatItem.DynDnsAddress}:{serverStatItem.Port}", true));
               discordEmbedBuilder.WithTimestamp(serverStatItem.FetchTime);

               if (serverStatItem.ServerUp)
               {
                  discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Online", true));
                  discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatItem.Players}/{serverStatItem.MaxPlayers}", true));
                  discordEmbedBuilder.Color = DiscordColor.Green;
                  //buggy
                  //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatItem.Version}", true);
               }
               else
               {
                  discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Offline", true));
                  //buggy
                  //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", "N/A", true);
                  discordEmbedBuilder.Color = DiscordColor.Red;
                  discordEmbedBuilder.AddField(new DiscordEmbedField("Players", "N/A", true));
                  discordEmbedBuilder.AddField(new DiscordEmbedField("UpTime", serverStatItem.UpTimeInPercent + "%", true));
               }

               await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }
         }
         else
         {
            bool isFull = "FULL".ToLower() == discordApplicationCommandOptionChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == fourtytwoChoice.ToLower()).Name.ToLower();

            foreach (ServerStat serverStatItem in serverStatListLive)
            {
               DiscordEmbedBuilder discordEmbedBuilder = new();
               discordEmbedBuilder.WithDescription("               ⁣   ⁣                                               ⁣");
               discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
               discordEmbedBuilder.AddField(new DiscordEmbedField("Name", $"{serverStatItem.Name}", true));
               discordEmbedBuilder.AddField(new DiscordEmbedField("Game", serverStatItem.Game, true));
               discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatItem.DynDnsAddress}:{serverStatItem.Port}", true));
               discordEmbedBuilder.WithTimestamp(serverStatItem.FetchTime);


               if (serverStatItem.ServerUp)
               {
                  discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Online", true));
                  discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatItem.Players}/{serverStatItem.MaxPlayers}", true));
                  discordEmbedBuilder.Color = DiscordColor.Green;
               }
               else
               {
                  discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Offline", true));
                  //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", "N/A", true);
                  discordEmbedBuilder.Color = DiscordColor.Red;
               }

               if (isFull)
               {
                  if (serverStatItem.ServerUp)
                  {
                     //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatItem.Version}", true);
                  }
                  else
                  {
                     discordEmbedBuilder.AddField(new DiscordEmbedField("Players", "N/A", true));
                  }

                  discordEmbedBuilder.AddField(new DiscordEmbedField("UpTime", serverStatItem.UpTimeInPercent + "%", true));
               }

               await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
      }

      /// <summary>
      ///    Show´s the server list
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      [SlashCommand("list", "Show´s the server list")]
      public static async Task ListAsync(InteractionContext interactionContext)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         List<ServerStat> serverStatListLive = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
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
            discordEmbedBuilder.AddField(new DiscordEmbedField(serverStatItem.Name?.ToUpper(), serverStatItem.Game?.ToUpper()));
         }

         discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
         discordEmbedBuilder.WithAuthor("StatFetch list");
         discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
         discordEmbedBuilder.WithTimestamp(DateTime.Now);

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Show´s status of a single server
      /// </summary>
      /// <param name="interactionContext">The ctx.</param>
      /// <param name="serverNameChoice">The servers.</param>
      [SlashCommand("status", "Show´s status from a single server")]
      public static async Task StatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider)), Option("Server", "status")] string serverNameChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         ServerNameChoiceProvider serverNameChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> serverNameChoices = await serverNameChoiceProvider.Provider();
         string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == serverNameChoice.ToLower()).Name.ToLower();

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         ServerInfo? serverInfo = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            serverInfo = serverInfoItem;
            if (serverInfoItem?.Name?.ToLower() == serverName.ToLower())
            {
               break;
            }
         }

         ServerStat serverStatObj = ServerStat.CreateObj(serverInfo);

         DiscordEmbedBuilder discordEmbedBuilder = new();
         discordEmbedBuilder.AddField(new DiscordEmbedField("Name", $"{serverStatObj.Name}", true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("Game", serverStatObj.Game, true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("UpTime", serverStatObj.UpTimeInPercent + "%", true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true));
         discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);
         discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");

         if (serverStatObj.ServerUp)
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Online", true));
            discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true));

            if (serverStatObj.Version != "")
            {
               discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatObj.Version}", true));
            }

            ServerUsageObj serverUsageObj = DataBaseConnection.Read(serverStatObj.Port);
            if (serverUsageObj.RAMUsage != 0)
            {
               discordEmbedBuilder.AddField(new DiscordEmbedField("CPU usage", $"{serverUsageObj.CPUUsage} %", true));
               discordEmbedBuilder.AddField(new DiscordEmbedField("RAM usage", $"{serverUsageObj.RAMUsage} MB", true));
            }

            discordEmbedBuilder.Color = DiscordColor.Green;
         }
         else
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Offline", true));
            discordEmbedBuilder.AddField(new DiscordEmbedField("Players", "N/A", true));
            discordEmbedBuilder.AddField(new DiscordEmbedField("Version", "N/A", true));
            discordEmbedBuilder.Color = DiscordColor.Red;
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Show´s the player statistics from a single server
      /// </summary>
      /// <param name="interactionContext">The ctx.</param>
      /// <param name="serverNameChoice">The servers.</param>
      [SlashCommand("statistics", "Show´s the player-statistics from a single server")]
      public static async Task StatisticsAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider)), Option("Server", "statistics")] string serverNameChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         ServerNameChoiceProvider serverNameChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> serverNameChoices = await serverNameChoiceProvider.Provider();
         string serverName = serverNameChoices.First(firstMatch => string.Equals(firstMatch.Value.ToString(), serverNameChoice, StringComparison.CurrentCultureIgnoreCase)).Name.ToLower();

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();

         ServerInfo? serverInfoObj = new();
         foreach (ServerInfo? serverInfoItem in serverInfoList.Where(serverInfoItem => serverInfoItem?.Name != null && string.Equals(serverInfoItem.Name, serverName, StringComparison.CurrentCultureIgnoreCase)))
         {
            serverInfoObj = serverInfoItem;
            break;
         }

         ServerStat serverStatObj = ServerStat.CreateObj(serverInfoObj);

         DiscordEmbedBuilder discordEmbedBuilder = new();

         Uri? qcUri = QC_UriGenerator.CreateObj(serverInfoObj).QcUri;
         if (qcUri != null)
         {
            if (serverInfoObj != null)
            {
               serverInfoObj.QcUri = new Uri(qcUri.AbsoluteUri);
            }
         }

         discordEmbedBuilder.ImageUrl = serverInfoObj?.QcUri.AbsoluteUri;

         discordEmbedBuilder.AddField(new DiscordEmbedField("Name", $"{serverStatObj.Name}", true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("Game", serverStatObj.Game, true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("UpTime", serverStatObj.UpTimeInPercent + "%", true));
         discordEmbedBuilder.AddField(new DiscordEmbedField("Ip address", $"{serverStatObj.DynDnsAddress}:{serverStatObj.Port}", true));
         discordEmbedBuilder.WithTimestamp(serverStatObj.FetchTime);
         discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");

         if (serverStatObj.ServerUp)
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Online", true));
            discordEmbedBuilder.AddField(new DiscordEmbedField("Players", $"{serverStatObj.Players}/{serverStatObj.MaxPlayers}", true));
            //there is a bug somewhere
            //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatObj.Version}", true);
            discordEmbedBuilder.Color = DiscordColor.Green;
         }
         else
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField("ServerUp", "Offline", true));
            discordEmbedBuilder.AddField(new DiscordEmbedField("Players", "N/A", true));
            //there is a bug somewhere
            //discordEmbedBuilder.AddField(new DiscordEmbedField("Version", "N/A", true);
            discordEmbedBuilder.Color = DiscordColor.Red;
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Adds you to an subscription for a server
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="serverNameChoice"></param>
      /// <param name="aboTypeChoice"></param>
      [SlashCommand("add", "Adds you to an subscription for a server")]
      public static async Task AddAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider)), Option("Server", "adding")] string serverNameChoice, [ChoiceProvider(typeof(AboTypeChoiceProvider)), Option("Type", "Type")] string aboTypeChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         ServerNameChoiceProvider serverNameChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> serverNameChoices = await serverNameChoiceProvider.Provider();

         AboTypeChoiceProvider aboStateChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> aboStateChoices = await aboStateChoiceProvider.Provider();

         string serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == serverNameChoice.ToLower()).Name.ToLower();
         bool isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == aboTypeChoice.ToLower()).Name.ToLower();


         DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, true, isMinimal);

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      /// <summary>
      ///    Adds you to every server-subscription
      /// </summary>
      /// <param name="interactionContext">The interaction context.</param>
      /// <param name="aboTypeChoice"></param>
      [SlashCommand("addall", "Adds you to every server-subscription")]
      public static async Task AddAllAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(AboTypeChoiceProvider)), Option("Type", "Type")] string aboTypeChoice)
      {
         await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
         DiscordEmbedBuilder discordEmbedBuilderLoading = new();
         discordEmbedBuilderLoading.WithDescription("Loading... ");
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

         AboTypeChoiceProvider aboStateChoiceProvider = new();
         IEnumerable<DiscordApplicationCommandOptionChoice> aboStateChoices = await aboStateChoiceProvider.Provider();
         bool isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString()?.ToLower() == aboTypeChoice.ToLower()).Name.ToLower();

         List<ServerInfo?> serverInfoList = ServerInfo.ReadAll();
         foreach (ServerInfo? serverInfoItem in serverInfoList)
         {
            string? serverName = serverInfoItem!.Name;
            if (serverName != null)
            {
               DiscordEmbedBuilder discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, true, isMinimal);
               await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
            }
         }

         await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
      }
   }
}