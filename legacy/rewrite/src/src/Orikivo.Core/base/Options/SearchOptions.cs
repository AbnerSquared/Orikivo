namespace Orikivo
{
    /// <summary>
    /// Represents a collection of options for searches.
    /// </summary>
    public class SearchOptions
    {
        /// <summary>
        /// Constructs a new SearchOptions with optional specified values.
        /// </summary>
        public SearchOptions(string input, MatchValueHandling? mode = null, CaseFormat? casing = null)
        {
            Input = input;
            Mode = mode ?? Default.Mode;
            Casing = casing ?? Default.Casing;
        }

        /// <summary>
        /// Constructs a new SearchOptions with default values.
        /// </summary>
        public SearchOptions()
        {
            Input = null;
            Mode = MatchValueHandling.Equals;
            Casing = CaseFormat.Lowercase;
        }

        public static SearchOptions Default = new SearchOptions();
        public string Input { get; set; }
        public MatchValueHandling Mode { get; set; }
        public CaseFormat Casing { get; set; }

        public SearchOptions WithInput(string s)
        { SetInput(s); return this; }
        public SearchOptions WithMode(MatchValueHandling m)
        { SetMode(m); return this; }
        public SearchOptions WithCasing(CaseFormat c)
        { SetCasing(c); return this; }
        public void SetInput(string s)
            => Input = s;
        public void SetMode(MatchValueHandling m)
            => Mode = m;
        public void SetCasing(CaseFormat c)
            => Casing = c;
    }
}