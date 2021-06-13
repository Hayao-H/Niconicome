using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Style;
using Niconicome.Models.Domain.Local.Style.Type;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result.Generic;
using NiconicomeTest.Stabs.Models.Domain.Local.IO;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;

namespace NiconicomeTest.Local.Style
{
    class UserChromeHandlerUnitTest
    {
        private IUserChromeHandler? chromeHandler;

        [SetUp]
        public void SetUp()
        {
            this.chromeHandler = new UserChromeHandler(new NicoFileIOMock(() => true, () => Properties.Resources.userChrome), new LoggerStab());
        }

        [Test]
        public void スタイルを取得する()
        {
            IAttemptResult<UserChrome> result = this.chromeHandler!.GetUserChrome();

            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.MainPage.VideoList.ItemHeight, Is.EqualTo(200));
        }

        public void ひな形作成()
        {
            var chrome = new UserChrome();
            string content = JsonParser.Serialize(chrome);
            Console.WriteLine(content);
        }
    }
}
