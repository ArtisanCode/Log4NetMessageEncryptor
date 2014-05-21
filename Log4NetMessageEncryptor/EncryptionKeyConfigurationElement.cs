using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class EncryptionKeyConfigurationElement: ConfigurationElement
    {
        public EncryptionKeyConfigurationElement(int keySize, string key) : base()
        {
            this.KeySize = keySize;
            this.Key = key;
        }

        public EncryptionKeyConfigurationElement():base()
        {

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
        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get
            {
                return this["Key"] as string;
            }
            set
            {
                this["Key"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the key in bits.
        /// </summary>
        /// <value>
        /// The size of the key in bits.
        /// </value>
        [ConfigurationProperty("KeySize", IsRequired = true, DefaultValue = 256)]
        public int KeySize
        {
            get
            {
                return (int)this["KeySize"];
            }
            set
            {
                this["KeySize"] = value;
            }
        }
    }
}
