using ArtisanCode.Log4NetMessageEncryptor;
using FizzWare.NBuilder;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            // TODO: make the factory preserve the environment details
            //Assert.AreEqual(testEventData.Domain, result.Domain);
            //Assert.AreEqual(testEventData.Identity, result.Identity);
            //Assert.AreEqual(testEventData.LocationInfo, result.LocationInformation);
            //Assert.AreEqual(testEventData.ThreadName, result.ThreadName);
            //Assert.AreEqual(testEventData.UserName, result.UserName);
        }
    }
}
