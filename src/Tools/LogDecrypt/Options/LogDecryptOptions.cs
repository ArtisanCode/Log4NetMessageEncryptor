using CommandLine;
using System.Security.Cryptography;
using CommandLine.Text;

namespace ArtisanCode.LogManager.Options
{
    /// <summary>
    /// Defines all the common encryption options between the encrypt and decrypt operations
    /// </summary>
    public class LogDecryptOptions
    {
        /// <summary>
        /// Gets or sets the input path.
        /// </summary>
        /// <value>
        /// The input path.
        /// </value>
        [Option('i', "input", Required = true, HelpText = "The path to the input file.")]
        public string InputPath { get; set; }


        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        /// <value>
        /// The output path.
        /// </value>
        [Option('o', "output", Required = false, HelpText = "The path to the output file. If not specified, then [decrypted] will be appended to the file name and saved in the same directory as the input file.")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Option('k', "key", Required = false, HelpText = "(optional) The encryption key to use during the encryption/decryption operation. If this isn't specified, then the tool will attempt to use a local config file.")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the size of the key.
        /// </summary>
        /// <value>
        /// The size of the key.
        /// </value>
        [Option('s', "keysize", Required = false, DefaultValue = 256, HelpText = "[ADVANCED] (optional) The size (in bits) of the encryption key to use during the encryption/decryption operation. Defaults to 256 bits.")]
        public int KeySize { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        [Option('c', "cyphermode", Required = false, DefaultValue = CipherMode.CBC, HelpText = "[ADVANCED] (optional) The cipher mode to use during the encryption/decryption operation. Defaults to CBC mode.")]
        public CipherMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        [Option('p', "padding", Required = false, DefaultValue = PaddingMode.ISO10126, HelpText = "[ADVANCED] (optional) The padding mode to use during the encryption/decryption operation. Defaults to ISO10126 mode.")]
        public PaddingMode Padding { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns></returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
