using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace IceBot.Classes.Discord
{
    public class SoundFuncs
    {


        public static string soundListFunc(CommandContext context)
        {


            string[] soundCats = Classes.Sound.listSoundCats();

            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);

            Console.WriteLine("SOUND CATEGORIES: {0} :: {1}", soundCats.Length, args.Count);

            Global.PrintDictTable(args);

            string ret = "";


            if (soundCats.Length > 0)
            {

                if (args.Count >= 3)
                {
                    //Arguments...
                    int pg = -1;

                    var isInt = int.TryParse(args[2], out pg);

                    if (isInt == false)
                    {
                        int[] srchCat = Classes.Sound.srchSoundCatsLoose(args[2]);


                        Console.WriteLine("SRCH CAT: {0} : {1}", srchCat[0], srchCat[1]);

                        if (srchCat[0] == 1)
                        {
                            //Get sound view.
                            var soundView = 0;
                            Console.WriteLine("HELLO 1");
                            string[] catSounds = Classes.Sound.listSounds(srchCat[1], soundView);
                            Console.WriteLine("HELLO 2");
                            if (args.Count == 3)
                            {
                                Console.WriteLine("HELLO 3");
                                ret = catSounds[0];
                                Console.WriteLine("HELLO 4");
                            }
                            else if (args.Count == 4)
                            {
                                int pg2 = -1;

                                var isInt2 = int.TryParse(args[3], out pg2);

                                //Console.WriteLine("PG2: {0} : {1}", pg2, catSounds.Length);

                                if (isInt2 == true)
                                {

                                    if (pg2 >= 1 && pg2 <= catSounds.Length)
                                    {
                                        ret = catSounds[pg2 - 1];
                                    }
                                    else
                                    {
                                        ret = "Page must be a number between 1 and " + catSounds.Length.ToString() + ".";
                                        return ret;
                                    }

                                }
                                else
                                {
                                    ret = "Page must be a number between 1 and " + catSounds.Length.ToString() + ".";
                                    return ret;
                                }

                            }

                        }
                        else
                        {
                            //Console.WriteLine("SRCH CAT IS 0  - {0} :: {1}", srchCat[0], srchCat[1]);

                            ret = "That sound category does not exist.";
                            return ret;

                        }

                    }
                    else
                    {
                        //Console.WriteLine("PG: {0}", pg);

                        if (pg >= 1 && pg <= soundCats.Length)
                        {
                            ret = soundCats[pg - 1];
                        }
                        else
                        {
                            ret = "Page must be a number between 1 and " + soundCats.Length.ToString() + ".";
                            return ret;
                        }

                    }

                }
                else
                {
                    //No arguments.
                    //Console.WriteLine("NO ARGUMENTS");
                    ret = soundCats[0];
                }

            }
            else
            {
                ret = "There are no sound categories!";
                return ret;
            }

            return ret;
        }

        public static string soundSearchFunc(CommandContext context)
        {
            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);


            string ret = "";

            //Console.WriteLine("SOUND SEARCH ARGS: {0}   -- ARGS LENGTH: {1}", args[1], args.Length);

            if (args.Count >= 3)
            {
                string[] srchSounds = Classes.Sound.searchSounds(args[2]);


                if (args.Count == 4)
                {
                    int pg = -1;

                    try
                    {
                        pg = Convert.ToInt32(args[3]);
                    }
                    catch (FormatException e)
                    {
                        ret = "Page must be a number between 1 and " + srchSounds.Length.ToString() + ".";
                        return ret;
                    }

                    if (pg != -1)
                    {
                        ret = srchSounds[pg - 1];
                    }
                    else
                    {
                        ret = "Page must be a number between 1 and " + srchSounds.Length.ToString() + ".";
                        return ret;
                    }

                }
                else
                {
                    ret = srchSounds[0];
                }
            }
            else
            {
                ret = "You must specify something to search for. (Ex. '.sound search good')";
                return ret;
            }

            return ret;
        }

        public static async Task soundPlayFunc(CommandContext context)
        {
            var vnext = context.Client.GetVoiceNextClient();

            var vnc = vnext.GetConnection(context.Guild);
            if (vnc == null)
            {
                await context.RespondAsync("Must be connected to a voice server to play sounds!");
            }
            else
            {
                //Combine all args after 0;
                string soundName = "";
                int i = 0;

                Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);

                if (args.Count > 2)
                {
                    args.Remove(0);
                    args.Remove(1);
                } else
                {
                    args.Remove(0);
                }


                foreach (string str in args.Values)
                {

                    if (i == 0)
                    {
                        soundName = str;
                    }
                    else
                    {
                        soundName = soundName + " " + str;
                    }

                    i += 1;
                }

                Console.WriteLine("SOUNDNAME: {0}", soundName);


                DataTable ret = Sound.srchSounds("snd_aliases", soundName);

                if (ret.Rows.Count != 0)
                {
                    DataRow sndRow = ret.Rows[0];

                    if (sndRow["snd_ignore"].ToString() != "1")
                    {

                        //Log("SOUNDS: Playing " + soundFuncs.g_soundFiles[ret].ToString());
                        string[] spl = Sound.splitSoundFilename(sndRow["snd_filename"].ToString());

                        Console.WriteLine($"SPL : {spl[0]} -- {spl[1]} --");
                        // string path = "files\\sounds\\" + Sound.soundData["snd_filename"].ToString() + ".mp3";

                        Console.WriteLine($"PATH: {sndRow["snd_filename"]}");

                        string porq = "";

                        porq = "played the sound";

 
                        string who = context.User.Mention;

                        string sayMsg = who + " " + porq + " '" + spl[1] + "'";

                        await context.RespondAsync(sayMsg);

                        //await context.RespondAsync("👌");
                        await vnc.SendSpeakingAsync(true); // send a speaking indicator

                        await Playback.PlaySound($"files/sounds/{sndRow["snd_filename"]}.mp3", context.Guild);

                        await vnc.SendSpeakingAsync(false); // we're not speaking anymore
                    }
                    else
                    {
                        await context.RespondAsync("This sound is ignored! You cannot play it.");
                    }

                }
                else
                {
                    Console.WriteLine("SOUNDNAME: {0}", soundName);

                    //Send error message saying sound not found, find similar sounds.
                    string srchSim = Sound.searchSimilar(soundName);

                    //Console.WriteLine("SIMILAR: {0}", srchSim.Length);

                    if (srchSim.Length != 0)
                    {
                        await context.RespondAsync("Invalid sound name!\nSuggested: " + srchSim + "");
                    }
                    else
                    {
                        await context.RespondAsync("Invalid sound name!");
                    }

                }
            }
        }

        public static string soundNewFunc(CommandContext context)
        {
            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);

            string ret = "";

            int pg = 0;

            string[] newSounds = Classes.Sound.listNewSounds();

            if (args.Count == 3)
            {
                try
                {
                    pg = Convert.ToInt32(args[2]);
                    pg = pg - 1;
                }
                catch
                {
                    ret = "Page must be a number between 1 and " + newSounds.Length.ToString() + ".";
                }
            }

            ret = newSounds[pg];


            return ret;
        }


        public static string soundQueueFunc(CommandContext context)
        {
            Console.WriteLine("== Sound Queue Func ==");

            int pg = 0;

            string ret = "";

            string[] quSounds = Classes.Sound.listSoundQueued();
            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);

            if (args.Count == 3)
            {
                try
                {
                    pg = Convert.ToInt32(args[2]);
                    pg = pg - 1;
                }
                catch
                {
                    ret = "Page must be a number between 1 and " + quSounds.Length.ToString() + ".";
                    return ret;
                }
            }

            ret = quSounds[pg];

            return ret;
        }

        public static string soundRecentFunc(CommandContext context)
        {
            Console.WriteLine("== Sound Recent Func ==");

            int pg = 0;

            string ret = "";

            string[] reSounds = Classes.Sound.listSoundRecent();
            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);

            if (args.Count == 3)
            {
                try
                {
                    pg = Convert.ToInt32(args[2]);
                    pg = pg - 1;
                }
                catch
                {
                    ret = "Page must be a number between 1 and " + reSounds.Length.ToString() + ".";
                    return ret;
                }
            }

            if (reSounds.Length >= 3)
            {
                ret = reSounds[pg];
            }
            else
            {
                ret = "== Recently Played Sounds == \n- No recent sounds!";
                return ret;
            }

            return ret;
        }




    }
}
