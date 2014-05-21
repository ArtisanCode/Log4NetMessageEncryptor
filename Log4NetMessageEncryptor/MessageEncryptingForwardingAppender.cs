using ArtisanCode.Log4NetMessageEncryptor.Encryption;

using log4net.Appender;
using log4net.Core;
using System;
using System.Linq;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class MessageEncryptingForwardingAppender : ForwardingAppender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEncryptingForwardingAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Default constructor. This instantiates the class with all default dependencies
        /// </remarks>
        public MessageEncryptingForwardingAppender()
        {
            MessageEncryption = new RijndaelMessageEncryptor();
            LogEventFactory = new LoggingEventFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEncryptingForwardingAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Allows the class dependencies to be injected into the class
        /// </remarks>
        /// <param name="messageEncryption">The message encryption engine to use.</param>
        public MessageEncryptingForwardingAppender(IMessageEncryptor messageEncryption, ILoggingEventFactory logEventFactory)
        {
            this.MessageEncryption = messageEncryption;
            this.LogEventFactory = logEventFactory;
        }

        /// <summary>
        /// Gets or sets the log event factory.
        /// </summary>
        /// <value>
        /// The log event factory.
        /// </value>
        public ILoggingEventFactory LogEventFactory { get; set; }

        /// <summary>
        /// Gets or sets the message encryption engine.
        /// </summary>
        /// <value>
        /// The message encryption engine.
        /// </value>
        public IMessageEncryptor MessageEncryption { get; set; }

        /// <summary>
        /// Actions the append.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        public void ActionAppend(LoggingEvent loggingEvent)
        {
            var eventWithEncryptedMessage = GenerateEncryptedLogEvent(loggingEvent);

            base.Append(eventWithEncryptedMessage);
        }

        /// <summary>
        /// Actions the append.
        /// </summary>
        /// <param name="loggingEvents">The logging events.</param>
        public void ActionAppend(LoggingEvent[] loggingEvents)
        {
            var encryptedEvents = loggingEvents.Select(x => GenerateEncryptedLogEvent(x)).ToArray();

            base.Append(loggingEvents);
        }

        /// <summary>
        /// Generates the encrypted log event.
        /// </summary>
        /// <param name="source">The source logging event.</param>
        /// <returns>The source logging event with the message encrypted accordingly</returns>
        public LoggingEvent GenerateEncryptedLogEvent(LoggingEvent source)
        {
            LoggingEvent result;

            try
            {
                var encryptedMessage = MessageEncryption.Encrypt(source.RenderedMessage);

                string exceptionString = source.GetExceptionString();
                string encryptedExceptionMessage = null;

                if(!string.IsNullOrWhiteSpace(exceptionString))
                {
                    encryptedExceptionMessage = MessageEncryption.Encrypt(exceptionString);
                }

                result = LogEventFactory.CreateEncryptedLoggingEvent(source, encryptedMessage, encryptedExceptionMessage);
            }
            catch (Exception ex)
            {
                // Ensure that the logging encryption never fails with an unexpected exception, rather, create an error
                // log event so that can be logged instead. This is to ensure that we aren't inadvertently leaking
                // sensitive data in our logs if an error occurs, better to log nothing than leak data!
                result = LogEventFactory.CreateErrorEvent(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Forward the logging event to the attached appenders
        /// </summary>
        /// <param name="loggingEvent">The event to log.</param>
        /// <remarks>
        /// Delivers the logging event to all the attached appenders.
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            ActionAppend(loggingEvent);
        }

        /// <summary>
        /// Forward the logging events to the attached appenders
        /// </summary>
        /// <param name="loggingEvents">The array of events to log.</param>
        /// <remarks>
        /// Delivers the logging events to all the attached appenders.
        /// </remarks>
        protected override void Append(LoggingEvent[] loggingEvents)
        {
            ActionAppend(loggingEvents);
        }
    }
}