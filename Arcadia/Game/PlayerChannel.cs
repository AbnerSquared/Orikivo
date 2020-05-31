using Discord;

namespace Arcadia

{
    public class PlayerChannel
    {
        // who is the user i am bound to?
        public IUser User;

        // what message should i update?
        public IUserMessage InternalMessage;

        // what is the frequency of the display i am currently looking for?
        public int Frequency;
    }
}
