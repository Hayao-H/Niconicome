using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Event;
using Niconicome.Models.Network.Watch;
using NUnit.Framework;

namespace NiconicomeTest.Utils.Events
{
    [TestFixture]
    class EventUnitTest
    {
        [Test]
        public void イベントをテストする()
        {
            var result = 0;
            var ev = new Event(() => result = 1, _ => { });

            ev.Invoke();

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void イベントのキャンセルをテストする()
        {
            var result = 0;
            var ev = new Event(() => result = 1, _ => { });

            ev.IsCancelled = true;
            ev.Invoke();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void エラー処理をテストする()
        {
            var isError = false;
            var ev = new Event(() => throw new Exception(), _ => { }, onError: _ => isError = true);

            ev.Invoke();

            Assert.That(isError, Is.True);
        }
    }
}
