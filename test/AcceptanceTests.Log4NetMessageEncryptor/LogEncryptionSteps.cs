using System;
using TechTalk.SpecFlow;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using log4net.Core;
using ArtisanCode.Log4NetMessageEncryptor;

namespace AcceptanceTests.Log4NetMessageEncryptor
{
    [Binding]
    public class LogEncryptionSteps
    {
        [Given(@"Log4Net is configured")]
        public void GivenLogNetIsConfigured()
        {
            XmlConfigurator.Configure();
        }

        [When(@"I log a test message")]
        public void WhenILogATestMessage()
        {
            LogManager.GetLogger("LogEncryptionSteps").Info("Test Message");
        }

        [Then(@"there should be one log message present")]
        public void ThenThereShouldBeOneLogMessagePresent()
        {
            var events = GetLoggedEvents();

            Assert.AreEqual(1, events.Length);
        }

        [Then(@"the result should be encrypted")]
        public void ThenTheResultShouldBeEncrypted()
        {
            var events = GetLoggedEvents();

            Assert.IsFalse(events[0].RenderedMessage.Contains("Test Message"));
            Assert.IsTrue(events[0].RenderedMessage.Contains("??"));
        }

        public LoggingEvent[] GetLoggedEvents()
        {
            var encryptAppender = (((Hierarchy)LogManager.GetRepository()).Root.GetAppender("MessageEncryptingAppender")) as MessageEncryptingForwardingAppender;
            var memoryAppender = (encryptAppender.GetAppender("MemoryAppender")) as MemoryAppender;

            return memoryAppender.GetEvents();
        }
    }
}

