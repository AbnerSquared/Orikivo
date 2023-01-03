namespace Arcadia
{
    public enum CharacterLimitTarget
    {
        Content = 1, // The character limit is applied to the entire content length
        Page = 2, // The character limit is applied to the current page length 
        Element = 3, // The character limit is applied to the current element length
        Row = 4 // The character limit is applied to the current line length
    }
}
