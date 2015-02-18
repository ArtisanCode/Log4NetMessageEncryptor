using System;
using System.Linq;
using ArtisanCode.Log4NetMessageEncryptor;
using ArtisanCode.LogManager.Options;
using CommandLine;

namespace LogDecrypt
{
    internal class Program
    {
        /// <summary>
        ///     Log decrypt entry point.
        /// </summary>
        /// <param name="args">The arguments specified for the decryption.</param>
        private static void Main(string[] args)
        {
            try
            {
                var options = new LogDecryptOptions();

                if (!args.Any())
                {
                    // Handle the case when no arguments have been specified: prompt the user with the help screen
                    Console.WriteLine(options.GetUsage());
                    Environment.Exit(Parser.DefaultExitCodeFail);
                }

                if (Parser.Default.ParseArguments(args, options))
                {
                    var logDecryption = new DecryptionManager(new ConfigurationManagerHelper(), new FileHelper());

                    ValidationResult optionsValidationResult = logDecryption.AreOptionsValid(options);
                    if (optionsValidationResult.IsValid)
                    {
                        // If the inputs are valid, try to decrypt the log
                        logDecryption.DecryptLog(options);
                        Environment.Exit(0);
                    }

                    // Something in the inputs is wrong, inform the user and quit
                    Console.WriteLine(optionsValidationResult.ErrorMessage);
                    Environment.Exit(-2);
                }
                else
                {
                    Console.WriteLine("Unable to parse the input arguments, please check and try again.");
                    Console.WriteLine(options.GetUsage());
                    Environment.Exit(Parser.DefaultExitCodeFail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Oops, something went wrong! Please see the exception details below for more information.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("============================================");
                Console.WriteLine(ex.ToString());
                Environment.Exit(-3);
            }
        }
    }
}