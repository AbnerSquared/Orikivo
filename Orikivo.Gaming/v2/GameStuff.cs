using Orikivo.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo.Gaming
{

    public class WerewolfInfoNode : ContentNode
    {
        [Node(0)]
        [Formatting("**{0}**")]
        public string SessionName { get; set; }

        [Node(1)]
        [Formatting("#{0}")]
        public string SessionId { get; set; }

        [Node(2)]
        public string Privacy { get; set; }

        [Node(3)]
        [Formatting(" [{0}]")]
        public string Password { get; set; }

        [Node(4)]
        [Formatting("**{0}**")]
        public string GameMode { get; set; }

        [Node(5)]
        [GroupFormatting("```autohotkey\n{0}\n```", "\n")]
        public List<MessageNode> Messages { get; set; }

        // make it possible to remove certain parts if they are null.
        protected override string Formatting => "> {0}{1} `{2}{3}` \n> ⇛ Playing {4}\n{5}";
    }

    public class MessageNode : ContentNode
    {
        [Node(0)]
        [Formatting("[{0}]: ")]
        public string Author { get; set; }

        [Node(1)]
        [Formatting("\"{0}\"")]
        public string Message { get; set; }

        protected override string Formatting => ":: {0}{1}";
    }

    public class Command
    {
        public string Name;
        List<Parameter> Parameters;
    }

    public class CommandParser // interpret commands here.
    {

    }

    public class Parameter
    {
        public Type Type;
        public string Name;
        public object DefaultValue;
        public ParamMod Mods;
    }

    public enum ParamMod
    {
        Optional = 1
    }

    public class GameSession
    {

    }

    public class Lobby // represents a queue of incoming players.
    {

    }

    public abstract class BaseGameClient
    {
        ConcurrentDictionary<string, GameSession> Sessions { get; set; }
        ConcurrentDictionary<ulong, INetworkReader> Readers { get; set; } // all readers to a specific game session
    }

    public interface INetworkReader // an identity that reads the game updates.
    {
        string SessionId { get; } // the game session a network is bound to
        Display CurrentDisplay { get; }
        Task Sync();
        bool IsSynced();
    }

    public class GameBuilder
    {
        public List<AttributePredicate> Predicates { get; set; }
        public List<GameAttribute> Attributes { get; set; }
        public List<Command> Commands { get; set; } // commands that can be called by the user at any time.
        public List<GameTaskBuilder> Tasks { get; set; }
    }

    public class GameTaskBuilder // a task is an internal game session 
    {
        public List<Command> Commands { get; set; } // commands that are only called when this task is active.
    }

    public interface IEntity<TId> where TId : struct
    {
       TId Id { get; }
    }


    public class GameLogger
    {

    }

    // the new way game sessions should be handled: instead of invoking and layering to tasks, keep it at the base level, and just reference the task properties from the base level.
    // this way, global attributes keep their worth, and other things can easily be passed on.
    public class __Example__
    {
        public void Test()
        {
            var predicate = new AttributePredicate("attribute1", x => x == (object)1);
            var attribute = new GameAttribute("attribute1", 1);
            var predicate_a = attribute.Predicate(x => x == (object)1);

            var predicate2 = new AttributePredicate<DateTime>("attribute2", x => x.Year == DateTime.UtcNow.Year);
            var attribute2 = new GameAttribute<DateTime>("attribute2", DateTime.UtcNow);
            var predicate_b = attribute2.Predicate(x => x.Month == DateTime.MaxValue.Month);

            Console.WriteLine(attribute.Check(predicate));
            Console.WriteLine(attribute2.Check(predicate2));
        }
    }
}
