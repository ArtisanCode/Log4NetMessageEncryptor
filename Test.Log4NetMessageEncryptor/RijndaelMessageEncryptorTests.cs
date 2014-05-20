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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigureCryptoContainer_NullConfiguration_ArgumentNullExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = null;

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_NullEncryptionKey_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = null,
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_EmptyEncryptionKey_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = string.Empty,
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_WhitespaceEncryptionKey_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = "  \t",
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_IllegalKeySizeTooSmall_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = "testKey",
                KeySize = 127,
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_IllegalKeySizeTooLarge_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = "testKey",
                KeySize = 257,
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void ConfigureCryptoContainer_InvalidKeyLength_CryptographicExceptionThrown()
        {
            var testContainer = new RijndaelManaged();
            Log4NetMessageEncryptorConfiguration invalidTestConfig = new Log4NetMessageEncryptorConfiguration
            {
                EncryptionKey = Convert.ToBase64String(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }),
                KeySize = 256,
            };

            _target.ConfigureCryptoContainer(testContainer, invalidTestConfig);
        }
    }
}
