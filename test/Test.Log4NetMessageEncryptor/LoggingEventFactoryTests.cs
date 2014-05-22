using ArtisanCode.Log4NetMessageEncryptor;
using FizzWare.NBuilder;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArtisanCode.Test.Log4NetMessageEncryptor
{
    [TestClass]
    public class LoggingEventFactoryTests
    {
        LoggingEventFactory _target;

        [TestInitialize]
        public void __init()
        {
            _target = new LoggingEventFactory();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateEncryptedLoggingEvent_SourceValueNull_ExceptionThrown()
        {
            _target.CreateEncryptedLoggingEvent(null, "QQQQQ");
        }

        [TestMethod]
        public void CreateEncryptedLoggingEvent_CreateNewEvent_EventCreatedSucessfully()
        {
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            testEventData.ExceptionString = "This is an excceptionString!";
            var testSource = new LoggingEvent(testEventData);
            string encryptedLoggingMessage = "QQQQQ";

            var result = _target.CreateEncryptedLoggingEvent(testSource, encryptedLoggingMessage);

            Assert.AreEqual(encryptedLoggingMessage, result.RenderedMessage);
            Assert.AreEqual(testEventData.ExceptionString, result.GetExceptionString());
            Assert.AreEqual(testEventData.Level, result.Level);
            Assert.AreEqual(testEventData.LoggerName, result.LoggerName);
            Assert.AreEqual(testEventData.TimeStamp, result.TimeStamp);
        }

        [TestMethod]
        public void CreateErrorEvent_ValidErrorMessage_MessageCreated()
        {
            var msg = "Test error message";

            var result = _target.CreateErrorEvent(msg);

            Assert.IsNotNull(result);
            Assert.AreEqual(msg, result.GetExceptionString());
        }

        [TestMethod]
        public void CreateErrorEvent_EmptyErrorMessage_MessageCreated()
        {
            var msg = string.Empty;

            var result = _target.CreateErrorEvent(msg);

            Assert.IsNotNull(result);
            Assert.AreEqual(msg, result.GetExceptionString());
        }
    }
}
