using System.Collections.Generic;

namespace Orikivo
{
    public class Playlist
    {
        public Playlist()
        {
            Name = "New Playlist";
        //    Songs = new Queue<Song>();
        }
        public Playlist(string name = null)
        {
            Name = name;
        }

        //public Playlist(ServerQueue q)
        //{

       // }

        public string Name {get; set;}
        //public Queue<Song> Songs {get; set;}
    }
}