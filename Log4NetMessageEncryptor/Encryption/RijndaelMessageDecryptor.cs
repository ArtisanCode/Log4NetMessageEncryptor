using System;
using System.IO;
using System.Security.Cryptography;

namespace ArtisanCode.Log4NetMessageEncryptor.Encryption
{
    public class RijndaelMessageDecryptor : RijndaelMessageHandler, IMessageDecryptor
    {
        public RijndaelMessageDecryptor(): base()
        {

        }

        public RijndaelMessageDecryptor(Log4NetMessageEncryptorConfiguration config): base(config)
        {

        }

        public virtual string Decrypt(string source)
        {
            // Short-circuit decryption for empty strings
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var primatives = source.Split(new[] { CYPHER_TEXT_IV_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);

            if (primatives.Length != 2)
            {
                throw new ArgumentException("Invalid source string. Unable to determine the correct IV used for the encryption. Please ensure the source string is in the format 'Cypher Text'" + CYPHER_TEXT_IV_SEPERATOR + "'IV'", "source");
            }

            var cypherText = Convert.FromBase64String(primatives[0]);
            var iv = Convert.FromBase64String(primatives[1]);

            return DecryptStringFromBytes(cypherText, iv);
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
        public virtual string DecryptStringFromBytes(byte[] cipherText, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }

            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged cryptoContainer = new RijndaelManaged())
            {
                ConfigureCryptoContainer(cryptoContainer, Configuration);

                // Remember to set the IV to the correct value for decryption
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