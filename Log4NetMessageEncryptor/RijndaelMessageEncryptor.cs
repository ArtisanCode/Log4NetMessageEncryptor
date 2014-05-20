using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace ArtisanCode.Log4NetMessageEncryptor
{
    public class RijndaelMessageEncryptor : IMessageEncryptor
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public Log4NetMessageEncryptorConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RijndaelMessageEncryptor"/> class.
        /// </summary>
        /// <remarks>
        /// Reads the configuration directly from the configuration file section: Log4NetMessageEncryptorConfigurationSection
        /// </remarks>
        public RijndaelMessageEncryptor()
        {
            Configuration = ConfigurationManager.GetSection("Log4NetMessageEncryptorConfigurationSection") as Log4NetMessageEncryptorConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RijndaelMessageEncryptor"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public RijndaelMessageEncryptor(Log4NetMessageEncryptorConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Configures the crypto container.
        /// </summary>
        /// <param name="cryptoContainer">The crypto container to configure.</param>
        /// <param name="config">The configuration to use during encryption.</param>
        public void ConfigureCryptoContainer(RijndaelManaged cryptoContainer, Log4NetMessageEncryptorConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
        }

        /// <summary>
        /// Encrypts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public string Encrypt(string source)
        {
            // Short-circuit encryption for empty strings
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            // Create a new instance of the RijndaelManaged 
            // class.  This generates a new key and initialization  
            // vector (IV). 
            using (RijndaelManaged cryptoContainer = new RijndaelManaged())
            {
                cryptoContainer.Padding = PaddingMode.ISO10126;

                cryptoContainer.GenerateKey();
                cryptoContainer.GenerateIV();

                // Encrypt the string to an array of bytes. 
                byte[] encrypted = EncryptStringToBytes(source, cryptoContainer.Key, cryptoContainer.IV);

#if DEBUG
                // Decrypt the bytes to a string. 
                string roundtrip = DecryptStringFromBytes(encrypted, cryptoContainer.Key, cryptoContainer.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", source);
                Console.WriteLine("Round Trip: {0}", roundtrip);
#endif

                // Return the Base64 encoded cypher-text along with the (plaintext) unique IV used for this encryption
                return string.Format("{0}>>{1}", Convert.ToBase64String(encrypted), Convert.ToBase64String(cryptoContainer.IV));
            }
        }

        /// <summary>
        /// Encrypts the string to bytes.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The iv.</param>
        /// <remarks>
        /// Original version: http://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
        /// 20/05/2014 @ 20:05
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// plainText
        /// or
        /// Key
        /// or
        /// IV
        /// </exception>
        static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            byte[] encrypted;

            // Create an RijndaelManaged object with the specified key and IV. 
            using (RijndaelManaged cryptoContainer = new RijndaelManaged())
            {
                cryptoContainer.Padding = PaddingMode.ISO10126;

                cryptoContainer.Key = Key;
                cryptoContainer.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = cryptoContainer.CreateEncryptor(cryptoContainer.Key, cryptoContainer.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream. 
            return encrypted;
        }

        /// <summary>
        /// Decrypts the string from bytes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The iv.</param>
        /// <returns></returns>
        /// <remarks>
        /// Original version: http://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
        /// 20/05/2014 @ 20:05
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// cipherText
        /// or
        /// Key
        /// or
        /// IV
        /// </exception>
        static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged cryptoContainer = new RijndaelManaged())
            {
                cryptoContainer.Padding = PaddingMode.ISO10126;

                cryptoContainer.Key = Key;
                cryptoContainer.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = cryptoContainer.CreateDecryptor(cryptoContainer.Key, cryptoContainer.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}
