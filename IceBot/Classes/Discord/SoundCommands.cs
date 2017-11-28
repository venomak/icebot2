using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IceBot.Classes.Discord
{
    [Group("sound")]
    public class SoundCommands
    {
        [Command("new"), Aliases("n")]
        [Description("Shows a list of new sounds.")]
        public async Task NewSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            DiscordMain.Logger.LogMessage(DSharpPlus.LogLevel.Info, "SOUNDS", "NEW SOUNDS", DateTime.Now);
            var newSounds = SoundFuncs.soundNewFunc(ctx);

            await ctx.RespondAsync(newSounds);
        }

        [Command("list")]
        [Description("Shows a list of sounds.")]
        public async Task ListSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            var listSounds = SoundFuncs.soundListFunc(ctx);

            await ctx.RespondAsync(listSounds);
        }

        [Command("nlist"), Aliases("nl")]
        [Description("Shows a list of sounds.")]
        public async Task NewListSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            await SoundFuncs.newSoundListFunc(ctx);

            await Task.Delay(0);
        }

        [Command("play")]
        [Description("Play a sound.")]
        public async Task PlaySound(CommandContext ctx, [RemainingText]string text = "")
        {
            await SoundFuncs.soundPlayFunc(ctx, true);

            await Task.Delay(0);
        }

        [Command("search")]
        [Description("Search the sounds for a specific one.")]
        public async Task SearchSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            var searchSounds = SoundFuncs.soundSearchFunc(ctx);

            await ctx.RespondAsync(searchSounds);
        }

        [Command("recent")]
        [Description("Shows a list of recently played.")]
        public async Task RecentSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            var recentSounds = SoundFuncs.soundRecentFunc(ctx);

            await ctx.RespondAsync(recentSounds);
        }

        [Command("queue")]
        [Description("Shows a list of queued sounds.")]
        public async Task QueuedSounds(CommandContext ctx, [RemainingText]string text = "")
        {
            var queuedSounds = SoundFuncs.soundQueueFunc(ctx);

            await ctx.RespondAsync(queuedSounds);
        }

    }

}
