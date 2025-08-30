using NUnit.Framework;
using Moq;
using SWEN2_TourPlannerGroupProject.Logging;
using System;

namespace SWEN2_TourPlannerGroupProject.Tests
{
    [TestFixture]
    internal class LoggerWrapperTests
    {
        private Mock<log4net.ILog> _mockLog;
        private Log4NetWrapper _logger;

        [SetUp]
        public void Setup()
        {
            _mockLog = new Mock<log4net.ILog>();
            _logger = new Log4NetWrapper(_mockLog.Object);
        }

        [Test]
        public void Info_CallsUnderlyingLogMethod()
        {
            // Act
            _logger.Info("Hello world");

            // Assert
            _mockLog.Verify(l => l.Info("Hello world"), Times.Once);
        }

        [Test]
        public void Info_WithException_CallsUnderlyingLogMethod()
        {
            var ex = new InvalidOperationException("test");

            _logger.Info("Something went wrong", ex);

            _mockLog.Verify(l => l.Info("Something went wrong", ex), Times.Once);
        }

        [Test]
        public void Error_CallsUnderlyingLogMethod()
        {
            _logger.Error("Error happened");

            _mockLog.Verify(l => l.Error("Error happened"), Times.Once);
        }

        [Test]
        public void Fatal_WithException_CallsUnderlyingLogMethod()
        {
            var ex = new Exception("fatal");
            _logger.Fatal("Fatal error", ex);

            _mockLog.Verify(l => l.Fatal("Fatal error", ex), Times.Once);
        }

        [Test]
        public void AllMethods_CanBeCalled()
        {
            var ex = new Exception("test");
            _logger.Debug("debug");
            _logger.Debug("debug", ex);
            _logger.Info("info");
            _logger.Info("info", ex);
            _logger.Warn("warn");
            _logger.Warn("warn", ex);
            _logger.Error("error");
            _logger.Error("error", ex);
            _logger.Fatal("fatal");
            _logger.Fatal("fatal", ex);

            _mockLog.Verify(l => l.Debug("debug"), Times.Once);
            _mockLog.Verify(l => l.Debug("debug", ex), Times.Once);
            _mockLog.Verify(l => l.Info("info"), Times.Once);
            _mockLog.Verify(l => l.Info("info", ex), Times.Once);
            _mockLog.Verify(l => l.Warn("warn"), Times.Once);
            _mockLog.Verify(l => l.Warn("warn", ex), Times.Once);
            _mockLog.Verify(l => l.Error("error"), Times.Once);
            _mockLog.Verify(l => l.Error("error", ex), Times.Once);
            _mockLog.Verify(l => l.Fatal("fatal"), Times.Once);
            _mockLog.Verify(l => l.Fatal("fatal", ex), Times.Once);
        }
    }
}
