using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;

namespace IceBot.Classes
{
    public class Playback
    {
        public static Process ffProc;
        public static Stream ffOut;

        public async static Task StopSound()
        {
            ffProc.Kill();
            ffOut.Close();

            ffProc = null;
            ffOut = null;

            await Task.Delay(0);
        }

        public async static Task SeekAudio()
        {
            

            await Task.Delay(0);
        }

        public async static Task PlaySound(string file, DiscordGuild guild)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{file}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if(ffProc != null || ffOut != null)
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
}
