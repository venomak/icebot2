using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace IceBot.Classes
{
    public class Global
    {


        public static string OnlineOffline(bool tf)
        {

            if (tf == true)
            {
                return "Online";
            }
            else
            {
                return "Offline";
            }


        }

        //IceBot Version
        public string icebotVersion = "3.0a";
        //

        public static Random randomNumGen = new Random();


        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public float formatVol(int volu)
        {
            float f = 0.0f;

            f = (float)volu / 100;

            return f;
        }


        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public string Capitalize(string str)
        {
            var tmp = str;
            var letter = tmp.Substring(0, 1);
            tmp = tmp.Substring(1);

            letter = letter.ToUpper();

            return letter + tmp;

        }

        public static Dictionary<int, string> HandleArgs(string msg)
        {

            Dictionary<int, string> ret = new Dictionary<int, string>();

            try
            {
                string[] strSep = new string[] { " " };
                string[] spl = msg.Split(strSep, StringSplitOptions.RemoveEmptyEntries);

                int last = -1;
                string arg = "";
                int i = 0;

                foreach (string str in spl)
                {
                    //Console.WriteLine("STR2: {0}", str);

                    if (str.StartsWith("\"") == true || str.EndsWith("\"") == true)
                    {
                        //Console.WriteLine("TRUE 1");

                        if (str.StartsWith("\"") == true && str.EndsWith("\"") == false)
                        {
                            arg = str;
                            last = 0;
                        }
                        else if (str.EndsWith("\"") == true && str.StartsWith("\"") == false)
                        {
                            arg = arg + " " + str;

                            arg = arg.Replace("\"", "");

                            ret.Add(i, arg);

                            last = -1;

                            i++;
                        }
                        else if (str.StartsWith("\"") == true && str.EndsWith("\"") == true)
                        {
                            //Console.WriteLine("")
                            string tmp = str.Replace("\"", "");
                            ret.Add(i, tmp);
                            i++;

                            last = -1;
                        }

                    }
                    else
                    {
                        if (last == -1)
                        {
                            // No quote.
                            //Console.WriteLine("NO QUOTE");
                            ret.Add(i, str);
                            i++;
                        }
                        else
                        {
                            // Middle of a quoted argument.
                            //Console.WriteLine("MID QUOTE");

                            arg = arg + " " + str;
                            last = 0;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0} : {1} : {2}", ex.Message, ex.StackTrace, ex.TargetSite);


            }


            return ret;
        }


        //public async Task<Task> sendChatMsg(DiscordChannel channel, string msg, bool isError = false)
        //{
        //	if (isError == false)
        //	{
        //		await channel.SendMessage(msg);
        //	}
        //	else
        //	{
        //		await channel.SendMessage("Error! " + msg);
        //	}

        //	return Task.CompletedTask;
        //}

        public static string[] chatPage(SortedList<string, int> list, string title, string sep, string prefix, int limit = 450)
        {
            string[] ret = new string[0];

            string msg = "";
            string pre = "";

            if (prefix != null)
            {
                pre = prefix;
            }

            int ci = 0;

            foreach (string str in list.Keys)
            {
                
                if (ci != list.Count - 1)
                {

                    if (msg.Length <= limit)
                    {
                        msg = msg + pre + str + sep;
                    }
                    else
                    {
                        msg = msg + pre + str;

                        int tmp = ret.Length + 1;
                        //Console.WriteLine("TMP: {0}", tmp);
                        Array.Resize(ref ret, tmp);
                        ret[tmp - 1] = msg;

                        msg = "";
                    }


                }
                else
                {
                    msg = msg + pre + str;

                    int tmp = ret.Length + 1;
                    Array.Resize(ref ret, tmp);
                    ret[tmp - 1] = msg;

                    msg = "";
                }

                ci += 1;
            }


            //Add titles and notifications...
            ci = 1;

            foreach (string str in ret)
            {
                if (ci < ret.Length)
                {
                    int npg = ci + 1;
                    ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n" + str + "\n\nAnd more on Page " + npg.ToString() + "";
                }
                else
                {
                    ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n" + str;
                }

                ci += 1;
            }

            return ret;
        }

        //public string[] chatPageSimple(SortedList<int, string> list, string title, string sep, string prefix, int cols, string colSep = "   =   ", int limit = 450)
        //{
        //    string[] ret = new string[0];

        //    string msg = "";
        //    string pre = "";

        //    if (prefix != null)
        //    {
        //        pre = prefix;
        //    }

        //    int ci = 0;
        //    int c = 1;
        //    //int i = 1;

        //    foreach (string str in list.Values)
        //    {
        //        if (ci != list.Count() - 1)
        //        {

        //            if (msg.Length <= limit)
        //            {
        //                if (c < cols)
        //                {
        //                    msg += pre + str + colSep;
        //                    c += 1;
        //                }
        //                else if (c == cols)
        //                {
        //                    msg += pre + str + sep + "\n" + sep + " ";
        //                    c = 1;
        //                }
        //            }
        //            else
        //            {
        //                msg += pre + str + "  |";

        //                int tmp = ret.Length + 1;
        //                //Console.WriteLine("TMP: {0}", tmp);
        //                Array.Resize(ref ret, tmp);
        //                ret[tmp - 1] = msg;

        //                msg = "";
        //            }


        //        }
        //        else
        //        {
        //            msg += pre + str + "  |";

        //            int tmp = ret.Length + 1;
        //            Array.Resize(ref ret, tmp);
        //            ret[tmp - 1] = msg;

        //            msg = "";
        //        }

        //        ci += 1;
        //    }


        //    //Add titles and notifications...

        //    ci = 1;

        //    foreach (string str in ret)
        //    {
        //        if (ci < ret.Length)
        //        {
        //            int npg = ci + 1;
        //            ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n  |  " + str + "\n\nAnd more on Page " + npg.ToString() + "";
        //        }
        //        else
        //        {
        //            ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n  |  " + str;
        //        }

        //        ci += 1;
        //    }



        //    return ret;
        //}


        public static string[] chatPageByInt(SortedList<int, string> list, string title, string sep, string prefix, int limit = 550, bool desc = false)
        {
            string[] ret = new string[0];

            string msg = "";
            string pre = "";

            if (prefix != null)
            {
                pre = prefix;
            }

            int ci = 0;

            //if (desc == true)
            //{
            //    list.Reverse();

                
            //}

            foreach (string str in list.Values)
            {

                if (ci != list.Count - 1)
                {

                    if (msg.Length <= limit)
                    {
                        msg = msg + pre + str + sep;
                    }
                    else
                    {
                        msg = msg + pre + str;

                        int tmp = ret.Length + 1;
                        //Console.WriteLine("TMP: {0}", tmp);
                        Array.Resize(ref ret, tmp);
                        ret[tmp - 1] = msg;

                        msg = "";
                    }


                }
                else
                {
                    msg = msg + pre + str;

                    int tmp = ret.Length + 1;
                    Array.Resize(ref ret, tmp);
                    ret[tmp - 1] = msg;

                    msg = "";
                }

                ci += 1;
            }


            //Add titles and notifications...
            ci = 1;

            foreach (string str in ret)
            {
                if (ci < ret.Length)
                {
                    int npg = ci + 1;
                    ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n" + str + "\n\nAnd more on Page " + npg.ToString() + "";
                }
                else
                {
                    ret[ci - 1] = title + " -- Page " + ci.ToString() + " of " + ret.Length.ToString() + " -- \n" + str;
                }

                ci += 1;
            }


            return ret;
        }


        public string timeAgo(DateTime date)
        {
            var now = DateTime.Now;

            TimeSpan diff = now - date;

            //Console.WriteLine("Difference: {0}", diff.TotalHours);

            var totDays = Math.Floor(diff.TotalDays);
            var totHours = Math.Floor(diff.TotalHours);
            var totMins = Math.Floor(diff.TotalMinutes);
            var totSecs = Math.Floor(diff.TotalSeconds);

            string retStr = "";

            Console.WriteLine("Difference: {0} Days - {1} Hours - {2} Minutes - {3} Seconds", totDays, totHours, totMins, totSecs);

            if (totHours <= 0)
            {
                //Show minutes ago.
                //Console.WriteLine("SHOW MINUTES ONLY");

                if (totMins <= 0)
                {
                    //Show seconds ago.
                    //Console.WriteLine("SHOW SECONDS ONLY");

                    if (totSecs > 1)
                    {
                        retStr = string.Format("{0} seconds ago", totSecs);
                    }
                    else
                    {
                        retStr = string.Format("{0} second ago", totSecs);
                    }

                }

                if (totMins > 1)
                {
                    retStr = string.Format("{0} minutes ago", totMins);
                }
                else
                {
                    retStr = string.Format("{0} minute ago", totMins);
                }

            }
            else if (totHours >= 24)
            {
                //Show date.
                //Console.WriteLine("SHOW DATE ONLY");

                retStr = String.Format("{0}/{1}/{2}", date.Month, date.Day, date.Year);

            }
            else if (totHours >= 0 && totHours <= 23)
            {
                //Show hours ago.
                Console.WriteLine("SHOW HOURS ONLY");

                if (totHours > 1)
                {
                    retStr = string.Format("{0} hours ago", totHours);
                }
                else
                {
                    retStr = string.Format("{0} hour ago", totHours);
                }
            }

            //Console.WriteLine("RETURN STR: {0}", retStr);

            return retStr;
        }

        public int CheckIfPage(string str)
        {
            int pg = -1;

            try
            {
                pg = Convert.ToInt32(str);
            }
            catch
            {

            }

            return pg;
        }


        private string AddChars(string chars, int count)
        {
            int i = 0;
            string ret = "";

            while (i <= count)
            {
                ret = ret + chars;

                i += 1;
            }

            return ret;
        }


        public string NeatFormat(string[] headers, Dictionary<int, string[]> data, bool headBool = false)
        {
            string[] tmpH = new string[5] { "Name", "Hero", "KDA", "GPM", "XPM" };
            Dictionary<int, string[]> tmpD = new Dictionary<int, string[]>();

            tmpD.Add(0, new string[5] { "AJSD", "Lifestaler", "5/5/5", "1", "1" });
            tmpD.Add(1, new string[5] { "ASSSHUHUHU", "Riki", "51/5/5", "11", "123" });
            tmpD.Add(2, new string[5] { "HUHUHUASDFDFSA ASDF ", "Lifestaler", "5/5/5", "234", "234" });
            tmpD.Add(3, new string[5] { "UIIUOIOUOI", "Shadow Fiend", "5/50/5", "122", "424" });
            tmpD.Add(4, new string[5] { "AS", "Lifestaler", "5/5/50", "2331", "1111" });
            tmpD.Add(5, new string[5] { "OOOOPPP", "Anti-Mage", "50/50/50", "333", "666" });

            headers = tmpH;
            data = tmpD;

            Console.WriteLine("# Headers: {0} #", headers.Length);

            int[] sep = new int[0];
            int[] currMax = new int[headers.Length];

            //int maxLen = 0;

            foreach (KeyValuePair<int, string[]> str in data)
            {
                //Console.WriteLine("# {0} # {1} # {2} # {3} # {4} #", str.Value[0], str.Value[1], str.Value[2], str.Value[3], str.Value[4]);
                int i = 0;

                foreach (string s in str.Value)
                {
                    //Console.WriteLine("{0}", str.Value[i].Length);

                    if (currMax[i] == 0)
                    {
                        currMax[i] = str.Value[i].Length;

                        //Console.WriteLine("NEW CURR MAX: {0} :: {1} # {2}", i, currMax[i], str.Value[i].Length);
                    }
                    else
                    {
                        //Not 0, compare.

                        if (str.Value[i].Length > currMax[i])
                        {
                            //Console.WriteLine("NEW CURR MAX: {0} :: {1} # {2}", i, currMax[i], str.Value[i].Length);
                            currMax[i] = str.Value[i].Length;
                        }

                    }

                    i += 1;
                }

            }

            int maxLen = 0;

            foreach (int ind in currMax)
            {
                //Console.WriteLine("MAX: {0}", ind);

                maxLen = maxLen + ind;
            }


            string ret = "\n####################################\n# ";

            if (headBool == false)
            {
                int di = 0;

                foreach (string head in headers)
                {
                    int dif = currMax[di] - head.Length;

                    Console.WriteLine("DIFF: {0}", dif);
                    ret = ret + " " + head + AddChars(" ", dif) + "#";
                    di += 1;
                }
            }

            ret = ret + "\n# ";

            foreach (KeyValuePair<int, string[]> str in data)
            {
                int i = 0;

                foreach (string s in str.Value)
                {
                    //Console.WriteLine("{0} :: {1}", s.Length, s, currMax[i]);

                    int dif = currMax[i] - s.Length;

                    //Console.WriteLine("DIFFERENCE: {0}", dif);

                    ret = ret + s + AddChars(" ", dif + 1) + "# ";


                    i += 1;
                }

                if (str.Key != (data.Keys.Count - 1))
                {
                    Console.WriteLine("KEY: {0} :: {1}", str.Key, data.Keys.Count);

                    ret = ret + "\n# ";
                }

            }
            ret = ret + "\n####################################";

            Console.WriteLine("RET: {0}", ret);

            return ret;

        }


        public static int GetUnixTimestamp(DateTime when)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (when - epoch);
            int unixTime = Convert.ToInt32(span.TotalSeconds);

            return unixTime;
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            DateTime ret = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            ret.AddSeconds(Convert.ToDouble(unixTimeStamp));

            return ret;
        }

        public static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(Convert.ToDouble(timestamp));
        }

        public string Clean(string str)
        {
            string ret = str;

            ret = str.Replace("'", "''");

            return ret;
        }

        public string RemoveBB(string str)
        {
            string tMsg = Regex.Replace(str, @"\[[^]]+\]", "");

            return tMsg;
        }

        public static int IntLength(int i)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException();
            if (i == 0)
                return 1;
            return (int)Math.Floor(Math.Log10(i)) + 1;
        }

        public string formatT(TimeSpan time)
        {
            string ret = "";
            string totsecs = ((int)time.Seconds).ToString();
            string totmins = ((int)time.Minutes).ToString();

            string mins = totmins;
            string secs = totsecs;


            if (totmins.Length == 1)
            {
                mins = "0" + mins;
            }

            if (totsecs.Length == 1)
            {
                secs = "0" + secs;
            }

            ret = mins + ":" + secs;


            return ret;
        }

        public int formatTimeToSecs(TimeSpan time)
        {
            int ret = -1;

            int mins = time.Minutes;
            int secs = time.Seconds;

            ret = (mins * 60) + secs;


            return ret;
        }

        public TimeSpan formatSecsToTime(int secs)
        {
            int mins = secs / 60;
            int sec = secs % 60;

            TimeSpan ret = new TimeSpan(0, mins, sec);

            Console.WriteLine("TIME: {0}:{1}", mins, sec);

            return ret;
        }

        public bool checkTime(string str)
        {
            bool ret = false;

            string[] strSep = new string[] { ":" };
            string[] spl = str.Split(strSep, StringSplitOptions.RemoveEmptyEntries);

            if (spl.Length > 1)
            {
                int mins = -1;

                try
                {
                    mins = Convert.ToInt32(spl[0]);
                }
                catch
                {

                }

                if (mins != -1)
                {

                    int secs = -1;

                    try
                    {
                        secs = Convert.ToInt32(spl[0]);
                    }
                    catch
                    {

                    }

                    if (secs != -1)
                    {
                        ret = true;
                    }

                }

            }

            return ret;
        }

        public int timeToSecs(string str)
        {
            int ret = -1;

            string[] strSep = new string[] { ":" };
            string[] spl = str.Split(strSep, StringSplitOptions.RemoveEmptyEntries);

            int mins = Convert.ToInt32(spl[0]);

            ret = mins * 60;
            ret = ret + Convert.ToInt32(spl[1]);


            return ret;
        }

        public static string FormatHrMin(DateTime time)
        {
            string ret = "";

            int hour = time.Hour;
            int min = time.Minute;
            string ampm = "";

            if (hour >= 12)
            {

                if (hour != 12)
                {
                    hour = hour - 12;
                }

                ampm = "PM";
            }
            else
            {
                ampm = "AM";
            }

            if (hour == 0)
            {
                hour = 12;
            }

            string hr = hour.ToString();
            string m = min.ToString();

            if (hr.Length == 1)
            {
                hr = "0" + hr;
            }

            if (m.Length == 1)
            {
                m = "0" + m;
            }

            ret = hr + ":" + m + " " + ampm;

            return ret;
        }

        public string CleanURL(string str)
        {
            string ret = "";

            if (str.Contains("http://") || str.Contains("https://"))
            {
                ret = str;
            }
            else
            {
                if (str.Contains("https"))
                {
                    ret = "https://" + str;
                }
                else
                {
                    ret = "http://" + str;
                }
            }

            return ret;
        }


        public string FormatTimeRec(TimeSpan time)
        {
            string ret = "";

            string hrs = time.Hours.ToString();
            string mins = time.Minutes.ToString();
            string secs = time.Seconds.ToString();

            if (hrs.Length == 1)
            {
                hrs = "0" + hrs;
            }

            if (mins.Length == 1)
            {
                mins = "0" + mins;
            }

            if (secs.Length == 1)
            {
                secs = "0" + secs;
            }

            ret = String.Format("{0}h {1}m {2}s", hrs, mins, secs);

            return ret;
        }

        public string CommaNum(int num)
        {
            string ret = num.ToString("#,##0");

            return ret;
        }

        public string getPlural(int num)
        {
            var ret = "";

            if (num > 1)
            {
                ret = "s";
            }

            return ret;
        }

        public string FormatTime(int secs)
        {
            bool neg = false;
            if (secs < 0)
            {
                neg = true;
                secs = secs * -1;
            }

            int m = secs / 60;
            int s = secs % 60;

            string ret = "";
            string min = "";
            string sec = "";
            string ago = "";
            //Console.WriteLine("MIN: {0} :: SEC: {1}", m, s);

            if (m > 0)
            {
                min = " " + m.ToString() + " minute" + getPlural(m);
            }

            if (s > 0)
            {
                sec = " " + s.ToString() + " second" + getPlural(s);
            }

            if (neg == true)
            {
                ago = " ago";
            }

            ret = min + sec + ago;
            //Console.WriteLine("FORMAT TIME: {0}", ret);

            return ret;
        }

        public string FormatTimeByDiff(int time)
        {
            int now = GetUnixTimestamp(DateTime.Now);
            //Console.WriteLine("FORMAT DIFF: {0} :: {1}", time, now);

            int sub = now - time;
            int s = now - sub;

            //Console.WriteLine("SUB: {0} :: {1}", sub, s);

            TimeSpan ts = new TimeSpan(0, 0, s);

            //Console.WriteLine("DIFF: {0}:{1}", ts.Minutes, ts.Seconds);

            bool neg = false;
            if (sub < 0)
            {
                neg = true;
                sub = sub * -1;
            }

            //int m = sub / 60;
            //int s = sub % 60;

            string ret = "";
            string min = "";
            string sec = "";
            string ago = "";
            //Console.WriteLine("MIN: {0} :: SEC: {1}", m, s);

            if (ts.Minutes > 0)
            {
                min = " " + ts.Minutes.ToString() + " minute" + getPlural(ts.Minutes);
            }

            if (ts.Seconds > 0)
            {
                sec = " " + ts.Seconds.ToString() + " second" + getPlural(ts.Seconds);
            }

            if (neg == true)
            {
                ago = " ago";
            }

            ret = min + sec + ago;
            //Console.WriteLine("FORMAT TIME: {0}", ret);

            return ret;
        }

        public void PrintStringTable(string[] tbl)
        {
            Console.WriteLine("==== Printing String Table ====");
            foreach (string str in tbl)
            {
                Console.WriteLine("-- {0}", tbl);
            }
            Console.WriteLine("========================");
        }

        public static void PrintDictTable(Dictionary<int, string> dict)
        {
            Console.WriteLine("==== Printing Dictionary Table ====");
            foreach (KeyValuePair<int, string> pair in dict)
            {
                Console.WriteLine("-- {0} :: {1}", pair.Key, pair.Value);
            }
            Console.WriteLine("========================");
        }
        


        //SQLite Stuff

        public void Strip()
        {

        }

        public void UnStrip()
        {

        }
    }
    
}
