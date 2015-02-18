using System;
using System.IO;
using System.Linq;
using ArtisanCode.Log4NetMessageEncryptor;
using ArtisanCode.LogManager.Options;
using ArtisanCode.SimpleAesEncryption;

namespace LogDecrypt
{
    public class DecryptionManager
    {
        /// <summary>
        /// Sets the default config section name for the encryption settings
        /// </summary>
        public const string DEFAULT_CONFIG_SECTION_NAME = "Log4NetMessageEncryption";

        /// <summary>
        ///     Initializes a new instance of the <see cref="DecryptionManager" /> class.
        /// </summary>
        /// <param name="configurationManager">The helper to access the configuration manager.</param>
        /// <param name="fileHelper">The helper to access the file class.</param>
        public DecryptionManager(IConfigurationManagerHelper configurationManager, IFileHelper fileHelper)
        {
            ConfigurationManager = configurationManager;
            FileHelper = fileHelper;
        }

        public IConfigurationManagerHelper ConfigurationManager { get; set; }
        public IFileHelper FileHelper { get; set; }

        /// <summary>
        /// Decrypts the log.
        /// </summary>
        /// <param name="options">The options to use for decrypting the log.</param>
        public void DecryptLog(LogDecryptOptions options)
        {
            options.OutputPath = GenerateOutputPath(options);

            var encryptionConfiguration = GenerateEncryptionConfiguration(options);
            var decryptor = new RijndaelMessageDecryptor(encryptionConfiguration);

            using(var outputStream = new StreamWriter(options.OutputPath, true))
            {
                var lines = FileHelper.ReadLines(options.InputPath);
                foreach (string line in lines)
                {
                    if (line.Contains(RijndaelMessageHandler.CYPHER_TEXT_IV_SEPERATOR))
                    {
                        // Split the line by spaces, tabs
                        var tokens = line.Split(new[] {" ", "\t", ">", "<" }, StringSplitOptions.RemoveEmptyEntries);
                        var decryptionTargets = tokens.Where(x => x.Contains(RijndaelMessageHandler.CYPHER_TEXT_IV_SEPERATOR));
                        var decryptionPairs = decryptionTargets.Select(x => new Tuple<string, string>(x, decryptor.Decrypt(x)));

                        // Replace any encrypted strings in the line and then output it to the file
                        var output = decryptionPairs.Aggregate(line, (current, pair) => current.Replace(pair.Item1, pair.Item2));
                        outputStream.WriteLine(output);
                    }
                    else
                    {
                        // Output the unmodified line
                        outputStream.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the output path.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The output path for the decryption if none was specified</returns>
        public string GenerateOutputPath(LogDecryptOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.OutputPath))
            {
                var output = Path.Combine(Path.GetDirectoryName(options.InputPath),
                    Path.GetFileNameWithoutExtension(options.InputPath) + "[decrypted]" +
                    Path.GetExtension(options.InputPath));

                if (FileHelper.Exists(output))
                {
                    output = Path.Combine(Path.GetDirectoryName(options.InputPath),
                    Path.GetFileNameWithoutExtension(options.InputPath) + "[decrypted]." + Guid.NewGuid() +
                    Path.GetExtension(options.InputPath));
                }

                return output;
            }

            return options.OutputPath;
        }

        /// <summary>
        /// Checks that the options are valid.
        /// </summary>
        /// <param name="options">The options to validate.</param>
        /// <returns>The result of the validation.</returns>
        public ValidationResult AreOptionsValid(LogDecryptOptions options)
        {
            var result = new ValidationResult();

            // Check input file exists
            if (!FileHelper.Exists(options.InputPath))
            {
                result.ErrorMessage = "Cannot locate the input file: " + options.InputPath;
                result.IsValid = false;
            }

            return result;
        }

        /// <summary>
        /// Generates the encryption configuration.
        /// </summary>
        /// <param name="options">The commandline options to use for the encryption.</param>
        /// <returns>The configuration required for the Simple AES library</returns>
        public SimpleAesEncryptionConfiguration GenerateEncryptionConfiguration(LogDecryptOptions options)
        {
            SimpleAesEncryptionConfiguration result;

            if (string.IsNullOrWhiteSpace(options.Key))
            {
                // No key is specified, read the whole config from file
                result = ConfigurationManager.GetSection(DEFAULT_CONFIG_SECTION_NAME) as SimpleAesEncryptionConfiguration;
            }
            else
            {
                // Use the command line options
                result = new SimpleAesEncryptionConfiguration()
                {
                    CipherMode = options.Mode,
                    Padding = options.Padding,
                    EncryptionKey = new EncryptionKeyConfigurationElement()
                    {
                        KeySize = options.KeySize,
                        Key = options.Key
                    }
                };
            }

            return result;
        }
    }
}