using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Video.Ichiba;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Videos.Icihba
{
    class IchibaHtmlParserUnitTest
    {
        private IIchibaHtmlParser? htmlParser;

        [SetUp]
        public void SetUp()
        {
            this.htmlParser = new IchibaHtmlParser();
        }

        [Test]
        public void 市場のHTMlを解析する()
        {
            var result = this.htmlParser!.ParseHtml(Properties.Resources.IchibaHtml);
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.IchibaItems.Count, Is.EqualTo(16));

            var ichizoku = result.Data!.IchibaItems.FirstOrDefault(i => i.Name == "豪血寺一族");
            Assert.That(ichizoku, Is.Not.Null);
            Assert.That(ichizoku!.Name, Is.EqualTo("豪血寺一族"));
            Assert.That(ichizoku!.Category, Is.EqualTo("ゲーム"));
            Assert.That(ichizoku!.Price, Is.EqualTo("￥9,800"));
            Assert.That(ichizoku!.ThumbUrl, Is.EqualTo(@"https://m.media-amazon.com/images/I/51tk2w-C8SL.jpg"));
            Assert.That(ichizoku!.LinkUrl, Is.EqualTo(@"https://ichiba.nicovideo.jp/embed/redirect?item_id=azB000068HQ0"));
        }
    }
}
