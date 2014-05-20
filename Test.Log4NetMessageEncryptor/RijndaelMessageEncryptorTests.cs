using ArtisanCode.Log4NetMessageEncryptor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace ArtisanCode.Test.Log4NetMessageEncryptor
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

            _target = new RijndaelMessageEncryptor(testConfig);
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
        public void Encrypt_EmptyPlaintext_EmptyStringReturned()
        {
            var result = _target.Encrypt("");

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void Encrypt_WhitespacePlaintext_PlaintextEncrypted()
        {
            var result = _target.Encrypt("  \t");

            Assert.AreNotEqual(string.Empty, result);
        }

        [TestMethod]
        public void ConfigureCryptoContainer_ValidConfiguration_ContainerIsConfigured()
        {
            var testContainer = new RijndaelManaged();
            var testKey = new byte[32] { 
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF,
                0xDE, 0xAD, 0xBE, 0xEF, 0xDE, 0xAD, 0xBE, 0xEF
            };
            var validTestConfig = new Log4NetMessageEncryptorConfiguration()
            {
                CipherMode = CipherMode.CBC,
                EncryptionKey = Convert.ToBase64String(testKey),
                KeySize = 256,
                Padding = PaddingMode.ISO10126
            };

            _target.ConfigureCryptoContainer(testContainer, validTestConfig);

            Assert.IsTrue(testKey.SequenceEqual(testContainer.Key));
            Assert.AreEqual(validTestConfig.CipherMode, testContainer.Mode);
            Assert.AreEqual(validTestConfig.Padding, testContainer.Padding);
            Assert.AreEqual(validTestConfig.KeySize, testContainer.KeySize);
            Assert.IsTrue(testContainer.IV.Length == 16);
        }

    }
}
