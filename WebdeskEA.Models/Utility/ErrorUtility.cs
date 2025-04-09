using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebdeskEA.Models.Utility
{
    public static class ErrorUtility
    {
        /// <summary>
        /// Gets the full exception message including all inner exceptions.
        /// </summary>
        /// <param name="ex">The exception to process.</param>
        /// <returns>A string containing all exception messages concatenated.</returns>
        public static string GetFullExceptionMessage(Exception ex)
        {
            var messages = new List<string>();
            var currentException = ex;

            while (currentException != null)
            {
                messages.Add(currentException.Message);
                currentException = currentException.InnerException;
            }

            return string.Join(" --> ", messages);
        }

        /// <summary>
        /// Generates a unique error code for the given exception.
        /// </summary>
        /// <param name="ex">The exception to generate an error code for.</param>
        /// <returns>A string representing a unique error code.</returns>
        /// <summary>
        /// Generates a specific error code and HTTP status code for known exceptions or a generic code for unknown ones.
        /// </summary>
        /// <param name="ex">The exception to generate an error code for.</param>
        /// <returns>A tuple containing the error code and corresponding HTTP status code.</returns>
        public static (string ErrorCode, int StatusCode) GenerateErrorCodeAndStatus(Exception ex)
        {
            if (ex is DivideByZeroException)
            {
                return ("ERR_DIVIDE_BY_ZERO", 500); // Internal Server Error
            }
            else if (ex is NullReferenceException)
            {
                return ("ERR_NULL_REFERENCE", 500); // Internal Server Error
            }
            else if (ex is InvalidOperationException)
            {
                return ("ERR_INVALID_OPERATION", 400); // Bad Request
            }
            else if (ex is ArgumentException)
            {
                return ("ERR_ARGUMENT", 400); // Bad Request
            }
            else if (ex is FileNotFoundException)
            {
                return ("ERR_FILE_NOT_FOUND", 404); // Not Found
            }
            // Add more specific exceptions as needed

            // Default generic error code for unknown exceptions
            return ("ERR_GENERIC", 500); // Internal Server Error
        }
    }
}
