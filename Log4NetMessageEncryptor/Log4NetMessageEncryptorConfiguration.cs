using System.Configuration;
using System.Security.Cryptography;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class Log4NetMessageEncryptorConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the cipher mode.
        /// </summary>
        /// <value>
        /// The cipher mode.
        /// </value>
        [ConfigurationProperty("cipherMode", IsRequired = false, DefaultValue = CipherMode.CBC)]
        public CipherMode CipherMode
        {
            get
            {
                return (CipherMode)this["cipherMode"];
            }
            set
            {
                this["cipherMode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the encryption key.
        /// </summary>
        /// <remarks>
        /// The length of the key needs to be the same as the value defined within the keySize configuration
        /// </remarks>
        /// <value>
        /// The encryption key.
        /// </value>
        [ConfigurationProperty("encryptionKey", IsRequired = true)]
        public string EncryptionKey
        {
            get
            {
                return (string)this["encryptionKey"];
            }
            set
            {
                this["encryptionKey"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the key in bits.
        /// </summary>
        /// <value>
        /// The size of the key in bits.
        /// </value>
        [ConfigurationProperty("keySize", IsRequired = true, DefaultValue = 256)]
        public int KeySize
        {
            get
            {
                return (int)this["keySize"];
            }
            set
            {
                this["keySize"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the padding mode.
        /// </summary>
        /// <value>
        /// The padding mode.
        /// </value>
        [ConfigurationProperty("padding", IsRequired = false, DefaultValue = PaddingMode.ISO10126)]
        public PaddingMode Padding
        {
            get
            {
                return (PaddingMode)this["padding"];
            }
            set
            {
                this["padding"] = value;
            }
        }
    }
}