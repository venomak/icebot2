using System;
using System.IO;
using System.Diagnostics;

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;

namespace IceBot.Classes.Discord
{
    public class Commands
    {

        [Command("join")]
        public async Task Join(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);

            if(vnc != null)
            {
                vnc.Disconnect();

            }

            var chn = ctx.Member?.VoiceState?.Channel;
            if (chn == null)
                throw new InvalidOperationException("You need to be in a voice channel.");

            vnc = await vnext.ConnectAsync(chn);
            await ctx.RespondAsync("👌");

        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            vnc.Disconnect();
            await ctx.RespondAsync("👌");
        }


        [Command("play")]
        public async Task Play(CommandContext ctx, [RemainingText] string file)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");

            if(vnc.IsPlaying == true)
            {
                await Playback.StopSound();
            }

            if (!File.Exists(file))
                throw new FileNotFoundException("File was not found.");

            await ctx.RespondAsync("👌");
            await vnc.SendSpeakingAsync(true); // send a speaking indicator

            await Playback.PlaySound(file, ctx.Guild);

            await vnc.SendSpeakingAsync(false); // we're not speaking anymore
            

        }

    }
}
