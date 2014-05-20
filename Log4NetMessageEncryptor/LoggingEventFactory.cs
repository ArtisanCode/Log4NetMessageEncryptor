
using log4net.Core;
using System;
using System.Threading;
namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class LoggingEventFactory : ILoggingEventFactory
    {
        /// <summary>
        /// Creates the error event to forward to other appenders indicating an error with this component.
        /// </summary>
        /// <param name="ErrorMessage">The error message.</param>
        /// <returns>
        /// A new logging message that can be forwarded to the other appenders and logged.
        /// </returns>
        public LoggingEvent CreateErrorEvent(string ErrorMessage)
        {
            var data = new LoggingEventData()
            {
                Domain = "ArtisanCode.Log4NetMessageEncryptor",
                ExceptionString = ErrorMessage,
                Level = Level.Error,
                LoggerName = "ArtisanCode.Log4NetMessageEncryptor",
                Message = "An unexpected error occurred within the ArtisanCode.Log4NetMessageEncryptor component",
                ThreadName = Thread.CurrentThread.Name,
                TimeStamp = DateTime.Now,
                UserName = Environment.UserName
            };

            return new LoggingEvent(data);
        }

        /// <summary>
        /// Creates the encrypted logging event.
        /// </summary>
        /// <param name="source">The source logging event to use.</param>
        /// <param name="encryptedLoggingMessage">The encrypted logging message to set within the LoggingEvent.</param>
        /// <returns>A cloned instance of LoggingEvent with the message property set to the encryptedLoggingMessage</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public LoggingEvent CreateEncryptedLoggingEvent(LoggingEvent source, string encryptedLoggingMessage)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var sourceEventData = source.GetLoggingEventData();
            sourceEventData.Message = encryptedLoggingMessage;

            return new LoggingEvent(sourceEventData);
        }
    }
}
