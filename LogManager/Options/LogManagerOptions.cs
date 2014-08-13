using CommandLine;
using CommandLine.Text;

namespace ArtisanCode.LogManager.Options
{
    /// <summary>
    /// Defines the root options object
    /// </summary>
    public class LogManagerOptions
    {
        /// <summary>
        /// Gets or sets the encrypt verb.
        /// </summary>
        /// <value>
        /// The encrypt verb.
        /// </value>
        [VerbOption("encrypt", HelpText = "Encrypts the input file.")]
        public EncryptOptions EncryptVerb { get; set; }

        /// <summary>
        /// Gets or sets the decrypt verb.
        /// </summary>
        /// <value>
        /// The decrypt verb.
        /// </value>
        [VerbOption("decrypt", HelpText = "Decrypts the input file.")]
        public DecryptOptions DecryptVerb { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <returns></returns>
        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}