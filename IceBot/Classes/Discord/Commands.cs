using System;
using System.IO;
using System.Diagnostics;

using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;
using System.Collections.Generic;

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

            if (chn != null)
            {
                vnc = await vnext.ConnectAsync(chn);
                await ctx.RespondAsync($"Joined '{chn.Name}'");
            }

        }

        [Command("leave")]
        public async Task Leave(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc == null)
                throw new InvalidOperationException("Not connected in this guild.");
            var chn = ctx.Member?.VoiceState?.Channel;

            vnc.Disconnect();
            await ctx.RespondAsync($"Left '{chn.Name}'");
        }


        [Command("play"), Aliases("s")]
        public async Task Play(CommandContext ctx)
        {
            await SoundFuncs.soundPlayFunc(ctx, true);

            await Task.Delay(0);

        }


        #region "Misc Funcs"

        static Random r = new Random();



        static Dictionary<int, string> timeZones = new Dictionary<int, string> {
            { -10, "HST - Honolulu" },
            { -8, "PST - San Francisco" },
            { -7, "MST - Denver" },
            { -6, "CST - Chicago" },
            { -5, "EST - New York" },
            { -2, "BRST - Rio de Janiero" },
            { 0, "GMT - London" },
            { 1, "CET - Vienna" },
            { 5, "IST - Mumbai" },
            { 8, "SGT - Singapore" },
            { 9, "JST - Tokyo" },
            { 11, "AEDT - Sydney" },
            { 13, "NZDT - Auckland" }
        };



        private static Dictionary<int, string> GetTimes()
        {
            Dictionary<int, string> ret = new Dictionary<int, string>();

            int i = 0;

            foreach (int k in timeZones.Keys)
            {
                DateTime now = DateTime.UtcNow;
                //Console.WriteLine("HOURS: {0}", k);
                now = now.AddHours(k);

                if (k == 5)
                {
                    now = now.AddMinutes(30);
                }

                ret.Add(k, timeZones[k] + " --  " + Global.FormatHrMin(now) + "\n ----");

                i++;
            }

            return ret;
        }


        public static string timeFunc(CommandContext context)
        {
            Dictionary<int, string> times = GetTimes();

            string msg = "```==== Time ====\n";
            foreach (string str in times.Values)
            {
                msg = msg + str + "\n";
            }

            msg = msg + "```";

            return msg;
        }


        public static string rollFunc(CommandContext context)
        {
            r.Next();


            string who = context.User.Mention;
            string msgContent = context.Message.Content;

            Console.WriteLine("MSG CONTENT: {0}", msgContent);

            Dictionary<int, string> hndlArgs = Global.HandleArgs(msgContent);

            string ret = "";

            if (hndlArgs.Count == 1)
            {
                //Regular 1-100 roll.
                int randNum = r.Next(1, 100);

                string msg = string.Format("{0} rolled a '{1}' !   (1 - 100)", who, randNum);

                ret = msg;
            }
            else if (hndlArgs.Count == 2)
            {
                //Roll with max number set only.
                int maxNum = -1;

                try
                {
                    maxNum = Convert.ToInt32(hndlArgs[1]);
                }
                catch
                {

                }

                if (maxNum > -1)
                {
                    int randNum = r.Next(1, maxNum);

                    string msg = string.Format("{0} rolled a '{1}' !   (1 - {2})", who, randNum, maxNum);

                    ret = msg;
                }
                else
                {
                    //Invalid max number.
                    ret = "Maximum must be a number!";
                }

            }
            else if (hndlArgs.Count == 3)
            {
                //Roll with min and max set.
                int minNum = -1;
                int maxNum = -1;

                try
                {
                    minNum = Convert.ToInt32(hndlArgs[1]);
                    maxNum = Convert.ToInt32(hndlArgs[2]);
                }
                catch
                {

                }

                if (minNum > -1 && maxNum > -1)
                {
                    int randNum = r.Next(minNum, maxNum);

                    string msg = string.Format("{0} rolled a '{1}' !   ({2} - {3})", who, randNum, minNum, maxNum);

                    ret = msg;
                }
                else
                {
                    ret = "Minimum and maximum must be numbers!";
                }


            }

            return ret;
            //else if (args.Count == 4)
            //{
            //	//Roll with min max and number of times to roll.
            //	int minNum = -1;
            //	int maxNum = -1;
            //	int rollNum = -1;
            //	try
            //	{
            //		minNum = Convert.ToInt32(args[1]);
            //		maxNum = Convert.ToInt32(args[2]);
            //		rollNum = Convert.ToInt32(args[3]);

            //	}
            //	catch
            //	{

            //	}

            //	if (minNum > -1 && maxNum > -1 && rollNum > -1)
            //	{

            //	}
            //	else
            //	{

            //	}

            //}


        }

        public static string coinFunc(CommandContext context)
        {
            r.Next();

            string who = context.User.Mention;

            int randNum = r.Next(1, 100);
            string coin = "Heads";

            if (randNum >= 1 && randNum <= 49)
            {
                coin = "Tails";
            }

            string msg = string.Format("{0} flipped a coin! It is '{1}' !", who, coin);

            return msg;
        }

        public static string ballFunc(CommandContext context)
        {
            r.Next();

            int randNum = r.Next(1, 20);
            string result = "";

            switch (randNum)
            {
                case 1:
                    result = "It is certain.";
                    break;
                case 2:
                    result = "It is decidedly so.";
                    break;
                case 3:
                    result = "Without a doubt.";
                    break;
                case 4:
                    result = "Yes, definitely.";
                    break;
                case 5:
                    result = "You may rely on it.";
                    break;
                case 6:
                    result = "As I see it, yes.";
                    break;
                case 7:
                    result = "Most likely.";
                    break;
                case 8:
                    result = "Outlook good.";
                    break;
                case 9:
                    result = "Yes.";
                    break;
                case 10:
                    result = "Signs point to yes.";
                    break;
                case 11:
                    result = "Reply hazy try again.";
                    break;
                case 12:
                    result = "Ask again later.";

                    break;
                case 13:
                    result = "Better not tell you now.";

                    break;
                case 14:
                    result = "Cannot predict now.";

                    break;
                case 15:
                    result = "Concentrate and ask again.";

                    break;
                case 16:
                    result = "Don't count on it.";

                    break;
                case 17:
                    result = "My reply is no.";

                    break;
                case 18:
                    result = "My sources say no.";

                    break;
                case 19:
                    result = "Outlook not so good.";

                    break;
                case 20:
                    result = "Very doubtful.";

                    break;
            }

            string msg = string.Format("The Magic 8-Ball says '{0}'", result);

            return msg;

        }

        #endregion


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
            builder.AddField("Store ", storeStatus, true);

            builder.AddField("======", " ");

            //builder.AddField("User", userStatus, true);
            //builder.AddField("Client", clientStatus, true);
            //builder.AddField("Client", clientStatus, false);
            //builder.AddField("Online Players", dotaStats.PlayersOnline.ToString(), true);


            await ctx.RespondAsync(null, false, builder.Build());

            await Task.Delay(0);
        }

        [Command("flip")]
        [Description("Flip a coin.")]
        public async Task FlipCoin(CommandContext ctx, [RemainingText]string text = "")
        {
            var flipCoin = coinFunc(ctx);

            await ctx.TriggerTypingAsync();

            await ctx.RespondAsync(flipCoin);
        }


        [Command("8ball")]
        [Description("Asks the Magic 8-Ball a question.")]
        public async Task EightBall(CommandContext ctx, [RemainingText]string text = "")
        {
            var eightBall = ballFunc(ctx);

            await ctx.RespondAsync(eightBall);
        }

        [Command("times")]
        [Description("Shows the current time in all timezones.")]
        public async Task TimeZones(CommandContext ctx, [RemainingText]string text = "")
        {
            var timeZones = timeFunc(ctx);

            await ctx.RespondAsync(timeZones);
        }

        [Command("roll")]
        [Description("Rolls a die")]
        public async Task Roll(CommandContext ctx, [RemainingText]string text = "")
        {
            var roll = rollFunc(ctx);

            await ctx.RespondAsync(roll);
        }


        [Command("sendpaginated"), Description("Sends a paginated message.")]
        public async Task SendPaginated(CommandContext ctx)
        {
            // first retrieve the interactivity module from the client
            var interactivity = ctx.Client.GetInteractivityModule();

            // generate pages.
            var lipsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris vitae velit eget nunc iaculis laoreet vitae eu risus. Nullam sit amet cursus purus. Duis enim elit, malesuada consequat aliquam sit amet, interdum vel orci. Donec vehicula ut lacus consequat cursus. Aliquam pellentesque eleifend lectus vitae sollicitudin. Vestibulum sit amet risus rhoncus, hendrerit felis eget, tincidunt odio. Nulla sed urna ante. Mauris consectetur accumsan purus, ac dignissim ligula condimentum eu. Phasellus ullamcorper, arcu sed scelerisque tristique, ante elit tincidunt sapien, eu laoreet ipsum mauris eu justo. Curabitur mattis cursus urna, eu ornare lacus pulvinar in. Vivamus cursus gravida nunc. Sed dolor nisi, congue non hendrerit at, rutrum sed mi. Duis est metus, consectetur sed libero quis, dignissim gravida lacus. Mauris suscipit diam dolor, semper placerat justo sodales vel. Curabitur sed fringilla odio.\n\nMorbi pretium placerat nulla sit amet condimentum. Duis placerat, felis ornare vehicula auctor, augue odio consectetur eros, sit amet tristique dolor risus nec leo. Aenean vulputate ipsum sagittis augue malesuada, id viverra odio gravida. Curabitur aliquet elementum feugiat. Phasellus eu faucibus nibh, eget finibus nibh. Proin ac fermentum enim, non consequat orci. Nam quis elit vulputate, mollis eros ut, maximus lacus. Vivamus et lobortis odio. Suspendisse potenti. Fusce nec magna in eros tempor tincidunt non vel mi. Pellentesque auctor eros tellus, vel ultrices mi ultricies eu. Nam pharetra sed tortor id elementum. Donec sit amet mi eleifend, iaculis purus sit amet, interdum turpis.\n\nAliquam at consectetur lectus. Ut et ultrices augue. Etiam feugiat, tortor nec dictum pharetra, nulla mauris convallis magna, quis auctor libero ipsum vitae mi. Mauris posuere feugiat feugiat. Phasellus molestie purus sit amet ipsum sodales, eget pretium lorem pharetra. Quisque in porttitor quam, nec hendrerit ligula. Fusce tempus, diam ut malesuada semper, leo tortor vulputate erat, non porttitor nisi elit eget turpis. Nam vitae arcu felis. Aliquam molestie neque orci, vel consectetur velit mattis vel. Fusce eget tempus leo. Morbi sit amet bibendum mauris. Aliquam erat volutpat. Phasellus nunc lectus, vulputate vitae turpis vel, tristique vulputate nulla. Aenean sit amet augue at mauris laoreet convallis. Nam quis finibus dui, at lobortis lectus.\n\nSuspendisse potenti. Pellentesque massa enim, dapibus at tortor eu, posuere ultricies augue. Nunc condimentum enim id ex sagittis, ut dignissim neque tempor. Nulla cursus interdum turpis. Aenean auctor tempor justo, sed rhoncus lorem sollicitudin quis. Fusce non quam a ante suscipit laoreet eget at ligula. Aenean condimentum consectetur nunc, sit amet facilisis eros lacinia sit amet. Integer quis urna finibus, tristique justo ut, pretium lectus. Proin consectetur enim sed risus rutrum, eu vehicula augue pretium. Vivamus ultricies justo enim, id imperdiet lectus molestie at. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.\n\nNullam tincidunt dictum nibh, dignissim laoreet libero eleifend ut. Vestibulum eget maximus nulla. Suspendisse a auctor elit, ac facilisis tellus. Sed iaculis turpis ac purus tempor, ut pretium ante ultrices. Aenean commodo tempus vestibulum. Morbi vulputate pharetra molestie. Ut rhoncus quam felis, id mollis quam dapibus id. Curabitur faucibus id justo in ornare. Praesent facilisis dolor lorem, non vulputate velit finibus ut. Praesent vestibulum nunc ac nibh iaculis porttitor.\n\nFusce mattis leo sed ligula laoreet accumsan. Pellentesque tortor magna, ornare vitae tellus eget, mollis placerat est. Suspendisse potenti. Ut sit amet lacus sed nibh pulvinar mattis in bibendum dui. Mauris vitae turpis tempor, malesuada velit in, sodales lacus. Sed vehicula eros in magna condimentum vestibulum. Aenean semper finibus lectus, vel hendrerit lorem euismod a. Sed tempor ante quis magna sollicitudin, eu bibendum risus congue. Donec lectus sem, accumsan ut mollis et, accumsan sed lacus. Nam non dui non tellus pretium mattis. Mauris ultrices et felis ut imperdiet. Nam erat risus, consequat eu eros ac, convallis viverra sapien. Etiam maximus nunc et felis ultrices aliquam.\n\nUt tincidunt at magna at interdum. Sed fringilla in sem non lobortis. In dictum magna justo, nec lacinia eros porta at. Maecenas laoreet mattis vulputate. Sed efficitur tempor euismod. Integer volutpat a odio eu sagittis. Aliquam congue tristique nisi, quis aliquet nunc tristique vitae. Vivamus ac iaculis nunc, et faucibus diam. Donec vitae auctor ipsum, quis posuere est. Proin finibus, dolor ac euismod consequat, urna sem ultrices lectus, in iaculis sem nulla et odio. Integer et vulputate metus. Phasellus finibus et lorem eget lacinia. Maecenas velit est, luctus quis fermentum nec, fringilla eu lorem.\n\nPellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Mauris faucibus neque eu consectetur egestas. Mauris aliquet nibh pellentesque mollis facilisis. Duis egestas lectus sed justo sagittis ultrices. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Curabitur hendrerit quis arcu id dictum. Praesent in massa eget lectus pulvinar consectetur. Aliquam eget ipsum et velit congue porta vitae ut eros. Quisque convallis lacus et venenatis sagittis. Phasellus sit amet eros ac nibh facilisis laoreet vel eget nisi. In ante libero, volutpat in risus vel, tristique blandit leo. Morbi posuere bibendum libero, non efficitur mi sagittis vel. Cras viverra pulvinar pellentesque. Mauris auctor et lacus ut pellentesque. Nunc pretium luctus nisi eu convallis.\n\nSed nec ultricies arcu. Aliquam eu tincidunt diam, nec luctus ligula. Ut laoreet dignissim est, eu fermentum massa fermentum eget. Nullam non viverra justo, sed congue felis. Phasellus id convallis mauris. Aliquam elementum euismod ex, vitae dignissim nunc consectetur vitae. Donec ut odio quis ex placerat elementum sit amet eget lectus. Suspendisse potenti. Nam non massa id mi suscipit euismod. Nullam varius tincidunt diam congue congue. Proin pharetra vestibulum eros, vel imperdiet sem rutrum at. Cras eget gravida ligula, quis facilisis ex.\n\nEtiam consectetur elit mauris, euismod porta urna auctor a. Nulla facilisi. Praesent massa ipsum, iaculis non odio at, varius lobortis nisi. Aliquam viverra erat a dapibus porta. Pellentesque imperdiet maximus mattis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec luctus elit sit amet feugiat convallis. Phasellus varius, sem ut volutpat vestibulum, magna arcu porttitor libero, in dapibus metus dolor nec dolor. Fusce at eleifend magna. Mauris cursus pellentesque sagittis. Nullam nec laoreet ante, in sodales arcu.";
            var lipsum_pages = interactivity.GeneratePagesInEmbeds(lipsum);

            // send the paginator
            await interactivity.SendPaginatedMessage(ctx.Channel, ctx.User, lipsum_pages, TimeSpan.FromMinutes(5), TimeoutBehaviour.Delete);
        }


    }



    [Group("memes", CanInvokeWithoutSubcommand = true)] // this makes the class a group, but with a twist; the class now needs an ExecuteGroupAsync method
    [Description("Contains some memes. When invoked without subcommand, returns a random one.")]
    [Aliases("copypasta")]
    public class ExampleExecutableGroup
    {
        // commands in this group need to be executed as 
        // <prefix>memes [command] or <prefix>copypasta [command]

        // this is the group's command; unlike with other commands, 
        // any attributes on this one are ignored, but like other
        // commands, it can take arguments
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            // let's give them a random meme
            var rnd = new Random();
            var nxt = rnd.Next(0, 2);

            switch (nxt)
            {
                case 0:
                    await Pepe(ctx);
                    return;

                case 1:
                    await NavySeal(ctx);
                    return;

                case 2:
                    await Kekistani(ctx);
                    return;
            }
        }

        [Command("pepe"), Aliases("feelsbadman"), Description("Feels bad, man.")]
        public async Task Pepe(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Pepe",
                ImageUrl = "http://i.imgur.com/44SoSqS.jpg"
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("navyseal"), Aliases("gorillawarfare"), Description("What the fuck did you just say to me?")]
        public async Task NavySeal(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("What the fuck did you just fucking say about me, you little bitch? I’ll have you know I graduated top of my class in the Navy Seals, and I’ve been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I’m the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You’re fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that’s just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little “clever” comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn’t, you didn’t, and now you’re paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You’re fucking dead, kiddo.");
        }

        [Command("kekistani"), Aliases("kek", "normies"), Description("I'm a proud ethnic Kekistani.")]
        public async Task Kekistani(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync("I'm a proud ethnic Kekistani. For centuries my people bled under Normie oppression. But no more. We have suffered enough under your Social Media Tyranny. It is time to strike back. I hereby declare a meme jihad on all Normies. Normies, GET OUT! RRRÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆÆ﻿");
        }

        // this is a subgroup; you can nest groups as much 
        // as you like
        [Group("mememan", CanInvokeWithoutSubcommand = true), Hidden]
        public class MemeMan
        {
            public async Task ExecuteGroupAsync(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Meme man",
                    ImageUrl = "http://i.imgur.com/tEmKtNt.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("ukip"), Description("The UKIP pledge.")]
            public async Task Ukip(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "UKIP pledge",
                    ImageUrl = "http://i.imgur.com/ql76fCQ.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("lineofsight"), Description("Line of sight.")]
            public async Task LOS(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Line of sight",
                    ImageUrl = "http://i.imgur.com/ZuCUnEb.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("art"), Description("Art.")]
            public async Task Art(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Art",
                    ImageUrl = "http://i.imgur.com/VkmmmQd.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("seeameme"), Description("When you see a meme.")]
            public async Task SeeMeme(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "When you see a meme",
                    ImageUrl = "http://i.imgur.com/8GD0hbZ.jpg"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("thisis"), Description("This is meme man.")]
            public async Task ThisIs(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "This is meme man",
                    ImageUrl = "http://i.imgur.com/57vDOe6.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("deepdream"), Description("Deepdream'd meme man.")]
            public async Task DeepDream(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Deep dream",
                    ImageUrl = "http://i.imgur.com/U666J6x.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("sword"), Description("Meme with a sword?")]
            public async Task Sword(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Meme with a sword?",
                    ImageUrl = "http://i.imgur.com/T3FMXdu.png"
                };
                await ctx.RespondAsync(embed: embed);
            }

            [Command("christmas"), Description("Beneath the christmas spike...")]
            public async Task ChristmasSpike(CommandContext ctx)
            {
                await ctx.TriggerTypingAsync();

                // wrap it into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Christmas spike",
                    ImageUrl = "http://i.imgur.com/uXIqUS7.png"
                };
                await ctx.RespondAsync(embed: embed);
            }
        }
    }


    [Group("admin")] // let's mark this class as a command group
    [Description("Administrative commands.")] // give it a description for help purposes
    [Hidden] // let's hide this from the eyes of curious users
    [RequirePermissions(Permissions.ManageGuild)] // and restrict this to users who have appropriate permissions
    public class ExampleGrouppedCommands
    {
        // all the commands will need to be executed as <prefix>admin <command> <arguments>

        [Command("nick"), Description("Gives someone a new nickname."), RequirePermissions(Permissions.ManageNicknames)]
        public async Task ChangeNickname(CommandContext ctx, [Description("Member to change the nickname for.")] DiscordMember member, [RemainingText, Description("The nickname to give to that user.")] string new_nickname)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();
            Console.WriteLine($"MEMBER: '{member}'");
            try
            {
                // let's change the nickname, and tell the 
                // audit logs who did it.

                Console.WriteLine($"NEW NAME: {new_nickname}");
                
                await member.ModifyAsync(new_nickname, reason: $"Changed by {ctx.User.Username} ({ctx.User.Id}).");

                // let's make a simple response.
                var emoji = DiscordEmoji.FromName(ctx.Client, ":+1:");

                // and respond with it.
                await ctx.RespondAsync(emoji);
            }
            catch (Exception)
            {
                // oh no, something failed, let the invoker now
                var emoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await ctx.RespondAsync(emoji);
            }
        }

        [Command("clearmsgs"), Description("Clears messages in a channel."), RequirePermissions(Permissions.ManageMessages)]
        public async Task ClearMsgs(CommandContext ctx, [Description("Text channel to delete messages from.")] DiscordChannel channel, [RemainingText, Description("Number of messages to delete.")] int cnt = 5)
        {
            if (cnt < 10)
            {
                IReadOnlyList<DiscordMessage> msgList = await channel.GetMessagesAsync(cnt);

                await channel.DeleteMessagesAsync(msgList);

            }
            else
            {
                int i = 0;

                while(i < cnt)
                {
                    var comp = cnt - i;
                    IReadOnlyList<DiscordMessage> msgList;

                    if (comp < 10)
                    {
                        msgList = await channel.GetMessagesAsync(comp);
                        i += comp;
                    }
                    else
                    {
                        msgList = await channel.GetMessagesAsync(10);
                        i += 10;
                    }

                    await channel.DeleteMessagesAsync(msgList);
                }

            }
            await Task.Delay(0);
        }
    }


}
