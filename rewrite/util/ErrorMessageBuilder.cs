namespace Orikivo
{
    public class ErrorMessageBuilder
    {
        public string Reaction { get; set; } // Oops! Yikes!

        public string Title { get; set; } // An error has occurred.

        public string Reason { get; set; } // Null.

        public string StackTrace { get; set; } // at C:/ok.cs: line 1

        public OriColor Color { get; set; } // can be left empty.
        public Message Build()
            => new Message(this);
    }
}
