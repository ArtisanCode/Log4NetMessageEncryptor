using CommandLine;
using CommandLine.Text;

namespace ArtisanCode.LogManager.Options
{
    /// <summary>
    /// Defines any encryption specific options.
    /// </summary>
    public class EncryptOptions : CommmonLogManagerOptions
    {
        // Remainder omitted
        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns></returns>
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}
