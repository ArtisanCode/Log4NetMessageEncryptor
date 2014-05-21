using System.Security.Cryptography;

using ArtisanCode.Log4NetMessageEncryptor;
using ArtisanCode.Log4NetMessageEncryptor.Encryption;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtisanCode.Test.Log4NetMessageEncryptor.Encryption
{
    [TestClass]
    public class RijndaelMessageDecryptorTests
    {
        public RijndaelMessageDecryptor _target;

        public Log4NetMessageEncryptorConfiguration testConfig;

        [TestInitialize]
        public void __init()
        {
            testConfig = new Log4NetMessageEncryptorConfiguration();
            testConfig.EncryptionKey = new EncryptionKeyConfigurationElement(256, "3q2+796tvu/erb7v3q2+796tvu/erb7v3q2+796tvu8=");

            _target = new RijndaelMessageDecryptor(testConfig);
        }


        [TestMethod]
        public void Decrypt_EmptyPlaintext_EmptyStringReturned()
        {
            var result = _target.Decrypt("");

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Decrypt_EncodedMessageDecrypted_InputMessageEqualsDecryptedOutput()
        {           
            var encryptor = new RijndaelMessageEncryptor(testConfig);
            var secretMessage = "My ultra secret message";
            var input = encryptor.Encrypt(secretMessage);

            var result = _target.Decrypt(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(secretMessage, result);
        }

        [TestMethod]
        public void Decrypt_NullPlaintext_EmptyStringReturned()
        {
            var result = _target.Decrypt(null);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void Decrypt_InvalidLengthTextSentForDecryption_ExceptionThrown()
        {
            var result = _target.Decrypt("dGVzdCBkYXRh>>wLQO0465tJ5lxuodSSlmgg==");
        }
    }
}
