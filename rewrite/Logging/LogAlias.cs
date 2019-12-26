namespace Orikivo
{
    /// <summary>
    /// A shortcut reference that points to a specified object.
    /// </summary>
    public enum LogAlias
    {
        /// <summary>
        /// Points to the <see cref="OriGlobal.ClientName"/> of a log event.
        /// </summary>
        Name = 1,

        /// <summary>
        /// Points to the <see cref="System.DateTime"/> of a log event.
        /// </summary>
        Date = 2,

        /// <summary>
        /// Points to the <see cref="Discord.LogMessage.Message"/> of a log event.
        /// </summary>
        LogMessage = 3,

        /// <summary>
        /// Points to the <see cref="System.Exception"/> of a log event, if one is specified.
        /// </summary>
        Exception = 4,

        /// <summary>
        /// Points to the <see cref="System.Exception.GetType"/> of a log event, if one is specified.
        /// </summary>
        ExceptionType = 5,

        /// <summary>
        /// Points to the <see cref="System.Exception.Message"/> of a log event, if one is specified.
        /// </summary>
        ExceptionMessage = 6,

        /// <summary>
        /// Points to the <see cref="Discord.LogSeverity"/> of a log event.
        /// </summary>
        LogSeverity = 7,

        /// <summary>
        /// Points to the <see cref="Discord.LogMessage.Source"/> of a log event.
        /// </summary>
        LogSource = 8,

        /// <summary>
        /// Points to the <see cref="OriGlobal.ClientVersion"/> of a log event.
        /// </summary>
        ClientVersion = 9
    }
}
