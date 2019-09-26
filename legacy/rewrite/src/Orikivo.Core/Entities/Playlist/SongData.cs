using System;

namespace Orikivo
{
    public class SongData
    {
        public string Uploader {get; set;} // uploader name
        public string Description {get; set;} // video desc
        public DateTime UploadDate {get; set;} // date the video was uploaded
        public ulong Upvotes {get; set;} // likes
        public ulong Downvotes {get; set;} // dislikes
        public string Title {get; set;} // video title
        public TimeSpan Duration {get; set;} // length of video
        public ulong Views {get; set;} // view count
    }
}