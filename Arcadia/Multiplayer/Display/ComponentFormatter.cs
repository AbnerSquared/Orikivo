using System.Collections.Generic;

namespace Arcadia
{
    public class ComponentFormatter
    {
        // For IComponent
        // when this formatter is set on a component, BaseFormatter is the only thing that is used
        // if you are designing a formatter that will require the input of other components,
        // it must be specified
        // In a ComponentGroup, {0} will always be reserved for the base group container,
        // but if the element container will not be used, it can be overridden
        // Referencing the base index can be completely ignored
        public string BaseFormatter { get; set; }

        // ForComponentGroup
        public string Separator { get; set; }

        // ForComponentGroup
        // An element formatter cannot have extra arguments attached to it
        // This might change later, but for now, let's keep it simple
        public string ElementFormatter { get; set; }

        // if this is set to true for a ComponentGroup, drawing the component requires a list to reference from
        // by default however, this will be set to false
        // If this is set to true, the first argument specified will be the list that will be formatted
        public bool OverrideBaseIndex { get; set; } = false;

        // this returns the number of required arguments needed to properly use this formatter
        // if OverrideBaseIndex is set to true, this also increases its number by 1
        public int GetArgCount()
        {
            // set up the initial argument count
            // this is the number of the total unique argument counts
            int uniqueArgCount = 0;

            // this is the last index read
            int lastIndex = 0;

            // this is the highest index specified
            int highestIndex = 0;

            // this is the list of all unique indexes
            // if indexes ever contains
            var indexes = new List<int>();

            // now read each specified argument in the baseFormatter to see how many there are
            // each argument is specified by {n}, where n is any positive integer in increasing increment

            // this determines if the parser is currently in an index placeholder for an arg
            bool inPlaceholder = false;

            // this determines if a opening token should be ignored if the last character was an escape token
            bool ignore = false;

            // if this is set to true, it constantly looks to find the end of the current token
            bool skipClose = false;

            // this is all of the numbers specified in the current placeholder token
            string placeholderIndex = "";

            // iterate through each character to determine the argument count
            // ignore and return if incorrectly formatted
            foreach (char c in BaseFormatter)
            {
                if (skipClose)
                {
                    // if you finally find an end token
                    // and the force close, and continue
                    if (c == '}')
                    {
                        skipClose = false;
                        inPlaceholder = false;
                        continue;
                    }
                }

                // if they are currently in a place holder, proceed
                if (inPlaceholder)
                {
                    // if this character is a specified number, proceed and continue
                    if (char.IsNumber(c))
                    {
                        // set the assumed number to be this
                        placeholderIndex += c;
                    } // if an end token is found, end the placeholder and determine if the parsed number is correct.
                    else if (c == '}')
                    {
                        inPlaceholder = false;

                        // if the current index is 1 higher than the base index, increment the arg count and continue
                        // if this is out of position, skip and continue to find the number there
                        // 
                        if (int.TryParse(placeholderIndex, out int index))
                        {
                            // if the indexes already contains this index, ignore it can continue;
                            if (indexes.Contains(index))
                                continue;

                            // if the specified index is 0
                            // if argCount is 0,
                            // and if OverrideBaseIndex is true
                            // allow the argument count to include this
                            if (index == 0 && uniqueArgCount == 0 && OverrideBaseIndex)
                            {
                                uniqueArgCount++;

                                if (index > highestIndex)
                                    highestIndex = index;

                                indexes.Add(index);
                            } // afterwards, now the new check should simply check to see if the new index is 1 higher than the last
                            else if (index == lastIndex + 1)
                            {
                                // if it is, tick up 1 to the arg count
                                uniqueArgCount++;

                                // and set the last index to the current index
                                lastIndex = index;

                                if (index > highestIndex)
                                    highestIndex = index;

                                indexes.Add(index);
                            }
                            else
                            {
                                // otherwise, if the index specified is anything lower or equal to the last index
                                // simply skip and continue

                                //else if (index <= lastIndex)
                                //    continue;

                                if (index > highestIndex)
                                    highestIndex = index;

                                indexes.Add(index);
                            }
                        }

                        placeholderIndex = "";
                    } // otherwise, if this character is not a closer or a number, simply ignore everything in it until it closes
                    else
                    {
                        // if this character is not a number, mark this as a skipClose
                        // as this is an invalid argument token
                        skipClose = true;
                    }

                    // proceed to the next character
                    continue;
                }

                // if they find an opening index
                if (c == '{')
                {
                    // if they were in an ignore sequence
                    // prevent opening a new placeholder
                    if (ignore)
                    {
                        ignore = false;
                    }
                    else
                    {
                        // mark them as inside a placeholder and continue
                        inPlaceholder = true;
                    }

                    continue;
                }

                // if an escape character was specified
                // allow an ignore sequence
                if (c == '\\')
                {
                    ignore = true;
                    continue;
                }

                // otherwise, if nothing else was done with the ignore sequence
                // this can simply be set to false
                if (ignore)
                    ignore = false;
            }

            // no matter the indexes, i think this should be autohandling index specifications
            // the highest index specified is the number of required arguments
            int argCount = highestIndex + 1;

            // if OverrideBaseIndex is set to false however, subtract 1 from the argCount as 0 is not included
            if (!OverrideBaseIndex)
                argCount--;

            return argCount;
        }
    }
}
