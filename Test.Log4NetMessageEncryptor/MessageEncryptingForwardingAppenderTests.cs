using ArtisanCode.Log4NetMessageEncryptor;
using FizzWare.NBuilder;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ArtisanCode.Test.Log4NetMessageEncryptor
{
    [TestClass]
    public class MessageEncryptingForwardingAppenderTests
    {
        MockRepository mocks;

        MessageEncryptingForwardingAppender _target;

        Mock<IMessageEncryptor> encryptorMock;

        Mock<ILoggingEventFactory> logEventFactoryMock;

        [TestInitialize]
        public void __init()
        {
            mocks = new MockRepository(MockBehavior.Strict);

            encryptorMock = mocks.Create<IMessageEncryptor>();
            logEventFactoryMock = mocks.Create<ILoggingEventFactory>();

            _target = new MessageEncryptingForwardingAppender(encryptorMock.Object, logEventFactoryMock.Object);
        }

        [TestMethod]
        public void GenerateEncryptedLogEvent_TestOrchestrationGreenPath_MessageEncrypted()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvent = new LoggingEvent(testEventData);
            string testEncryptedString = "QQQQQQQQQ";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Returns(testEncryptedString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(testEvent, testEncryptedString)).Returns(testEvent);

            // act
            var result = _target.GenerateEncryptedLogEvent(testEvent);

            // assert
            mocks.VerifyAll();
            Assert.AreSame(testEvent, result);
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
        public void ActionAppendSingle_TestOrchestrationGreenPath_BaseCalled()
        {
            // arrange
            var testEventData = Builder<LoggingEventData>.CreateNew().Build();
            var testEvent = new LoggingEvent(testEventData);
            string testEncryptedString = "QQQQQQQQQ";

            encryptorMock.Setup(x => x.Encrypt(testEvent.RenderedMessage)).Returns(testEncryptedString);
            logEventFactoryMock.Setup(x => x.CreateEncryptedLoggingEvent(testEvent, testEncryptedString)).Returns(testEvent);

            // act
            _target.ActionAppend(testEvent);

            // assert
            mocks.VerifyAll();
        }
    }
}
