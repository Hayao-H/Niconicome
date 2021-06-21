using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Series;
using Niconicome.Models.Helper.Result.Generic;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Series
{
    class SeriesPageHtmlParserUnitTest
    {
        private ISeriesPageHtmlParser? seriesPageHtmlParser;

        [SetUp]
        public void SetUp()
        {
            this.seriesPageHtmlParser = new SeriesPageHtmlParser();
        }

        [Test]
        public void シリーズを解析する()
        {
            IAttemptResult<SeriesInfo> result = this.seriesPageHtmlParser!.GetSeriesInfo(Properties.Resources.Series_Touhou);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.SeriesName, Is.EqualTo("つくった東方メドレー"));
            Assert.That(result.Data!.SeriesVideos.Count, Is.EqualTo(12));
        }
    }
}
