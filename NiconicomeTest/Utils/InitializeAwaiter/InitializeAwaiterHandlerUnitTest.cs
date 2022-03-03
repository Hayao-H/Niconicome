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
    public class InitializeAwaiterHandlerUnitTest
    {
        [Test]
        public async Task 名前付きタスクを待機する()
        {

            var name = "テスト";
            var awaiter = new IA::InitializeAwaiterHandler();
            awaiter.RegisterStep(name,typeof(string));

            var isCompleted = false;

            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                isCompleted = true;
                awaiter.NotifyCompletedStep(name, typeof(string));
            });

            await awaiter.GetAwaiter(name);

            Assert.That(isCompleted, Is.True);
        }
    }
}
