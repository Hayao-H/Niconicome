using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Remote;
using Niconicome.Models.Domain.Niconico.Remote.Series;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Niconico.Download.General;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Series
{
    class SeriesPageHtmlParserUnitTest
    {
        private ISeriesPageHtmlParser? seriesPageHtmlParser;

        [SetUp]
        public void SetUp()
        {
            this.seriesPageHtmlParser = new SeriesPageHtmlParser(new NiconicoUtils(new ReplaceHandlerStub()));
        }

        [Test]
        public void シリーズを解析する()
        {
            IAttemptResult<RemotePlaylistInfo> result = this.seriesPageHtmlParser!.GetSeriesInfo(Properties.Resources.Series_Touhou);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.PlaylistName, Is.EqualTo("つくった東方メドレー"));
            Assert.That(result.Data!.Videos.Count, Is.EqualTo(12));
        }
    }
}
