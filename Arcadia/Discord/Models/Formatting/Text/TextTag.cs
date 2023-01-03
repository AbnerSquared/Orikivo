using System;

namespace Arcadia
{
    // These are used to set placeholder text values in a written piece of text.
    // Hello there, {user}.
    // new TextTag { Alias = "user", Value = reference => reference    }
    public class TextTag<T>
    {
        public string Alias { get; set; }

        public Func<T, string> Writer { get; set; }
    }
}
