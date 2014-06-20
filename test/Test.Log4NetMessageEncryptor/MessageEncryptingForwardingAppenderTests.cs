using ArtisanCode.Log4NetMessageEncryptor;
using ArtisanCode.SimpleAesEncryption;

using FizzWare.NBuilder;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace ArtisanCode.Test.Log4NetMessageEncryptor
{
    [TestClass]
    public class MessageEncryptingForwardingAppenderTests
    {
        private MessageEncryptingForwardingAppender _target;
        private Mock<IMessageEncryptor> encryptorMock;
        private Mock<ILoggingEventFactory> logEventFactoryMock;
        private MockRepository mocks;

        [TestInitialize]
        public void __init()
        {
            mocks = new MockRepository(MockBehavior.Strict);

            encryptorMock = mocks.Create<IMessageEncryptor>();
            logEventFactoryMock = mocks.Create<ILoggingEventFactory>();

            _target = new MessageEncryptingForwardingAppender(encryptorMock.Object, logEventFactoryMock.Object);
        }

        [TestMethod]
        public void ActionAppendSingle_TestOrchestrationGreenPath_BaseCalled()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvent = new LoggingEvent(testEventData);
            string testEncryptedString = "QQQQQQQQQ";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Returns(testEncryptedString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(testEvent, testEncryptedString, null)).Returns(testEvent);

            // act
            _target.ActionAppend(testEvent);

            // assert
            mocks.VerifyAll();
        }

        [TestMethod]
        public void Constructor_NoParameters_NewDependenciesAreCreated()
        {
            var localTarget = new MessageEncryptingForwardingAppender();

            Assert.IsNotNull(localTarget.LogEventFactory);
            Assert.IsNotNull(localTarget.MessageEncryption);
        }

        [TestMethod]
        public void Constructor_DependenciesInjected_DependenciesAreStoredCorrectly()
        {
            var messageEncryption = new RijndaelMessageEncryptor();
            var logEventFactory = new LoggingEventFactory();

            var localTarget = new MessageEncryptingForwardingAppender(messageEncryption, logEventFactory);

            Assert.AreSame(messageEncryption, localTarget.MessageEncryption);
            Assert.AreSame(logEventFactory, localTarget.LogEventFactory);
        }

        [TestMethod]
        public void GenerateEncryptedLogEvent_TestOrchestrationExceptionThrown_ErrorMessageGeneratedAndReturned()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvent = new LoggingEvent(testEventData);
            var expectedEvent = new LoggingEvent(new LoggingEventData());
            string exceptionMessage = "Test Encryption Exception";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Throws(new ApplicationException(exceptionMessage));
            logEventFactoryMock.Setup(x => x.CreateErrorEvent(exceptionMessage)).Returns(expectedEvent);

            // act
            var result = _target.GenerateEncryptedLogEvent(testEvent);

            // assert
            mocks.VerifyAll();
            Assert.AreSame(expectedEvent, result);
        }

        [TestMethod]
        public void GenerateEncryptedLogEvent_TestOrchestrationGreenPath_MessageEncrypted()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvent = new LoggingEvent(testEventData);
            string testEncryptedString = "QQQQQQQQQ";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Returns(testEncryptedString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(testEvent, testEncryptedString, null)).Returns(testEvent);

            // act
            var result = _target.GenerateEncryptedLogEvent(testEvent);

            // assert
            mocks.VerifyAll();
            Assert.AreSame(testEvent, result);
        }

        [TestMethod]
        public void GenerateEncryptedLogEvent_TestOrchestrationWithExceptionGreenPath_MessageEncrypted()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            testEventData.ExceptionString = "This is an excceptionString!";
            var testEvent = new LoggingEvent(testEventData);
            string testEncryptedString = "QQQQQQQQQ";
            string testEncryptedExceptionString = "EEEEEEEEEE";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Returns(testEncryptedString);
            encryptorMock.Setup(x => x.Encrypt(testEventData.ExceptionString)).Returns(testEncryptedExceptionString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(testEvent, testEncryptedString, testEncryptedExceptionString)).Returns(testEvent);

            // act
            var result = _target.GenerateEncryptedLogEvent(testEvent);

            // assert
            mocks.VerifyAll();
            Assert.AreSame(testEvent, result);
        }

        [TestMethod]
        public void ActionAppend_ListOfEvents_OrchestrationSuccessfullAllEventsEncrypted()
        {
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvents = new List<LoggingEvent> 
            { 
                new LoggingEvent(testEventData), new LoggingEvent(testEventData), new LoggingEvent(testEventData), new LoggingEvent(testEventData), new LoggingEvent(testEventData) 
            };
            string testEncryptedString = "QQQQQQQQQ";

            encryptorMock.Setup(x => x.Encrypt(It.IsAny<string>())).Returns(testEncryptedString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(It.IsAny<LoggingEvent>(), testEncryptedString, null)).Returns(new LoggingEvent(testEventData));

            _target.ActionAppend(testEvents.ToArray());

            encryptorMock.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Exactly(5));
            logEventFactoryMock.Verify(x => x.CreateEncryptedLoggingEvent(It.IsAny<LoggingEvent>(), testEncryptedString, null), Times.Exactly(5));
        }
    }
}