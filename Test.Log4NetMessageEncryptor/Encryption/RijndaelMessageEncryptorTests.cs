using ArtisanCode.Log4NetMessageEncryptor;
using ArtisanCode.Log4NetMessageEncryptor.Encryption;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtisanCode.Test.Log4NetMessageEncryptor.Encryption
{
    [TestClass]
    public class RijndaelMessageEncryptorTests
    {
        public RijndaelMessageEncryptor _target;

        public Log4NetMessageEncryptorConfiguration testConfig;

        [TestInitialize]
        public void __init()
        {
            testConfig = new Log4NetMessageEncryptorConfiguration();
            testConfig.EncryptionKey = new EncryptionKeyConfigurationElement(256, "3q2+796tvu/erb7v3q2+796tvu/erb7v3q2+796tvu8=");

            _target = new RijndaelMessageEncryptor(testConfig);
        }


        [TestMethod]
        public void Encrypt_EmptyPlaintext_EmptyStringReturned()
        {
            var result = _target.Encrypt("");

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Encrypt_MessageEncryptedSucessfully_NoExceptionsRaised()
        {
            var result = _target.Encrypt("my very secret string");
        }

        [TestMethod]
        public void Encrypt_NullPlaintext_EmptyStringReturned()
        {
            var result = _target.Encrypt(null);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Encrypt_WhitespacePlaintext_PlaintextEncrypted()
        {
            var result = _target.Encrypt("  \t");

            Assert.AreNotEqual(string.Empty, result);
        }
    }
}