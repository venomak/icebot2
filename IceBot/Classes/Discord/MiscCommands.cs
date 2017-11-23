using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


using SteamGaugesApi.Core;
using SteamGaugesApi.Core.Models;
using DSharpPlus.Entities;

namespace IceBot.Classes.Discord
{
    public class MiscCommands
    {

        [Command("hi")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
            var interactivity = ctx.Client.GetInteractivityModule();
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id && xm.Content.ToLower() == "how are you?", TimeSpan.FromMinutes(1));
            if (msg != null)
                await ctx.RespondAsync($"I'm fine, thank you!");
        }

        [Command("test")]
        public async Task Test(CommandContext ctx)
        {
            var client = new SteamGaugesApi.Core.Client();
            var response = client.Get();

            var dotaInter = response.SteamGameCoordinationInterface.DotaGameCoordinatorInterface;
            var dotaStatus = Global.OnlineOffline(dotaInter.Online);

            var clientStatus = Global.OnlineOffline(response.SteamClientInterface.Online);
            var communityStatus = Global.OnlineOffline(response.SteamCommunity.Online);
            var storeStatus = Global.OnlineOffline(response.SteamStore.Online);
            var userStatus = Global.OnlineOffline(response.SteamUserInterface.Online);

            var dotaError = dotaInter.Error;
            
            
           

            var dotaStats = dotaInter.Statistics;

            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", $"Steam Community is online: '{dotaStats.PlayersSearching.Value}'", DateTime.Now);

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Steam Statistics"
               
            };

            //builder.AddField("Client", clientStatus, true);
            builder.AddField("Community", communityStatus, true);
            builder.AddField("Store", storeStatus, true);

            builder.AddField("======", " ");

            //builder.AddField("User", userStatus, true);
            //builder.AddField("Client", clientStatus, true);
            //builder.AddField("Client", clientStatus, false);
            //builder.AddField("Online Players", dotaStats.PlayersOnline.ToString(), true);


            await ctx.RespondAsync(null, false, builder.Build());
            
            await Task.Delay(0);
        }


    }
}
