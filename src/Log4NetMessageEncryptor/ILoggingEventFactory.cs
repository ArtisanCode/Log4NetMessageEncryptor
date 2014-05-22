using log4net.Core;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public interface ILoggingEventFactory
    {
        /// <summary>
        /// Creates the encrypted logging event.
        /// </summary>
        /// <param name="source">The source logging event to use.</param>
        /// <param name="encryptedLoggingMessage">The encrypted logging message to set within the LoggingEvent.</param>
        /// <param name="encryptedExceptionMessage">The encrypted exception message to set (if at all) within the LoggingEvent.</param>
        /// <returns>A cloned instance of LoggingEvent with the message property set to the encryptedLoggingMessage</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        LoggingEvent CreateEncryptedLoggingEvent(LoggingEvent source, string encryptedLoggingMessage, string encryptedExceptionMessage = null);

        /// <summary>
        /// Creates the error event to forward to other appenders indicating an error with this component.
        /// </summary>
        /// <param name="ErrorMessage">The error message.</param>
        /// <returns>A new logging message that can be forwarded to the other appenders and logged.</returns>
        LoggingEvent CreateErrorEvent(string ErrorMessage);
    }
}