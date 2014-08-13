using ArtisanCode.LogManager.Options;
using System;
using System.Linq;

namespace ArtisanCode.LogManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string invokedVerb = string.Empty;
            object invokedVerbInstance = null;

            var options = new LogManagerOptions();

            if (!args.Any())
            {
                Console.WriteLine(options.GetUsage());
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (!CommandLine.Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
                  // if parsing succeeds the verb name and correct instance
                  // will be passed to onVerbCommand delegate (string,object)
                  invokedVerb = verb;
                  invokedVerbInstance = subOptions;
              }))
            {
                switch (invokedVerb.ToLower())
                {
                    case "encrypt":
                        {
                            Console.WriteLine(new EncryptOptions().GetUsage(invokedVerb));
                            break;
                        }
                    case "decrypt":
                        {
                            Console.WriteLine(new EncryptOptions().GetUsage(invokedVerb));
                            break;
                        }
                    default:
                        {
                            Console.WriteLine(options.GetUsage());
                            break;
                        }
                }
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            if (invokedVerb == "encrypt")
            {
                var commitSubOptions = (EncryptOptions)invokedVerbInstance;
            }
            else if (invokedVerb == "decrypt")
            {
                var commitSubOptions = (DecryptOptions)invokedVerbInstance;
            }
            else
            {
                Console.WriteLine(options.GetUsage());
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
        }

        public void Encrypt(EncryptOptions options)
        {

        }

        public void Decrypt(DecryptOptions options)
        {

        }
    }
}
