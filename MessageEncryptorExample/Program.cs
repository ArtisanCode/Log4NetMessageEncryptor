using ArtisanCode.Log4NetMessageEncryptor;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtisanCode.MessageEncryptorExample
{
    class Program
    {
        private static ILog log = LogManager.GetLogger("MessageEncryptorExample");

        static void Main(string[] args)
        {
            // Ensure log4net is configured
            XmlConfigurator.Configure();

            Log4NetMessageEncryptorConfiguration config = (Log4NetMessageEncryptorConfiguration)ConfigurationManager.GetSection("Log4NetMessageEncryption");

            Console.WriteLine("Configuration found:");
            Console.WriteLine("Configuration.Padding - {0}", config.Padding);
            Console.WriteLine("Configuration.CipherMode - {0}", config.CipherMode);
            Console.WriteLine("Configuration.EncryptionKey.KeySize - {0}", config.EncryptionKey.KeySize);
            Console.WriteLine("Configuration.EncryptionKey.Key - {0}", config.EncryptionKey.Key);

            log.Debug("Debug message 1");
            log.DebugFormat("Debug message {0}a", 1);
            log.Debug("Debug Exception message", new ApplicationException("Debug message log"));

            log.Info("Info message 1");
            log.InfoFormat("Info message {0}a", 1);
            log.Info("Info Exception message", new ApplicationException("Info message log"));

            log.Warn("Warning message 1");
            log.WarnFormat("Warning message {0}a", 1);
            log.Warn("Warning Exception message", new ApplicationException("Debug message log"));

            log.Error("Error message 1");
            log.ErrorFormat("Error message {0}a", 1);
            log.Error("Error Exception message", new MemberAccessException("Error message log"));

            log.Fatal("Fatal message 1");
            log.FatalFormat("Fatal message {0}a", 1);
            log.Fatal("Fatal Exception message", new StackOverflowException("Fatal message log", new OutOfMemoryException("Out of memory inner exception")));

            Console.WriteLine("Please press any key to close...");
            Console.ReadKey();
        }
    }
}
