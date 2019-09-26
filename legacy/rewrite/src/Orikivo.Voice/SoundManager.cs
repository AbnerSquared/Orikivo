using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RunnerAttribute : Attribute {}

    public class SoundManager
    {
        private static string _FFMPEG_OPEN_STREAM_ = "-i \"{0}\" -ac 2 -f s16le -ar 48000 pipe:1";

        private static string _YDL_FORMAT_ = "bestaudio[ext=mp3]";

        private static string _YDL_GET_URL_ = $"-f '{_YDL_FORMAT_}' -g "+"\"{0}\"";
        private static string _YDL_DL_AUDIO_ = "-x --audio-format mp3 -o \"{0}\" \"{1}\"";

        //public Queue<Song> queue = new Queue<Song>();

        public ProcessStartInfo FFMPEG([Runner]string args) =>
            Manager.BuildProcessStartInfo("ffmpeg", args);
        
        public ProcessStartInfo YoutubeDL([Runner]string args) =>
            Manager.BuildProcessStartInfo("youtube-dl", args);

        public string GetStreamUrl(string url) =>
            Manager.ExecuteOutput(YoutubeDL(Resolve(_YDL_GET_URL_, url)));

        public Process BuildStream(string path) =>
            Manager.Execute(FFMPEG(Resolve(_FFMPEG_OPEN_STREAM_, path)));

        public Process GetSong(string url) =>
            BuildStream(GetStreamUrl(url));

        public string Resolve(string s, params string[] args) =>
            string.Format(s, args);
    }
}