using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using IceBot.Classes;
using VideoLibrary;
//using HundredMilesSoftware.UltraID3Lib;
using DSharpPlus.CommandsNext;
using System.Data;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using System.Diagnostics;

namespace IceBot.Classes
{

    public class DownloadFuncs
    {

        [Command("download"), Aliases("down", "dl")]
        public async Task downloadFunc(CommandContext context, [Description("Specify whether this is a sound clip or a song.")] string soundormusic, [Description("If a sound, specify the category, if music specify Artist name.")] string category, [Description("If a sound, this is the alias or name of it. If music, this is the Title of the track.")] string aliasortitle, [Description("The Youtube video's ID. (Ex. dQw4w9WgXcQ")] string youtubeID)
        {

            Dictionary<int, string> args = Global.HandleArgs(context.Message.Content);
            Global.PrintDictTable(args);

            
            if (args.Count == 5)
            {
                //Console.WriteLine("DOWNLOAD VIDEO NULL");

                // Check arguments.
                if (args[1] == "sound" || args[1] == "s" || args[1] == "music" || args[1] == "m")
                {
                    int type = -1;

                    if (args[1] == "sound" || args[1] == "s")
                    {
                        type = 0;
                    }
                    else
                    {
                        type = 1;
                    }
                    //1 sound-music
                    //2 sound category-artist
                    //3 sound alias-title
                    //4 link

                    bool error = false;
                    bool catex = true;

                    if (type == 0)
                    {
                        //Check to see if category name has any weird characters.

                        //Check sound cat.
                        int[] srchCat = Sound.srchSoundCatsLoose(args[2]);

                        //Console.WriteLine("INT: {0} : {1}", srchCat[0], srchCat[1]);
                        if (srchCat[0] == 0)
                        {
                            //Not exist, make it?
                            //Console.WriteLine("CAT EXIST FALSE");
                            catex = false;
                        }
                        else
                        {
                            //Exists, add it.
                            //Check sound alias exist.
                            DataTable srch = Sound.srchSounds("snd_aliases", args[3]);

                            if (srch.Rows.Count != 0)
                            {
                                //Exists already, show error.
                                await context.RespondAsync("A sound with that alias already exists, try a more unique name.");
                                error = true;
                            }
                                

                        }

                        //Console.WriteLine("ERR EX: {0} : {1}", error, catex);

                        if (error == false)
                        {
                            string catFolder = "";

                            if (catex == false)
                            {
                                //Create directory with formatted name.
                                catFolder = Sound.stripSoundDir(args[2]);
                                Directory.CreateDirectory("files\\sounds\\" + catFolder);

                                //Add to sound cats.
                                Sound.soundCats.Add(Sound.soundCats.Count, Sound.formatSoundDir(catFolder));
                            }
                            else
                            {
                                //Download from YouTube to folder.

                                //Get sound category folder.
                                catFolder = Sound.stripSoundDir(Sound.soundCats[srchCat[1]]);
                            }

                            Console.WriteLine("CATFOLDER: {0}", "sounds\\" + catFolder);

                            //Last in queue's title.
                            var yt = YouTube.Default;
                            YouTubeVideo video;

                            try
                            {
                                video = yt.GetVideo("https://www.youtube.com/watch?v=" + args[4]);
                            }
                            catch
                            {
                                video = null;
                            }

                            if (video != null)
                            {

                                if (video.IsEncrypted == false)
                                {

                                    if (Download.downQueue.Count > 0)
                                    {
                                        await context.RespondAsync("Queued the download and conversion of: `" + video.Title + "`\nYou will be notified when it is finished.");
                                    }
                                    else
                                    {
                                        await context.RespondAsync("Started the download and conversion of: `" + video.Title + "`\nYou will be notified when it is finished.");
                                    }

                                    await Download.AddToQueue(args[4], catFolder, args[3], context.Member, 0);
                                }

                                else
                                {
                                    await context.RespondAsync("Sorry! This video is encrypted and cannot be downloaded!");

                                }
                            } else
                            {
                                await context.RespondAsync("Invalid Youtube video ID!");
                            }
                        }

                    }
                    else
                    {
                        // Music.

                        //Check link.
                                
                        Console.WriteLine("==== ADDING DOWNLOAD: {0}  {1}  {2}  {3}  {4}", args[2], args[3], args[4], "files/music", context.Member.Username);


                        Console.WriteLine("==== Download QUEUE: {0}", Download.downQueue.Count);

                        //Last in queue's title.
                        var yt = YouTube.Default;
                        var video = yt.GetVideo(args[3]);
                        
                        if (IceBot.Classes.Download.downQueue.Count > 0)
                        {
                            await context.RespondAsync("Queued the download and conversion of: '" + video.Title + "'\nYou will be notified when it is finished.");
                        }
                        else
                        {
                            await context.RespondAsync("Started the download and conversion of: '" + video.Title + "'\nYou will be notified when it is finished.");
                        }

                        await Download.AddToQueue(args[4], "files\\music", args[2] + " - " + args[3], context.Member, 1, args[2], args[3]);


                    }

                }
                else
                {
                    await context.RespondAsync("You must specify whether to add this to sounds or music. (Ex. '.dl music')");
                }

            }
            else if (args.Count != 5 && args.Count != 1)
            {
                await context.RespondAsync("For help with the Download functions use '.help down'");
            }
            else if (args.Count == 1)
            {
                Console.WriteLine("ARGS LENGTH IS 1");
            }
            

        }

    }


	public class Download
	{

        public static Dictionary<int, YouTubeVideo> downQueue = new Dictionary<int, YouTubeVideo>();
        public static Dictionary<int, List<string>> downInfo = new Dictionary<int, List<string>>();
        public static Dictionary<int, DiscordMember> downMembers = new Dictionary<int, DiscordMember>();
        public static string dlStatus = "Queued";




        private static string GetTitle()
        {
            var title = "";
           
            List<string> vidInfo = downInfo[0];
            YouTubeVideo video = downQueue[0];

            if (vidInfo[3] == "0")
            {
                title = vidInfo[2];
            }
            else
            {
                title = string.Format("{0} - {1}", vidInfo[4], vidInfo[5]);
            }
            Console.WriteLine("TEST 4");

            return title;
        }

        private static string GetFolder()
        {
            var folder = "";
            List<string> vidInfo = downInfo[0];
            YouTubeVideo video = downQueue[0];

            if (vidInfo[3] == "0")
            {
                Console.WriteLine("FOLDER: {0}", Directory.GetCurrentDirectory());

                folder = Environment.CurrentDirectory + "\\files\\sounds\\" + vidInfo[1];
            }
            else
            {
                folder = "\\files\\music";
            }

            return folder;
        }

        public static async Task AddToQueue(string uri, string folder, string alias, DiscordMember member, int soundmusic, string artist = "", string title = "")
        {
            uri = "https://youtube.com/watch?v=" + uri;
            var yt = YouTube.Default;
            var videos = yt.GetAllVideos(uri);

            foreach (YouTubeVideo video in videos)
            {
                Console.WriteLine("VIDEO INFO: {0} : {1}", video.AdaptiveKind, video.FileExtension);
                if (video.AdaptiveKind == AdaptiveKind.Audio && video.FileExtension == ".mp4")
                {
                    Console.WriteLine("AUDIO FOUND");
                    downQueue.Add(downQueue.Count, video);
                    break;
                }
            }

            List<string> nDict = new List<string>();
            nDict.Add(uri);
            nDict.Add(folder);
            nDict.Add(alias);
            nDict.Add(soundmusic.ToString());
            nDict.Add(artist);
            nDict.Add(title);

            downInfo.Add(downInfo.Count, nDict);

            downMembers.Add(downMembers.Count, member);

            //Console.WriteLine($"Added Youtube video: '{uri}' to download queue.");

            await DownloadVideo();
        }

        private static async Task ConvertVideo()
        {
            Console.WriteLine("CONVERT VIDEO");

            
            List<string> vidInfo = downInfo[0];
            string title = GetTitle();
            string folder = GetFolder();

            Console.WriteLine("YO: " + folder + "\\" + title + ".mp4");
            var _out = "";
            var _process = new Process();
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.StartInfo.FileName = "ffmpeg";
            _process.StartInfo.Arguments = " -i \"" + folder + "\\" + title + ".mp4\" -vn -f mp3 -ab 192k \"" + folder + "\\" + title + ".mp3\"";
            Console.WriteLine(" -i \"" + folder + "\\" + title + ".mp4\" -vn -f mp3 -ab 192k \"" + folder + "\\" + title + ".mp3\"");
            _process.Start();
            _process.StandardOutput.ReadToEnd();
            _out = _process.StandardError.ReadToEnd();
            _process.WaitForExit();

            //Console.WriteLine("OUTPUT: {0}", _out);
            _process.Close();
            _process.Dispose();

            //Conversion is done, delete .mp4.
            Console.WriteLine("DELETE: {0}", folder + "\\" + title + ".mp4");
            File.Delete(folder + "\\" + title + ".mp4");



            //Add .mp3 file to Music or Sound Library.						
            if (vidInfo[3] == "0")
            {
                //Add to sounds
                Console.WriteLine("VID INFO: {0}", vidInfo[1]);

                Sound.AddSound(title, vidInfo[1]);
            }
            else
            {
                //Add to music

                //mp3Funcs.AddMusic(folder, vidInfo[5], vidInfo[6]);
            }


            //Inform user their download is complete.

            await downMembers[0].SendMessageAsync($"Your download of '{downQueue[0].Title}' has finished! It has been saved as '{title}'.");

            //Delete from dictionaries.
            downQueue.Remove(0);
            downInfo.Remove(0);
            downMembers.Remove(0);

            //Check to see if there is another one in queue, if so download.
            if (downQueue.Count > 0)
            {
                await DownloadVideo();
            }

            await Task.Delay(0);
        }

        private static async Task DownloadVideo()
        {

            try
            {
                var ytService = YouTube.Default;
                string id = downInfo[0][0];

                string title = GetTitle();
                string folder = GetFolder();

                Console.WriteLine("ID: " + id);
                var video = ytService.GetVideo(id);

                Console.WriteLine("Downloaded!");

                string path = Path.Combine(folder, title + ".mp4");

                Console.WriteLine("Saving to " + video.IsEncrypted.ToString() + "...");

                File.WriteAllBytes(path, video.GetBytes());

                Console.WriteLine("Done.");


                //ytService.
                await ConvertVideo();
            }
            catch
            {

            }
            

        }

        public bool CheckYTUrl(string link)
		{
			bool ret = false;

			try
			{
				if (link.Contains("youtube.com") == true)
				{
					WebClient x = new WebClient();
					string source = x.DownloadString(link);
					string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

					//logFuncs.AddToLogs("TITLE: {0}", title);

					if (title != "Youtube")
					{
						ret = true;
					}
				}
			}
			catch (Exception e)
			{
				//logFuncs.AddToLogs(String.Format("ERROR: {0} : {1}", e.Message, e.InnerException));
			}

			//logFuncs.AddToLogs(String.Format("CHECK YOUTUBE URL: {0}", ret));

			return ret;
		}


	}

}
