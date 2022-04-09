using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using IA = Niconicome.Models.Utils.InitializeAwaiter;

namespace NiconicomeTest.Utils.InitializeAwaiter
{
    [TestFixture]
    public class InitializeAwaiterUnitTest
    {
        [Test]
        public async Task 処理を待機する()
        {
            var awaiter = new IA::InitializeAwaiter();
            awaiter.RegisterStep(typeof(string));

            var isCompleted = false;

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                isCompleted = true;
                awaiter.NotifyCompletedStep(typeof(string));
            });

            await awaiter.Awaiter;

            Assert.That(isCompleted, Is.True);
            Assert.That(awaiter.IsCompleted, Is.True);
        }
    }
}
