namespace Orikivo.Drawing
{
    /// <summary>
    /// An indexable value that correlates with a <see cref="char[][][][]"/> grid collection.
    /// </summary>
    public struct CharMapIndex
    {
        internal static CharMapIndex FromResult(char c, int? i, int? x, int? y) => new CharMapIndex(c, i.HasValue && x.HasValue && y.HasValue, i, y, x);

        private CharMapIndex(char c, bool isSuccess, int? page = null, int? row = null, int? column = null)
        {
            Char = c;
            IsSuccess = isSuccess;

            if (isSuccess)
            {
                Page = page.Value;
                Row = row.Value;
                Column = column.Value;
            }
            else
            {
                Page = 0;
                Row = 0;
                Column = 0;
            }
        }

        /// <summary>
        /// The <see cref="char"/> value that resulted in the index match.
        /// </summary>
        public char Char { get; }

        /// <summary>
        /// Returns true if the operation was a success; Otherwise, false.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Represents the char[Page][][][] value.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Represents the char[][Row][][] value.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Represents the char[][][Column][] value.
        /// </summary>
        public int Column { get; }

    }
}
