using System;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;
using Utils = Niconicome.Models.Domain.Utils;
using Local = Niconicome.Models.Local;
using State = Niconicome.Models.Local.State;

namespace NiconicomeTest.Local.LoggerTest
{
    [TestFixture]
    class LoggerTest
    {

        private Utils::ILogger? logger;

        private LogstreamStab? logstream;

        [SetUp]
        public void SetUp()
        {
            this.logstream = new LogstreamStab();
            this.logger = new Utils::Logger(this.logstream, new Utils::ErrorHandler(), new State::LocalState() { IsDebugMode = true });
        }

        [Test]
        public void LogTest()
        {
            this.logger?.Log("テスト");
            Assert.That(this.logstream?.logContent,Does.EndWith("[Log]テスト"));
        }

        [Test]
        public void ErrorTest()
        {
            var e = new Exception("テストエラー");
            this.logger?.Error("テスト", e);
            Assert.IsTrue(this.logstream?.logContent?.Contains("Exception Message : テストエラー"));
            Assert.IsTrue(this.logstream?.logContent?.Contains("Message : テスト"));
        }
    }
}