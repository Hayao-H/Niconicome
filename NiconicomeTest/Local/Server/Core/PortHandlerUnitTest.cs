using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Local.Server.Connection;
using NiconicomeTest.Stabs.Models.Domain.Local.Settings;
using NiconicomeTest.Stabs.Models.Domain.Utils.Error;
using NUnit.Framework;

namespace NiconicomeTest.Local.Server.Core
{
    internal class PortHandlerUnitTest
    {
        private IPortHandler? _portHandler;


        [SetUp]
        public void Init()
        {
            var settingsContainer = new SettingsContainerStub();
            settingsContainer.Settings.Add(SettingNames.LocalServerPort, 2580);

            var connections = new TCPConnectionHandlerStub();
            connections.OccupiedPorts.AddRange(new[] { 1024, 2580 });

            this._portHandler = new PortHandler(new ErrorHandlerStub(), settingsContainer, connections);
        }

        [Test]
        public void 設定値を取得する()
        {
            Assert.That(this._portHandler!.GetSettingValue(), Is.EqualTo(2580));
        }

        [Test]
        public  void 空きポートを取得する()
        {
            IAttemptResult<IEnumerable<int>> result = this._portHandler!.GetAvailablePorts();

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            var data = result.Data!.ToList();

            Assert.That(data.Count, Is.EqualTo(64513 - 2));
            Assert.That(data.Contains(1024), Is.Not.True);
            Assert.That(data.Contains(2580), Is.Not.True);
            Assert.That(data.Contains(1025), Is.True);
            Assert.That(data.Contains(64512), Is.True);
        }

        [Test]
        public void ポートの空き状態を確認する()
        {
            Assert.That(this._portHandler!.IsPortAvailable(1024), Is.Not.True);
            Assert.That(this._portHandler.IsPortAvailable(2580), Is.Not.True);
            Assert.That(this._portHandler.IsPortAvailable(1025), Is.True);
            Assert.That(this._portHandler.IsPortAvailable(64512), Is.True);
        }
    }
}
