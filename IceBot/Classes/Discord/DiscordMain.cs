using System;
using DSharpPlus;
using IceBot.Classes;

using DSharpPlus.EventArgs;

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;

namespace IceBot.Classes.Discord
{
    public class DiscordMain
    {

        public static DiscordClient discCl;
        public static CommandsNextModule discCmds;
        public static InteractivityModule discInter;
        public static VoiceNextClient discVoice;
        public static DebugLogger Logger;

        public static async Task DiscordInit()
        {

            discCl = new DiscordClient(new DiscordConfiguration
            {
                Token = "MjgwNDk5MDI2NjI4MDUwOTQ2.DPiKFw.cAR1XJu4dEXPeNMhYb9ZJcpL9eQ",
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Info,
                DateTimeFormat = "MM-dd-yyyy hh:mm:ss",
                UseInternalLogHandler = true
            });

            //Easier logging path
            Logger = discCl.DebugLogger;

            discCl.GuildMemberAdded += Events.GuildMemberAdded;
            discCl.GuildMemberRemoved += Events.GuildMemberRemoved;
            discCl.GuildMemberUpdated += Events.GuildMemberUpdated;

            discCl.GuildAvailable += Events.GuildAvailable;
            discCl.GuildUnavailable += Events.GuildUnavailable;
            discCl.GuildUpdated += Events.GuildUpdated;

            discCl.MessageAcknowledged += Events.MessageAcknowledged;

            discCl.MessageDeleted += Events.MessageDeleted;
            discCl.MessagesBulkDeleted += Events.MessagesBulkDeleted;

            discCl.PresenceUpdated += Events.PresenceUpdated;

            discCl.Ready += Events.Ready;

            discCl.Resumed += Events.Resumed;

            discCl.TypingStarted += Events.TypingStart;

            discCl.VoiceServerUpdated += Events.VoiceServerUpdated;
            discCl.VoiceStateUpdated += Events.VoiceStateUpdated;


            discCl.MessageCreated += Events.MessageCreated;

            discCmds = discCl.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = ".",
                EnableDms = false
            });

            //discCmds.CommandExecuted += Events.CommandExecuted;
            //discCmds.CommandErrored += Events.CommandErrored;

            discCmds.RegisterCommands<Commands>();
            discCmds.RegisterCommands<SoundCommands>();
            discCmds.RegisterCommands<ExampleExecutableGroup>();
            discCmds.RegisterCommands<ExampleGrouppedCommands>();

            discInter = discCl.UseInteractivity(new InteractivityConfiguration());

            discVoice = discCl.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = true
            });

            await discCl.ConnectAsync();

        }
    }
}
