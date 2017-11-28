using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

using IceBot.Classes.Discord;
using System.Diagnostics;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;

using System.Timers;
using System.Text;

namespace IceBot.Classes
{


    public class Playback
    {
        public static Process ffProc;
        public static Stream ffOut;

        public async static Task StopSound()
        {
            //ffProc.Kill();
            ffOut.Dispose();

            //ffProc = null;
            //ffOut = null;

            await Task.Delay(0);
        }

        public static void Dispose()
        {
            ffProc.Kill();
            ffOut.Close();

            ffProc = null;
            ffOut = null;
        }

        public async static Task PlaySound(string file, DiscordGuild guild)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                UseShellExecute = false
            };

            if (ffProc != null || ffOut != null)
            {
                await StopSound();
            }


            ffProc = Process.Start(psi);
            ffOut = ffProc.StandardOutput.BaseStream;

            var buff = new byte[3840];
            var br = 0;

            var vcl = Discord.DiscordMain.discCl.GetVoiceNextClient();


            if (vcl != null)
            {
                var vnc = vcl.GetConnection(guild);


                while ((br = ffOut.Read(buff, 0, buff.Length)) > 0)
                {
                    if (br < buff.Length) // not a full sample, mute the rest
                        for (var i = br; i < buff.Length; i++)
                            buff[i] = 0;

                    await vnc.SendAsync(buff, 20);
                }

            }
        }



    }


    public class Sound
	{
		private static SQLiteDatabase sqlData = new SQLiteDatabase();

		public static DataTable sqlSounds;
		public static DataTable sqlCombos;

		public static List<DataRow> soundQueue = new List<DataRow>();
        public static List<DiscordGuild> soundGuild = new List<DiscordGuild>();

		public static Dictionary<int, string> soundCats = new Dictionary<int, string>();

        public static Timer sTimer = new Timer(500);


		public static bool isScanning = false;

		public enum SoundPlayback
		{
			Interrupt,
			Queue
		}

		public static SoundPlayback PlaybackType = SoundPlayback.Queue;

		#region "Sound Combo Functions"

		public static void LoadCombos()
		{
			DataTable tmpCombos;

			tmpCombos = sqlData.GetDataTable("SELECT * FROM soundcombos WHERE 1;");

			sqlCombos = tmpCombos;
		}

		public string[] listUserCombos(string acctID)
		{
			SortedList<string, int> sort = new SortedList<string, int>();

			int i = 0;

			if (sqlCombos != null)
			{
				foreach (DataRow str in sqlCombos.Rows)
				{
					//Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);

					if (str["sc_acctid"].ToString().Contains(acctID))
					{
						sort.Add(str["sc_alias"].ToString(), i);
						i += 1;
					}

				}
			}

			if (i == 0)
			{
				sort.Add("You have created no sound combos!", 0);
			}

			string[] res = Global.chatPage(sort, " -- Sound combos by You", "  -  ", "");

			return res;
		}

		public string[] listAllCombos()
		{
			SortedList<string, int> sort = new SortedList<string, int>();

			int i = 0;

			if (sqlCombos != null)
			{
				foreach (DataRow str in sqlCombos.Rows)
				{
					//Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);

					sort.Add(str["sc_alias"].ToString(), i);
					i += 1;

				}
			}

			if (i == 0)
			{
				sort.Add("There are no sound combos!", 0);
			}

			string[] res = Global.chatPage(sort, " -- Sound combos ", "  -  ", "");

			return res;
		}

		public bool SearchCombos(string alias)
		{
			bool ret = false;

			if (sqlCombos != null)
			{
				DataRow[] result = sqlCombos.Select("sc_alias = '" + alias + "'");

				if (result.Length > 0)
				{
					ret = true;
				}
			}

			return ret;
		}

		public string TrimName(string str)
		{
			string ret = str;

			return ret;
		}

		public string[] ConvertToStringArr(string str)
		{
			string[] del = new string[] { "," };
			string[] spl = str.Split(del, StringSplitOptions.None);


			return spl;
		}

		public void AddCombo(string alias, string combo, string acctID)
		{
			Dictionary<string, string> addCombo = new Dictionary<string, string>();
			addCombo.Add("sc_alias", alias);
			addCombo.Add("sc_acctid", acctID);
			addCombo.Add("sc_combo", combo);
			addCombo.Add("sc_created", Global.GetUnixTimestamp(DateTime.Now).ToString());

			sqlData.Insert("soundcombos", addCombo);

			LoadCombos();
		}

		#endregion


		#region "Misc Sound Funcs"
		public static string[] splitSound(string str)
		{

			string[] strSep = new string[] { "\\" };
			string[] spl = str.Split(strSep, StringSplitOptions.RemoveEmptyEntries);
			spl[1] = spl[1].Replace(".mp3", "");
			//spl[1] = spl[1].Replace("-", " ");

			return spl;
		}

		public static string[] splitSoundFilename(string str)
		{

			string[] strSep = new string[] { "/" };
			string[] spl = str.Split(strSep, StringSplitOptions.RemoveEmptyEntries);

			return spl;
		}

		public static string formatSoundDir(string dir)
		{

			dir = dir.Replace("_", " ");
			dir = Global.ToTitleCase(dir);

			return dir;
		}

		public static string stripSoundDir(string cat)
		{
			cat = cat.Replace(" ", "_");
			cat = cat.ToLower();

			return cat;
		}

		public static bool UpdateSoundData(DataRow soundData)
		{
			bool ret = false;

			Dictionary<String, String> data = new Dictionary<String, String>();

			int playCount = Convert.ToInt32(soundData["snd_plays"].ToString());
			playCount += 1;

			data.Add("snd_lastplay", Global.GetUnixTimestamp(DateTime.Now).ToString());
			data.Add("snd_plays", playCount.ToString());

			try
			{
				sqlData.Update("sounds", data, String.Format("sounds.snd_id = {0}", soundData["snd_id"].ToString()));
				sqlSounds = sqlData.GetDataTable("SELECT * FROM `sounds`;");
			}
			catch (Exception crap)
			{
				Console.WriteLine(crap.Message);
			}

			return ret;
		}

		public static void UpdateSound(string ID, Dictionary<string, string> data)
		{

			try
			{
				sqlData.Update("sounds", data, String.Format("sounds.snd_id = '{0}'", ID));
				sqlSounds = sqlData.GetDataTable("SELECT * FROM `sounds`;");
			}
			catch (Exception crap)
			{
				Console.WriteLine(crap.Message);
			}

		}


		public static DataTable srchSounds(string srchBy, string value)
		{
			DataTable ret = new DataTable();

			try
			{
				ret = sqlData.GetDataTable("SELECT * FROM sounds WHERE `" + srchBy + "` = '" + value + "';");

			}
			catch (Exception fail)
			{
				String error = "The following error has occurred:\n\n";
				error += fail.Message.ToString() + "\n\n";

				Console.WriteLine(error);
			}

			return ret;
		}

		public bool CheckSoundCat(string catName)
		{
			bool ret = false;

			int i = 0;

			foreach (string str in soundCats.Values)
			{

				if (str.ToLower().Contains(catName.ToLower()))
				{
					ret = true;

					break;
				}

				i += 1;
			}

			return ret;
		}

		public static int[] srchSoundCatsLoose(string catName)
		{
			int[] ret = new int[2];

			ret[0] = 0;

			int i = 0;

			foreach (string str in soundCats.Values)
			{

				if (str.ToLower().Contains(catName.ToLower()))
				{
					ret[1] = i;
					ret[0] += 1;

					break;
				}

				i += 1;
			}

			return ret;
		}

		public static string[] searchSounds(string stri)
		{
			SortedList<string, int> sort = new SortedList<string, int>();

			int i = 0;

			DataRow[] results = sqlSounds.Select("snd_aliases LIKE '%" + stri + "%'");

			if (results != null)
			{
				foreach (DataRow str in results)
				{
					try
					{
						sort.Add(str[1].ToString(), i);
						i += 1;
					}
					catch (Exception e)
					{
						Console.WriteLine("More than likely a duplicate row error, add a number to it.");
						sort.Add(str[1].ToString() + " (Dupe)", i);
						i += 1;
					}
					finally
					{

					}
					//Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);
				}
			}

			if (i == 0)
			{
				sort.Add("No sounds found matching your search!", 0);
			}

			string[] res = Global.chatPage(sort, " -- Search for '" + stri + "'", "  -  ", "");

			return res;

		}

		public static int srchSoundMultiValid(string[] names)
		{
			int ret = -1;
			int fail = -1;

			foreach (string nameStr in names)
			{
				bool valid = false;

				foreach (DataRow str in sqlSounds.Rows)
				{
					string n = str["snd_aliases"].ToString();


					if (n.ToLower() == nameStr.ToLower())
					{
						if (str["snd_ignore"].ToString() != "1")
						{
							ret = 1;
							valid = true;

							//Console.WriteLine("{0} FOUND", nameStr);
							break;
						}
						else
						{
							ret = -1;
							valid = false;

							break;
						}
					}

				}

				if (valid == false)
				{
					ret = -1;
					fail = 1;

					//Console.WriteLine("{0} NOT FOUND", nameStr);
				}

			}

			return fail;
		}

		public static string srchSoundMulti(string[] names)
		{
			string ret = "Invalid or Ignored Sounds: ";
			int i = 0;

			foreach (string nameStr in names)
			{
				bool valid = false;

				foreach (DataRow str in sqlSounds.Rows)
				{
					string n = str["snd_aliases"].ToString();

					if (n.ToLower() == nameStr.ToLower())
					{
						if (str["snd_ignore"].ToString() != "1")
						{
							valid = true;
							//Console.WriteLine("{0} FOUND", nameStr);
							break;
						}
						else
						{
							valid = false;
							break;
						}
					}
				}

				if (valid == false)
				{
					//Console.WriteLine("{0} NOT FOUND", nameStr);
					if (i == 0)
					{
						ret = ret + nameStr;
					}
					else
					{
						ret = ret + ", " + nameStr;
					}

					i += 1;
				}

			}

			if (i == 0)
			{
				ret = "";
			}

			return ret;
		}

		public static List<DataRow> srchSoundFileMulti(string[] names)
		{

			List<DataRow> ret = new List<DataRow>();

			//Console.WriteLine("SOUNDS: {0}", g_soundFiles.Length);
			foreach (string nameStr in names)
			{
				int i = 0;

				foreach (DataRow str in sqlSounds.Rows)
				{
					string n = str["snd_aliases"].ToString();

					if (n.ToLower() == nameStr.ToLower())
					{
						//Console.WriteLine("SPL: {0}, {1}, {2}", n, nameStr, i);
						ret.Add(str);
						break;
					}

					i += 1;
				}

			}

			return ret;

		}

		public static string searchSimilar(string stri)
		{
			SortedList<string, int> sort = new SortedList<string, int>();
			string retStr = "";

			int i = 0;

			DataRow[] results = sqlSounds.Select("snd_aliases LIKE '%" + stri + "%'");

			if (results != null)
			{
				foreach (DataRow str in results)
				{
					//Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);
					sort.Add(str[1].ToString(), i);
					i += 1;
				}
			}
			int len = sort.Count;

			retStr = string.Join(", ", sort.Keys);

			return retStr;
		}


		public static string[] listSounds(int cat, int soundView)
		{
			SortedList<string, int> sort = new SortedList<string, int>();

			int i = 0;

			foreach (DataRow str in sqlSounds.Rows)
			{
				//Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);

				if (str["snd_tags"].ToString().Contains(soundCats[cat]))
				{
					if (str["snd_ignore"].ToString() != "1")
					{

						sort.Add(str["snd_aliases"].ToString(), i);
                        i++;
					}

				}

			}

			if (i == 0)
			{
				sort.Add("No sounds!", 0);
			}

			string[] res;

			if (soundView == 0)
			{
				res = Global.chatPage(sort, " -- Sounds in " + soundCats[cat] + "", "  -  ", "");
			}
			else
			{
				res = Global.chatPage(sort, " -- Sounds in " + soundCats[cat] + "", "\n", "");
			}


			return res;

		}

        public static string listAllSounds(int cat)
        {
            SortedList<string, int> sort = new SortedList<string, int>();

            int i = 0;
            string res = "";


            foreach (DataRow str in sqlSounds.Rows)
            {
                //Console.WriteLine("STR: {0} : {1}", str, g_soundList[str]);

                if (str["snd_tags"].ToString().Contains(soundCats[cat]))
                {
                    if (str["snd_ignore"].ToString() != "1")
                    {

                        res = res + $"** {str["snd_aliases"].ToString()} **     -     ";
                        i++;
                    }

                }

            }

            if (i == 0)
            {
                res = "No sounds in this category!";
            }

            //var res = sort.

            return res.ToString();

        }


        public static string[] listSoundCats()
		{
			SortedList<string, int> sort = new SortedList<string, int>();

			int i = 0;

			foreach (string str in soundCats.Values)
			{
				sort.Add(str, i);
				i += 1;
			}

			if (i == 0)
			{
				sort.Add("No sound categories!", 0);
			}

			string[] res = Global.chatPage(sort, " -- Sounds Categories", "  -  ", "");

			return res;
		}

        public static string listAllSoundCats()
        {
            SortedList<string, int> sort = new SortedList<string, int>();

            int i = 0;
            string res = "";

            foreach (string str in soundCats.Values)
            {
                res = res + $"**{str}**     -    ";
                i += 1;
            }

            if (i == 0)
            {
                res = "No sound categories!";
            }

            
            return res.ToString();
        }



        public static string[] listNewSounds()
		{
			SortedList<int, string> sort = new SortedList<int, string>();

			int i = 0;

			DataTable tmpSqlSounds;

			tmpSqlSounds = sqlData.GetDataTable("SELECT * FROM sounds WHERE 1 ORDER BY `snd_dateadd` DESC");

			if (tmpSqlSounds != null)
			{

				foreach (DataRow str in tmpSqlSounds.Rows)
				{
					if (str["snd_ignore"].ToString() != "1")
					{
						sort.Add(i, str[1].ToString());
						i++;
					}
				}

			}

			if (i == 0)
			{
				sort.Add(0, "No new sounds!");
			}

			string[] res = Global.chatPageByInt(sort, " -- Newest sounds", "  -  ", "");

			return res;
		}

		public static string[] listTopSounds()
		{
			SortedList<int, string> sort = new SortedList<int, string>();

			int i = 0;

			DataTable tmpSqlSounds;

			tmpSqlSounds = sqlData.GetDataTable("SELECT * FROM `sounds` WHERE 1 ORDER BY `snd_plays` DESC");

			if (tmpSqlSounds != null)
			{

				foreach (DataRow str in tmpSqlSounds.Rows)
				{
					if (str["snd_ignore"].ToString() != "1")
					{
						sort.Add(i, str[1].ToString() + " ( " + str["snd_plays"].ToString() + " ) ");
						i++;
					}
				}

			}

			if (i == 0)
			{
				sort.Add(0, "No top sounds!");
			}

			string[] res = Global.chatPageByInt(sort, " -- Popular sounds -- Name (Plays)", "  -  ", "");

			return res;
		}

		public static string[] listSoundQueued()
		{
			SortedList<int, string> sort = new SortedList<int, string>();

			int i = 0;

			foreach (DataRow str in soundQueue)
			{
				int tmp = i + 1;

				sort.Add(i, tmp + ". " + str["snd_aliases"].ToString());
				i++;
			}

			if (i == 0)
			{
				sort.Add(0, "No queued sounds!");
			}

			string[] res = Global.chatPageByInt(sort, " -- Queued Sounds", "  -  ", "");

			return res;
		}

		public static string[] listSoundRecent()
		{
			SortedList<int, string> sort = new SortedList<int, string>();

			int i = 0;
			DataTable tmpSqlSounds;

			tmpSqlSounds = sqlData.GetDataTable("SELECT * FROM sounds WHERE 1 ORDER BY `snd_lastplay` DESC LIMIT 0,15;");


			if (tmpSqlSounds != null)
			{
				foreach (DataRow str in tmpSqlSounds.Rows)
				{
					if (str["snd_ignore"].ToString() != "1")
					{
						sort.Add(i, str[1].ToString());
						i++;
					}
				}
			}

			if (i == 0)
			{
				sort.Add(0, "No recent sounds!");
			}

			string[] res = Global.chatPageByInt(sort, " -- Recently Played Sounds", "  -  ", "");

			return res;
		}

		public DataRow GetRecentSound()
		{
			DataRow ret = null;

			DataTable tmpSqlSounds;

			tmpSqlSounds = sqlData.GetDataTable("SELECT * FROM sounds WHERE 1 ORDER BY `snd_lastplay` DESC LIMIT 0,1;");

			if (tmpSqlSounds != null)
			{
				foreach (DataRow str in tmpSqlSounds.Rows)
				{
					ret = str;
					break;
				}
			}

			return ret;
		}


		private void CompareSounds(string file)
		{

		}

		public static void AddSound(string alias, string cat)
		{

			Dictionary<String, String> data = new Dictionary<String, String>();

			//Times
			string unixTime = Global.GetUnixTimestamp(DateTime.Now).ToString();

			//data.Add("u_created", unixTime);
			//data.Add("u_lastlog", unixTime);

			data.Add("snd_aliases", alias);

			string tempCat = formatSoundDir(cat);


			data.Add("snd_filename", cat + "/" + alias);
			data.Add("snd_dateadd", unixTime.ToString());
			data.Add("snd_tags", tempCat + ";");

            

			try
			{
                Console.WriteLine("INSERTING INTO SQL");

				sqlData.Insert("sounds", data);

                Console.WriteLine("AFTER INSERTION");

				DataTable getData;
				getData = sqlData.GetDataTable("SELECT * FROM sounds;");

				sqlSounds = getData;
			}
			catch (Exception crap)
			{
				//MessageBox.Show(crap.Message);
				Console.WriteLine("ERROR CREATING USER: {0}", crap.Message);
			}

		}

		public static async Task InsertSounds()
		{
			//sqlData.ClearTable("sounds");

			//sqlData.ExecuteNonQuery("delete from sounds; delete from sqlite_sequence where name='sounds';");
			//Console.WriteLine("DOING SOUNDS");

			isScanning = true;

			try
			{
				sqlData.GetDataTable("SELECT 1 FROM sounds;");
			}
			catch
			{
				Console.WriteLine("NO SOUNDS TABLE");

				sqlData.ExecuteNonQuery("CREATE TABLE `sounds` (" +
					"`snd_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
					"`snd_aliases`	VARCHAR(255)," +
					"`snd_filename`	VARCHAR(100)," +
					"`snd_dateadd`	INTEGER," +
					"`snd_tags`	TEXT," +
					"`snd_lastplay`	INTEGER," +
					"`snd_plays`	INTEGER DEFAULT '0'," +
					"`snd_ignore`	INTEGER DEFAULT 0," +
					"`snd_volume`	INTEGER DEFAULT 0" +
					"`snd_serverID` TEXT" +
				"); ");

			}

			try
			{
				sqlData.GetDataTable("SELECT 1 FROM soundcombos;");
			}
			catch
			{
				sqlData.ExecuteNonQuery("CREATE TABLE `soundcombos` (" +
					"`sc_id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
					"`sc_alias`	VARCHAR(45), " +
					"`sc_combo`	VARCHAR(255), " +
					"`sc_plays`	INTEGER DEFAULT '0', " +
					"`sc_created`	INTEGER, " +
					"`sc_acctid`	VARCHAR(50)" +
					"`sc_serverID`   TEXT" +
				"); ");
			}

			if (Directory.Exists(Environment.CurrentDirectory + "\\files\\sounds\\"))
			{
				//Task newTask = new Task(new Action(AddToLogs));

				//("Scanning sounds...", "SOUND");

				DataTable tmpSqlSounds;

				tmpSqlSounds = sqlData.GetDataTable("SELECT * FROM sounds WHERE 1;");


				foreach (DataRow row in tmpSqlSounds.Rows)
				{
					if (System.IO.File.Exists("files/sounds/" + row["snd_filename"].ToString() + ".mp3") == false)
					{
						Console.WriteLine("NONEXISTANT FILE: {0}", row["snd_filename"].ToString());

						sqlData.ExecuteNonQuery("DELETE from sounds WHERE `snd_id` = '" + row["snd_id"].ToString() + "';");

					}
				}



				isScanning = true;

				String[] allfiles = System.IO.Directory.GetFiles(Environment.CurrentDirectory + "\\files\\sounds\\", "*.mp3", System.IO.SearchOption.AllDirectories);

				int i = 0;
				int catID = 0;

				Dictionary<int, Dictionary<string, string>> soundData = new Dictionary<int, Dictionary<string, string>>();


				foreach (string s in allfiles)
				{
					string str = s.Replace(Environment.CurrentDirectory + "\\files\\sounds\\", "");

					string[] spl = splitSound(str);

					string formDir = formatSoundDir(spl[0]);

					//

					DataTable chkSound;
					//Console.WriteLine("SPL 1: {0}", spl[1]);
					String query = "SELECT * FROM sounds WHERE snd_aliases = '" + spl[1] + "' LIMIT 0,1;";

					chkSound = sqlData.GetDataTable(query);

					if (soundCats.ContainsValue(formDir) == false)
					{
						soundCats.Add(catID, formDir);

						catID += 1;
					}

					if (chkSound.Rows.Count == 0)
					{

						try
						{
							int unixTime = Global.GetUnixTimestamp(DateTime.Now);
							//Console.WriteLine("UNIX: {0}", unixTime);

							Dictionary<string, string> dict = new Dictionary<string, string>();
							dict.Add("snd_aliases", spl[1]);
							dict.Add("snd_filename", spl[0] + "/" + spl[1]);
							dict.Add("snd_dateadd", unixTime.ToString());
							dict.Add("snd_tags", formDir + ";");
							//dict.Add("snd_serverID", row["snd_id"].ToString());

							soundData.Add(i, dict);
						}
						catch (Exception ex)
						{
							Console.WriteLine("EX: {0}", ex.Message);
						}

						i += 1;
					}

				}

				isScanning = false;

				Console.WriteLine("Sound Scan Complete!");
                
				sqlData.MassInsert("sounds", soundData);

				//Console.WriteLine("Querying it into a DataTable");

				DataTable getData;
				getData = sqlData.GetDataTable("SELECT * FROM sounds;");

				sqlSounds = getData;

                //Console.WriteLine("DATA ROWS: {0}", getData.Rows.Count);
                DiscordMain.Logger.LogMessage(DSharpPlus.LogLevel.Info, "SOUNDS", $"Sound Scan Complete!  -- {getData.Rows.Count.ToString()} sounds available!", DateTime.Now);

                LoadCombos();
				//Console.WriteLine("Sound Categories: {0}", soundCats.Count);

				//logFuncs.AddToLogs(String.Format("Finished scanning sounds. '{0}' total sounds.", sqlSounds.Rows.Count), "SOUND");
			}
			else
			{
				//logFuncs.AddToLogs("ERROR! Could not find 'sounds' directory.", "SOUND");
			}

			//Test();
			

			isScanning = false;

			await Task.Delay(0);
		}



        #endregion




        #region "Sound Playback"


        


        private static void Skip()
        {

            if (soundQueue.Count > 1)
            {
                soundQueue.RemoveAt(0);

                playSound(soundQueue[0]);
            }
            else if (soundQueue.Count == 1)
            {
                soundQueue.RemoveAt(0);
                //disposeWave2();
            }
            else
            {
                //disposeWave2();
            }
        }


        public static async Task playSound(DataRow soundData)
        {
            Console.WriteLine("PLAY QUEUE");

            //disposeWave2();
            //Playback.Dispose();

            Console.WriteLine("PQ");

            string path = "files\\sounds\\" + soundData["snd_filename"].ToString() + ".mp3";

            Console.WriteLine($"PATH: {path}");

            await Playback.PlaySound(path, soundGuild[0]);

            //Add to recently played list.
            UpdateSoundData(soundData);


        }

        public void skipSound()
        {
            Skip();
        }

        #endregion


    }

}
