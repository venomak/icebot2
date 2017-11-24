using System;
using IceBot;
using IceBot.Classes;
using IceBot.Classes.Discord;
using System.Threading.Tasks;

namespace IceBot
{
    class Program
    {

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            await DiscordMain.DiscordInit();

            await Classes.Sound.InsertSounds();

            await Task.Delay(-1);
        }
    }


}
