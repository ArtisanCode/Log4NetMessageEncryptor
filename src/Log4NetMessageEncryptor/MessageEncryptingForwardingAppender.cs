using ArtisanCode.SimpleAesEncryption;
using log4net.Appender;
using log4net.Core;
using System;
using System.Configuration;
using System.Linq;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class MessageEncryptingForwardingAppender : ForwardingAppender
    {
        /// <summary>
        /// Backing store for the ConfigurationHelper property
        /// </summary>
        protected IConfigurationManagerHelper _configHelper = new ConfigurationManagerHelper();

        /// <summary>
        /// Sets the default config section name for the encryption settings
        /// </summary>
        protected const string DEFAULT_CONFIG_SECTION_NAME = "Log4NetMessageEncryption";

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEncryptingForwardingAppender"/> class.
        /// </summary>
        /// <remarks>
        /// Default constructor. This instantiates the class with all default dependencies
        /// </remarks>
        public MessageEncryptingForwardingAppender()
        {
            // Use the default config section name
            var sectionName = LocateEncryptionConfigurationSection();

            MessageEncryption = new RijndaelMessageEncryptor(sectionName);
            LogEventFactory = new LoggingEventFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEncryptingForwardingAppender"/> class.
        /// </summary>
        /// <param name="configurationSection">The name of the configuration section that contains the encryption information.</param>
        /// <remarks>
        /// Default constructor. This instantiates the class with all default dependencies
        /// </remarks>
        public MessageEncryptingForwardingAppender(string configurationSection)
        {
            MessageEncryption = new RijndaelMessageEncryptor(configurationSection);
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
        /// Use an interface to access the configuration files
        /// </summary>
        public IConfigurationManagerHelper ConfigHelper
        {
            get { return _configHelper; }
            set { _configHelper = value; }
        }

        /// <summary>
        /// Actions the append.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        public virtual void ActionAppend(LoggingEvent loggingEvent)
        {
            var eventWithEncryptedMessage = GenerateEncryptedLogEvent(loggingEvent);

            base.Append(eventWithEncryptedMessage);
        }

        /// <summary>
        /// Actions the append.
        /// </summary>
        /// <param name="loggingEvents">The logging events.</param>
        public virtual void ActionAppend(LoggingEvent[] loggingEvents)
        {
            var encryptedEvents = loggingEvents.Select(x => GenerateEncryptedLogEvent(x)).ToArray();

            base.Append(encryptedEvents);
        }

        /// <summary>
        /// Generates the encrypted log event.
        /// </summary>
        /// <param name="source">The source logging event.</param>
        /// <returns>The source logging event with the message encrypted accordingly</returns>
        public virtual LoggingEvent GenerateEncryptedLogEvent(LoggingEvent source)
        {
            LoggingEvent result;

            try
            {
                var encryptedMessage = MessageEncryption.Encrypt(source.RenderedMessage);

                string exceptionString = source.GetExceptionString();
                string encryptedExceptionMessage = null;

                if (!string.IsNullOrWhiteSpace(exceptionString))
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
        /// Locates the Encryption configuration section required for the library to function.
        /// </summary>
        /// <returns>The configuration section name found </returns>
        public virtual string LocateEncryptionConfigurationSection()
        {
            // look to see if the default section is present
            var defaultSection = ConfigHelper.GetSection(DEFAULT_CONFIG_SECTION_NAME) as Log4NetMessageEncryptorConfiguration;
            if (defaultSection != null)
            {
                // There is a valid configuration object using the default section name, use this config entry.
                return DEFAULT_CONFIG_SECTION_NAME;
            }

            // The configuration cannot be found using the default section name, see if is named something else
            const string targetConfigType = "ArtisanCode.Log4NetMessageEncryptor.Log4NetMessageEncryptorConfiguration";

            // Open the configuration file to examine the <configSections> info
            var config = ConfigHelper.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sections = config.RootSectionGroup.Sections;

            for (int i = 0; i < sections.Count; i++)
            {
                // If a section entry contains the config file type, then use this section name to get the encryption info from
                if (sections[i].SectionInformation.Type.Contains(targetConfigType))
                {
                    return sections[i].SectionInformation.Name;
                }
            }

            throw new ConfigurationErrorsException("Unable to locate a configuration section of type: " + targetConfigType + Environment.NewLine + "Have you forgotten to add the necessary entry in the <configSections> part of the config file?");
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
