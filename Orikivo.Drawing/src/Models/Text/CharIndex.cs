namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents the index of a char that correlates with a character grid.
    /// </summary>
    public struct CharIndex
    {
        internal static CharIndex FromResult(char c, int? i, int? x, int? y) => new CharIndex(c, i.HasValue && x.HasValue && y.HasValue, i, y, x);

        private CharIndex(char c, bool isSuccess, int? page = null, int? row = null, int? column = null)
        {
            Char = c;
            IsSuccess = isSuccess;

            Page = page.GetValueOrDefault(0);
            Row = row.GetValueOrDefault(0);
            Column = column.GetValueOrDefault(0);
        }

        /// <summary>
        /// The <see cref="char"/> value that resulted in the index match.
        /// </summary>
        public char Char { get; }

        /// <summary>
        /// Returns a <see cref="bool"/> that specifies if this search was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Represents the page index.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Represents the row index.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Represents the column index.
        /// </summary>
        public int Column { get; }

    }
}
