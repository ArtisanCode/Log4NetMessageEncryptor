using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ArtisanCode.Log4NetMessageEncryptor.KeyGen
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Generating a new key for Log4Net.MessageEncryptor: ");

            // Read the configuration file for the key size information
            Log4NetMessageEncryptorConfiguration config = (Log4NetMessageEncryptorConfiguration)ConfigurationManager.GetSection("Log4NetMessageEncryption");

            using (RijndaelManaged cryptoContainer = new RijndaelManaged())
            {
                cryptoContainer.KeySize = config.EncryptionKey.KeySize;

                // Generates a new key using the standard .NET method of generating a new symmetric key
                cryptoContainer.GenerateKey();

                var key = Convert.ToBase64String(cryptoContainer.Key);

                // Output the new key to the screen and the clipboard
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(key);
                Clipboard.SetText(key);

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("This Key has been copied to your clipboard.");
            }

            Console.WriteLine();
            Console.WriteLine("Please press any key to exit.");
            Console.ReadKey();
        }
    }
}
