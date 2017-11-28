using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using IceBot.Classes.Discord;

namespace IceBot.Classes.Discord
{
    public class Events
    {

        public static async Task MessageCreated(MessageCreateEventArgs args)
        {
            if (args.Message.Content.ToLower().StartsWith("ping"))
                await args.Message.RespondAsync("pong!");

        }

        public static async Task GuildMemberAdded(GuildMemberAddEventArgs args)
        {

            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Member Added", DateTime.Now);


            await Task.Delay(0);
        }

        public static async Task GuildMemberRemoved(GuildMemberRemoveEventArgs args)
        {

            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Member Removed", DateTime.Now);


            await Task.Delay(0);
        }

        public static async Task GuildMemberUpdated(GuildMemberUpdateEventArgs args)
        {

            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Member Updated", DateTime.Now);


            await Task.Delay(0);
        }

        public static async Task GuildAvailable(GuildCreateEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Available", DateTime.Now);


            await Task.Delay(0);
        }

        public static async Task GuildUnavailable(GuildDeleteEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Unavailable", DateTime.Now);

            await Task.Delay(0);
        }
        public static async Task GuildUpdated(GuildUpdateEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Guild Updated", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task MessageAcknowledged(MessageAcknowledgeEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Message Acknowledged", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task MessageDeleted(MessageDeleteEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Message Deleted", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task MessagesBulkDeleted(MessageBulkDeleteEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Message Bulk Deleted", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task PresenceUpdated(PresenceUpdateEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", $"Presence Updated -- {args.Member.Nickname} -- {args.PresenceBefore} -- {args.Status}", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task Ready(ReadyEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Ready", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task Resumed(ReadyEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", "Resumed", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task TypingStart(TypingStartEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", $"'{args.User.Username}' started typing.", DateTime.Now);


            await Task.Delay(0);
        }

        public static async Task VoiceServerUpdated(VoiceServerUpdateEventArgs args)
        {
           DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", $"Voice Server Updated --", DateTime.Now);

            await Task.Delay(0);
        }

        public static async Task VoiceStateUpdated(VoiceStateUpdateEventArgs args)
        {
            DiscordMain.discCl.DebugLogger.LogMessage(LogLevel.Info, "DISCORD", $"Voice State Updated --", DateTime.Now);

            await Task.Delay(0);
        }

        //public static async Task CommandExecuted(CommandExecutionEventArgs args)
        //{

        //}

        //public static async Task CommandErrored(CommandExecutionEventArgs args)
        //{


        //}

    }
}
