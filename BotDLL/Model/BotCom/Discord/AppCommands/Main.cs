using BotDLL.Model.BotCom.Discord.DiscordCommands;
using BotDLL.Model.BotCom.Discord.Main;
using BotDLL.Model.Objects;
using BotDLL.Model.QuickCharts;
using BotDLL.Persistence;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.Attributes;
using DisCatSharp.ApplicationCommands.Context;
using DisCatSharp.Entities;
using DisCatSharp.Enums;

namespace BotDLL.Model.BotCom.Discord.AppCommands;

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
   public static async Task DelAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))] [Option("Server", "deleting")] string serverNameChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      ServerNameChoiceProvider? serverNameChoiceProvider = new();
      var serverNameChoices = await serverNameChoiceProvider.Provider();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
      var serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

      var discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, false, false);

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
   }

   /// <summary>
   ///    Delete´s you from an subscription for a server
   /// </summary>
   /// <param name="interactionContext">The interaction context.</param>
   [SlashCommand("delall", "Deletes you from every serversubscription")]
   public static async Task DelAllAsync(InteractionContext interactionContext)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      var serverInfoList = ServerInfo.ReadAll();
      foreach (var serverInfoItem in serverInfoList)
      {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
         var discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoItem.Name, interactionContext, false, false);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
   }

   /// <summary>
   ///    ChangeSubscriptionCommand
   /// </summary>
   /// <param name="serverName">The servername.</param>
   /// <param name="interactionContext">The interaction context.</param>
   /// <param name="abo">Whether to abo or not.</param>
   /// <param name="isMinimal">Whether to low key abo or not.</param>
   public static DiscordEmbedBuilder ChangeSubscriptionCommand(string serverName, InteractionContext interactionContext, bool abo, bool isMinimal)
   {
      var found = false;
      var dC_UserdataList = DB_DCUserdata.ReadAll();

      var serverInfoList = ServerInfo.ReadAll();
      ServerStat serverStatObj = new();
      foreach (var serverInfoItem in serverInfoList)
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

      foreach (var dC_UserdataItem in dC_UserdataList)
         if (dC_UserdataItem.AuthorId == interactionContext.Member.Id && dC_UserdataItem.ChannelId == interactionContext.Channel.Id && dC_UserdataItem.ServerInfoId == serverStatObj.ServerInfoId)
            found = true;

      if (found)
         DCUserdata.Change(dC_UserdataObj);
      else
         DCUserdata.Add(dC_UserdataObj);

      DiscordEmbedBuilder discordEmbedBuilder = new() { Color = new DiscordColor(255, 0, 255) };

      if (abo && isMinimal)
         discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | MINIMAL";
      else if (abo && !isMinimal)
         discordEmbedBuilder.Title = $"You will get notifications for {serverName}! | FULL";
      else if (!abo && isMinimal)
         discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";
      else if (!abo && !isMinimal)
         discordEmbedBuilder.Title = $"You will not get notified for {serverName} anymore!";

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

      var dC_UserdataList = DB_DCUserdata.ReadAll();
      List<DCUserdata> dC_UserdataListAbo = new();
      List<DCUserdata> dC_UserdataListAboSorted = new();

      var sub2nothing = true;
      List<ulong>? differentchannel = new();
      var servers = "";
      ulong lastChannel = 0;

      var serverInfoList = ServerInfo.ReadAll();
      List<ServerStat> serverStatListLive = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         var serverStatObj = ServerStat.CreateObj(serverInfoItem);
         serverStatListLive.Add(serverStatObj);
      }

      DiscordEmbedBuilder discordEmbedBuilder = new() { Color = new DiscordColor(255, 0, 255) };
      discordEmbedBuilder.WithDescription(interactionContext.Member.Mention);
      discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
      discordEmbedBuilder.WithAuthor("StatFetch abo");
      discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
      discordEmbedBuilder.WithTimestamp(DateTime.Now);

      foreach (var serverStatItem in serverStatListLive)
      foreach (var dC_UserdataItem in dC_UserdataList)
         if (dC_UserdataItem.Abo && dC_UserdataItem.ServerInfoId == serverStatItem.ServerInfoId && Convert.ToUInt64(dC_UserdataItem.AuthorId) == interactionContext.Member.Id)
         {
            if (!differentchannel.Contains(Convert.ToUInt64(dC_UserdataItem.ChannelId)) && Convert.ToUInt64(dC_UserdataItem.AuthorId) == interactionContext.Member.Id)
               differentchannel.Add(Convert.ToUInt64(dC_UserdataItem.ChannelId));

            dC_UserdataListAbo.Add(dC_UserdataItem);

            sub2nothing = false;
         }

      if (sub2nothing)
         discordEmbedBuilder.AddField(new DiscordEmbedField("You are unsubscribed from everything", ":("));
      else
         foreach (var differentChannelItem in differentchannel)
         foreach (var dC_UserdatatItem in dC_UserdataListAbo)
            if (!dC_UserdataListAboSorted.Contains(dC_UserdatatItem) && dC_UserdatatItem.ChannelId == differentChannelItem)
               dC_UserdataListAboSorted.Add(dC_UserdatatItem);

      foreach (var dC_UserdataItem in dC_UserdataListAboSorted)
      {
         if (lastChannel == 0)
            lastChannel = dC_UserdataItem.ChannelId;

         var serverInfoName = serverInfoList.First(firstMatch => firstMatch.ServerInfoId == dC_UserdataItem.ServerInfoId).Name;

         var aboType = "FULL";
         if (dC_UserdataItem.IsMinimalAbo)
            aboType = "MINIMAL";

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
         if (!servers.Contains(serverInfoName))
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
            servers += $"{aboType} - {serverInfoName}\n";

         if (lastChannel != dC_UserdataItem.ChannelId)
         {
            discordEmbedBuilder.AddField(new DiscordEmbedField(servers, "<#" + lastChannel + ">"));
            servers = "";
            servers += $"{aboType} - {serverInfoName}\n";
            lastChannel = dC_UserdataItem.ChannelId;
         }

         if (dC_UserdataListAboSorted.Last() == dC_UserdataItem)
            discordEmbedBuilder.AddField(new DiscordEmbedField(servers, "<#" + lastChannel + ">"));
      }

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
   }

   /// <summary>
   ///    Testst the functionality of the DCChange [player, status, version]
   /// </summary>
   /// <param name="interactionContext">The interaction context.</param>
   [SlashCommand("test", "Test´s the functionality of the DCChange [player, status, version]", true)]
   public static async Task TestAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))] [Option("Server", "testserver")] string serverNameChoice, [ChoiceProvider(typeof(TestFunctionsChoiceProvider))] [Option("Function", "function")] string testFunctionChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      ServerNameChoiceProvider? serverNameChoiceProvider = new();
      var serverNameChoices = await serverNameChoiceProvider.Provider();

      TestFunctionsChoiceProvider? testFunctionChoiceProvider = new();
      var testFunctionChoices = await testFunctionChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
      var serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

      var serverInfoList = ServerInfo.ReadAll();
      ServerStat serverStatObj = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         serverStatObj = ServerStat.CreateObj(serverInfoItem);
         if (serverInfoItem.Name.ToLower() == serverName.ToLower())
            break;
      }

      if ("PLAYERCOUNT".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
         Bot.DCChange(serverStatObj, "player", false);
      else if ("ONLINESTATE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
         Bot.DCChange(serverStatObj, "status", false);
      else if ("VERSIONCHANGE".ToLower() == testFunctionChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == testFunctionChoice.ToLower()).Name.ToLower())
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
         Bot.DCChange(serverStatObj, "version", false);

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
      var botInvite = interactionContext.Client.GetInAppOAuth(Permissions.Administrator, OAuthScopes.BOT_MINIMAL);
      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent(botInvite.AbsoluteUri));
   }

   /// <summary>
   ///    Show´s every Server with their informations
   /// </summary>
   /// <param name="interactionContext">The interaction context.</param>
   [SlashCommand("42", "Show´s every Server with their informations")]
   public static async Task ShowServerStatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(FourtytwoTypeChoiceProvider))] [Option("Type", "Type")] string fourtytwoChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      var serverInfoList = ServerInfo.ReadAll();
      List<ServerStat> serverStatListLive = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         var serverStatObj = ServerStat.CreateObj(serverInfoItem);
         serverStatListLive.Add(serverStatObj);
      }

      FourtytwoTypeChoiceProvider? fourtytwoTypeChoiceProvider = new();
      var fourtytwoTypeChoices = await fourtytwoTypeChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
      if ("STATISTICS".ToLower() == fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower())
      {
         foreach (var serverStatItem in serverStatListLive)
         {
            DiscordEmbedBuilder discordEmbedBuilder = new();

            foreach (var serverInfoItem in serverInfoList)
               if (serverInfoItem.ServerInfoId == serverStatItem.ServerInfoId)
               {
                  serverInfoItem.QcUri = new Uri(QCUriGenerator.CreateObj(serverInfoItem).QCUri.AbsoluteUri);
                  discordEmbedBuilder.ImageUrl = serverInfoItem.QcUri.AbsoluteUri;
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
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
         var isFull = "FULL".ToLower() == fourtytwoTypeChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == fourtytwoChoice.ToLower()).Name.ToLower();
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

         foreach (var serverStatItem in serverStatListLive)
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

      var serverInfoList = ServerInfo.ReadAll();
      List<ServerStat> serverStatListLive = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         var serverStatObj = ServerStat.CreateObj(serverInfoItem);
         serverStatListLive.Add(serverStatObj);
      }

      DiscordEmbedBuilder discordEmbedBuilder = new() { Description = "This is the list for all registered servers", Color = new DiscordColor(255, 0, 255) };

      foreach (var serverStatItem in serverStatListLive)
      {
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
         discordEmbedBuilder.AddField(new DiscordEmbedField(serverStatItem.Name.ToUpper(), serverStatItem.Game.ToUpper()));
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
      }

      discordEmbedBuilder.WithThumbnail("https://i.imgur.com/2OqzCvU.png");
      discordEmbedBuilder.WithAuthor("StatFetch list");
      discordEmbedBuilder.WithFooter("(✿◠‿◠) thanks for using me");
      discordEmbedBuilder.WithTimestamp(DateTime.Now);

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
   }

   /// <summary>
   ///    Show´s status of a singel server
   /// </summary>
   /// <param name="interactionContext">The ctx.</param>
   /// <param name="serverNameChoice">The servers.</param>
   [SlashCommand("status", "Show´s status from a singel server")]
   public static async Task StatusAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))] [Option("Server", "status")] string serverNameChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      ServerNameChoiceProvider? serverNameChoiceProvider = new();
      var serverNameChoices = await serverNameChoiceProvider.Provider();
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
      var serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

      var serverInfoList = ServerInfo.ReadAll();
      ServerStat serverStatObj = new();
      ServerInfo serverInfo = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         serverInfo = serverInfoItem;
         if (serverInfoItem.Name.ToLower() == serverName.ToLower())
            break;
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
      }

      serverStatObj = ServerStat.CreateObj(serverInfo);

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
         discordEmbedBuilder.AddField(new DiscordEmbedField("Version", $"{serverStatObj.Version}", true));
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
   ///    Show´s the playerstatistics from a singel server
   /// </summary>
   /// <param name="interactionContext">The ctx.</param>
   /// <param name="serverNameChoice">The servers.</param>
   [SlashCommand("statistics", "Show´s the playerstatistics from a singel server")]
   public static async Task StatisticsAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))] [Option("Server", "statistics")] string serverNameChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      ServerNameChoiceProvider? serverNameChoiceProvider = new();
      var serverNameChoices = await serverNameChoiceProvider.Provider();
      var serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();

      var serverInfoList = ServerInfo.ReadAll();

      ServerStat serverStatObj = new();
      ServerInfo serverInfoObj = new();
      foreach (var serverInfoItem in serverInfoList)
      {
         if (serverInfoItem.Name.ToLower() == serverName.ToLower())
         {
            serverInfoObj = serverInfoItem;
            break;
         }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
      }

      serverStatObj = ServerStat.CreateObj(serverInfoObj);

      DiscordEmbedBuilder discordEmbedBuilder = new();

      var qcUri = QCUriGenerator.CreateObj(serverInfoObj).QCUri;
      if (qcUri != null) serverInfoObj.QcUri = new Uri(qcUri.AbsoluteUri);

      discordEmbedBuilder.ImageUrl = serverInfoObj.QcUri.AbsoluteUri;

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
   /// <param name="servers">The servers.</param>
   /// <param name="type">The type of subscription.</param>
   [SlashCommand("add", "Adds you to an subscription for a server")]
   public static async Task AddAboAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(ServerNameChoiceProvider))] [Option("Server", "adding")] string serverNameChoice, [ChoiceProvider(typeof(AboTypeChoiceProvider))] [Option("Type", "Type")] string aboTypeChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      ServerNameChoiceProvider? serverNameChoiceProvider = new();
      var serverNameChoices = await serverNameChoiceProvider.Provider();

      AboTypeChoiceProvider? aboStateChoiceProvider = new();
      var aboStateChoices = await aboStateChoiceProvider.Provider();

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
      var serverName = serverNameChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == serverNameChoice.ToLower()).Name.ToLower();
      var isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == aboTypeChoice.ToLower()).Name.ToLower();


      var discordEmbedBuilder = ChangeSubscriptionCommand(serverName, interactionContext, true, isMinimal);

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(discordEmbedBuilder.Build()));
   }

   /// <summary>
   ///    Adds you to every serversubscription
   /// </summary>
   /// <param name="interactionContext">The interaction context.</param>
   [SlashCommand("addall", "Adds you to every serversubscription")]
   public static async Task AddAllAsync(InteractionContext interactionContext, [ChoiceProvider(typeof(AboTypeChoiceProvider))] [Option("Type", "Type")] string aboTypeChoice)
   {
      await interactionContext.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
      DiscordEmbedBuilder discordEmbedBuilderLoading = new();
      discordEmbedBuilderLoading.WithDescription("Loading... ");
      await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilderLoading.Build()));

      AboTypeChoiceProvider? aboStateChoiceProvider = new();
      var aboStateChoices = await aboStateChoiceProvider.Provider();
      var isMinimal = "MINIMAL".ToLower() == aboStateChoices.First(firstMatch => firstMatch.Value.ToString().ToLower() == aboTypeChoice.ToLower()).Name.ToLower();

      var serverInfoList = ServerInfo.ReadAll();
      foreach (var serverInfoItem in serverInfoList)
      {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
         var discordEmbedBuilder = ChangeSubscriptionCommand(serverInfoItem.Name, interactionContext, true, isMinimal);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
         await interactionContext.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(discordEmbedBuilder.Build()));
      }

      await interactionContext.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Finished!"));
   }
}