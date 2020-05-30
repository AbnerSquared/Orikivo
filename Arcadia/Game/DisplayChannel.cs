using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia

{

    public class LobbyDisplayChannel : DisplayChannel
    {
        public LobbyDisplayChannel()
        {
            Frequency = 0;
            RefreshRate = TimeSpan.FromSeconds(1);
            Content = new DisplayContent { Value = $"New Display Channel, Frequency {Frequency}" };

            var textInputs = new List<TextInput>();
            var reactionInputs = new List<ReactionInput>();


            TextInputs = textInputs;

            ReactionInputs = reactionInputs;
        }
    }

    public class DisplayChannel
    {
        internal DisplayChannel() { }

        public DisplayChannel(int frequency, DisplayContent content, TimeSpan? refreshRate = null)
        {
            if (frequency == 0)
                throw new Exception("This frequency is reserved for the lobby.");

            Frequency = frequency;
            RefreshRate = refreshRate ?? TimeSpan.FromSeconds(1);
            Content = content;

            TextInputs = new List<TextInput>();
            ReactionInputs = new List<ReactionInput>();
        }

        // what frequency is this display broadcasting to?
        public int Frequency; // default is 0, the lobby.
        // the lobby is always there

        // how fast can this display update? minimum of 1 second.
        public TimeSpan RefreshRate;

        // what is this display currently showing?
        public DisplayContent Content;

        // what can the player currently type in this display channel?
        public List<TextInput> TextInputs;

        // what can the player currently react with in this display channel?
        public List<ReactionInput> ReactionInputs;
    }
}
