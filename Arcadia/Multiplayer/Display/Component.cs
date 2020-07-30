using System;
using System.Linq;

namespace Arcadia.Multiplayer
{
    // a single string
    public class Component : IComponent
    {
        public string Id { get; internal set; }
        
        public bool Active { get; set; }

        public int Position { get; set; }

        public ComponentFormatter Formatter { get; internal set; }

        public string Value { get; internal set; }

        public string Buffer { get; protected set; }

        public string Draw()
        {
            Buffer = "";

            if (Formatter == null)
            {
                if (Value != null)
                {
                    Buffer = Value;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Formatter.BaseFormatter))
                    Formatter.BaseFormatter = "{0}";

                if (Formatter.OverrideBaseValue)
                    throw new Exception("The base index reference was marked as an override but is missing an object");

                Buffer = string.Format(Formatter.BaseFormatter, Value);
            }

            return Buffer;
        }

        // since the only information you would need is provided in the game server, keep the method local to that
        // instead of using arguments
        public virtual string Draw(GameServer server)
            => Draw();

        public string Draw(params object[] args)
        {
            Buffer = "";

            if (Formatter == null)
            {
                if (Value != null)
                {
                    Buffer = Value;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Formatter.BaseFormatter))
                    Formatter.BaseFormatter = "{0}";


                if (Formatter.OverrideBaseValue)
                {
                    if (args == null || args?.Length == 0)
                        throw new System.Exception("The base index reference was marked as an override but is missing an object");
                }

                var argBuffers = args.Select(x => x.ToString());

                if (!Formatter.OverrideBaseValue)
                    argBuffers = argBuffers.Prepend(Value);

                Console.WriteLine(argBuffers.Count());
                Console.WriteLine(Formatter.GetArgCount());
                Console.WriteLine(Formatter.BaseFormatter);
                Console.WriteLine(string.Join("\n", argBuffers.Select(x => x?.ToString())));

                var argArrays = argBuffers.ToArray();                
                Buffer = string.Format(Formatter.BaseFormatter, argArrays.ToArray());
            }

            return Buffer;
        }

        public void Set(string value)
        {
            Value = value;
        }
    }
}
