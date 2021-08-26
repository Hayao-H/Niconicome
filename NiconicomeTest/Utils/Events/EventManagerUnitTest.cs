using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Event;
using NUnit.Framework;

namespace NiconicomeTest.Utils.Events
{
    class EventManagerUnitTest
    {
        private IEventManager? manager;

        [SetUp]
        public void SetUp()
        {
            this.manager = new EventManager();
        }
        
        [Test]
        public async Task 単一イベントを発行するチェックする()
        {
            var result = 0;
            this.manager!.Regster(() => result = 1, DateTime.Now + new TimeSpan(0, 0, 2));

            await Task.Delay(4 * 1000);

            Assert.That(result,Is.EqualTo(1));
        }
    }
}
