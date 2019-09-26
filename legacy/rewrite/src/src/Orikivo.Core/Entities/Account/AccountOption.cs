namespace Orikivo
{
    /// <summary>
    /// A collection of configurable options in a Account.
    /// </summary>
    public enum AccountOption
    {
        /// <summary>
        /// The ability for all commands to automatically be corrected upon an error.
        /// </summary>
        AutoFix = 1,

        /// <summary>
        /// The ability to automatically correct overspending on funds.
        /// </summary>
        Overflow = 2,

        /// <summary>
        /// The nickname set.
        /// </summary>
        Nickname = 4,

        /// <summary>
        /// The base prefix for a command execution.
        /// </summary>
        Prefix = 8,

        /// <summary>
        /// The closing prefix of a command.
        /// </summary>
        ClosingPrefix = 16,

        /// <summary>
        /// A collection of banned words.
        /// </summary>
        LocaleBlacklist = 32,

        /// <summary>
        /// A collection of banned websites.
        /// </summary>
        SiteBlacklist = 64,

        /// <summary>
        /// If links are allowed to be sent.
        /// </summary>
        AllowLinks = 128,

        /// <summary>
        /// If errors are public.
        /// </summary>
        Exceptions = 256,

        /// <summary>
        /// The unsafe for work shield.
        /// </summary>
        SafeGuard = 512,

        /// <summary>
        /// How executed results are shown.
        /// </summary>
        OutputFormat = 1024,

        /// <summary>
        /// How empty or null items are formateed.
        /// </summary>
        NullFormat = 2048,

        /// <summary>
        /// The visibility set.
        /// </summary>
        Visibility = 4096,

        /// <summary>
        /// The raw insult power of Orikivo.
        /// </summary>
        SledgePower = 8192,

        /// <summary>
        /// How icons are formatted.
        /// </summary>
        IconFormat = 16384,

        /// <summary>
        /// The language used on Orikivo.
        /// </summary>
        Locale = 32768,

        /// <summary>
        /// If the string should be encoded before decoding.
        /// </summary>
        PreDecode = 65536,
        /// <summary>
        /// If all strings sent are decoded by default.
        /// </summary>
        GlobalDecoder = 131072,

        /// <summary>
        /// If reversing strings consider directional characters.
        /// </summary>
        IgnoreDirectional = 262144,

        /// <summary>
        /// Decides if assisted tips are enabled.
        /// </summary>
        Tooltips = 524288,

        /// <summary>
        /// The format of all visible names.
        /// </summary>
        UsernameFormat = 1048576,

        /// <summary>
        /// The mode at which matches occur.
        /// </summary>
        MatchHandling = 2097152,

        /// <summary>
        /// The chance of losing in a wager.
        /// </summary>
        Risk = 4194304,

        /// <summary>
        /// The style in which Color formats are displayed.
        /// </summary>
        ColorFormat = 16777216,

        /// <summary>
        /// Defines if messages are checked for invalid language.
        /// </summary>
        WordGuard = 33554432,

        /// <summary>
        /// Controls how search results are considered.
        /// </summary>
        SearchHandling = 67108864,

        /// <summary>
        /// Defines if the account option name should be displayed alongside the icons.
        /// </summary>
        SymbolNameDisplay = 134217728
    }
}
