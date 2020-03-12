using Discord;

namespace Orikivo.Gaming.Unstable
{
    public class InputController<TKey>
    {

    }

    public class GameClient
    {

    }

    public class Player
    {
        public Identity Identity;
        public GameAttribute[] Attributes;
    }

    public class GameAttribute
    {
        public string Id;
        public object Value;
        public object DefaultValue;
        public object PreviousValue;
    }

    public class GameDetails
    {
        public string Id;
        public string Name;
        public string Summary;
        public int RequiredPlayerCount;
        public int PlayerLimit;
    }

    public abstract class Game
    {
        public GameDetails Details;
    }

    public class Input<TKey>
    {
        public virtual void OnInput(Identity invoker, TKey value) { }
    }

    // the result from an input controller
    public class Output<TKey>
    {
        public Identity Invoker;
        public TKey Value;
    }

    public class Identity
    {
        private readonly IUser _user;

        public ulong Id => _user.Id;
    }

    public class Receiver
    {

    }

    public abstract class Component
    {
        public abstract string Draw();
    }
}
