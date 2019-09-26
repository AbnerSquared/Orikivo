using System.Collections.Generic;

namespace Orikivo.Systems.Presets
{
    public class Exceptions
    {
        /// <summary>
        /// Throws an error set when required.<para/>
        /// Connection Exceptions [1]:<para/>
        ///     1a: This error is thrown when Orikivo fails to connect to Discord servers.<para/>
        ///     1b: This error is thrown when a token required to connect is incorrect, or doesn't exist.<para/>
        /// Command Exceptions [2]:<para/>
        ///     2a: This error is thrown when a command called doesn't exist in the collection of modules.<para/>
        ///     2b: This error is thrown when a variable in a command doesn't exist in the task.<para/>
        ///     2c: This error is thrown when a command fails to execute.<para/>
        /// </summary>
        /// <param name="Error">Specifies the error to initiate.</param>
        /// <returns></returns>
        public static string ThrowError(string Error)
        {
            Error = Error.ToLower();
            string errorReturn = "The ExceptionHandler could not find the error mentioned.";
            string errorBase = $"error#{Error} : ";

            var errorDictionary = new Dictionary<string, string>
            {
                {"1a", "Orikivo could not connect for unknown reasons."},
                {"1b", "The token provided for Orikivo is invalid."},
                {"2a", "The command you called for does not exist."},
                {"2b", "The variable you specified does not exist."},
                {"2c", "The command received ran into an error."},
            };

            foreach (var errorKey in errorDictionary.Keys)
            {
                if (Error == errorKey)
                {
                    errorReturn = errorBase + errorDictionary[$"{errorKey}"];
                    break;
                }
            }
            return errorReturn;
        }
    }
}