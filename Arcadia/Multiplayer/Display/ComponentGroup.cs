using Orikivo;
using Orikivo.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    public class ContentGroup<TContent>
        where TContent : ContentNode
    {

    }

    // a list of strings
    public class ComponentGroup : IComponent
    {
        public string Id { get; internal set; }

        public bool Active { get; set; }

        public int Position { get; set; }

        public ComponentFormatter Formatter { get; internal set; }

        // set the initial capacity, cannot be changed once set
        public int Capacity { get; internal set; }

        // the array is initialized with the specified capacity.
        public string[] Values;


        // this represents the last draw method that was executed
        // in this case, the last time this component was rendered will be stored here
        // this is how the Draw() method comes into play, allowing you to set up the component
        // before updating all channels to show it
        public string Buffer { get; protected set; }

        // if there are no empty slots before the first specified index
        // it will shift everything in the array to the right
        // and set the smallest index to this value
        public void Prepend(string value)
        {
            var values = Values.Shift(1).ToArray();
            values[0] = value;

            Values = values;
            // if the existing size is == capacity
            // push all of the values up 1, removing the newest entry
            // insert the newest at the start
        }

        // if there are no empty slots after the last specified index
        // it will shift everything in the array to the left
        // and set the largest index to this value
        public void Append(string value)
        {
            var values = Values.Shift(-1).ToArray();
            values[Capacity - 1] = value;

            Values = values;
            // if the existing size is == capacity
            // push all of the values back 1, removing the oldest entry
            // insert the newest
        }

        // this will set a specific index to the specified value
        public void Set(int index, string value)
        {
            if (index < Capacity && index >= 0)
                Values[index] = value;
        }

        // this will set a specific value in its index to null
        public void Remove(int index)
        {
            if (index < Capacity && index >= 0)
                Values[index] = "";
        }

        // this will remove all values from the array
        public void Clear()
        {
            Values = new string[Capacity];
        }

        // this renders the component group
        public string Draw()
        {
            // clear the previous buffer from the component
            Buffer = "";
            // if there is no formatter, draw the simple element new line list
            // if there are no elements, ignore drawing this altogether
            if (Formatter == null)
            {
                if (Values != null)
                {
                    if (Values.Length > 0)
                    {
                        Buffer = string.Join('\n', Values);
                    }
                }
            }
            else
            {
                if (Formatter.OverrideBaseValue)
                    throw new Exception("The base index reference was marked as an override but is missing an enumerable");

                // If this formatter doesn't have any base formatters set,
                // this should be simply set to its default
                if (string.IsNullOrWhiteSpace(Formatter.BaseFormatter))
                    Formatter.BaseFormatter = "{0}";

                if (string.IsNullOrWhiteSpace(Formatter.ElementFormatter))
                    Formatter.ElementFormatter = "{0}";

                if (string.IsNullOrWhiteSpace(Formatter.Separator))
                    Formatter.Separator = "\n";

                // this is the rendered list of values
                string valueBuffer = string.Join(Formatter.Separator, Values.Select(x => string.Format(Formatter.ElementFormatter, x)));

                // this render is sent to the buffer, where it can be easily referenced with each channel update
                Buffer = string.Format(Formatter.BaseFormatter, valueBuffer);
            }

            return Buffer;
        }

        public virtual string Draw(GameServer server)
            => Draw();

        // this draws the new component group with specified arguments needed to fill it's place
        public string Draw(params object[] args)
        {
            // clear the previous buffer from the component
            Buffer = "";
            // if there is no formatter, draw the simple element new line list
            // if there are no elements, ignore drawing this altogether
            if (Formatter == null)
            {
                if (Values != null)
                {
                    if (Values.Length > 0)
                    {
                        Buffer = string.Join('\n', Values);
                    }
                }
            }
            else
            {
                // If this formatter doesn't have any base formatters set,
                // this should be simply set to its default
                if (string.IsNullOrWhiteSpace(Formatter.BaseFormatter))
                    Formatter.BaseFormatter = "{0}";

                if (string.IsNullOrWhiteSpace(Formatter.ElementFormatter))
                    Formatter.ElementFormatter = "{0}";

                if (string.IsNullOrWhiteSpace(Formatter.Separator))
                    Formatter.Separator = "\n";

                // this returns the number of required arguments
                // extra args can be ignored
                int argCount = Formatter.GetArgCount();

                // likewise, if enough arguments aren't specified, they can be left as is
                string valueBuffer = "";

                // if override base index is true
                // ensure that the first specified argument is written
                if (Formatter.OverrideBaseValue)
                {
                    // if the array is null or there are no elements, throw an error
                    // this requires at least 1 argument to be specified since it was marked as an override
                    if (args == null || args?.Length == 0)
                        throw new Exception("The base index reference was marked as an override but is missing an enumerable");

                    // this now determines if the first argument specified is an enumerable
                    var values = args[0];
                    if (!(values is IEnumerable))
                        throw new InvalidCastException("The value specified could not be cast into an IEnumerable.");

                    valueBuffer = string.Join(Formatter.Separator,
                        ((IEnumerable)values).OfType<object>().Select(x => string.Format(Formatter.ElementFormatter, x)));
                    
                }
                else // otherwise, if the base index is to not be overridden
                {
                    // render the list of values specified in the component group
                    valueBuffer = string.Join(Formatter.Separator,
                        Values.Select(x => string.Format(Formatter.ElementFormatter, x)));
                }

                // this now sets up the argument buffer
                var argBuffers = args.Select(x => x.ToString());

                // if the base index was overridden
                // skip over the first element to remove the base value collection to use
                if (Formatter.OverrideBaseValue)
                    argBuffers = argBuffers.Skip(1);

                // now prepend the valueBuffer that was initialized
                argBuffers = argBuffers.Prepend(valueBuffer);

                Console.WriteLine(string.Join("\n", argBuffers));

                // finally, you can now properly format the string with the specified arguments
                // this render is sent to the buffer, where it can be easily referenced with each channel update
                Buffer = string.Format(Formatter.BaseFormatter, argBuffers.ToArray());
            }

            return Buffer;
        }
    }
}
