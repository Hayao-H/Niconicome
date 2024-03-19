using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS.HLS;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using NiconicomeTest.Stabs.Models.Domain.Utils.Error;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Watch.V2.DMS
{
    public class StreamInfoUnitTest
    {
        private readonly string _video =  """
#EXTM3U
#EXT-X-VERSION:6
#EXT-X-TARGETDURATION:6
#EXT-X-MEDIA-SEQUENCE:1
#EXT-X-PLAYLIST-TYPE:VOD
#EXT-X-MAP:URI="https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/init01.cmfv"
#EXT-X-KEY:METHOD=AES-128,URI="https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/keys/video-h264-360p-lowest.key",IV=0x701970738D8478326537C3AC7374F2E9
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/01.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/02.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/03.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/04.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/05.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/06.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/07.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/08.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/09.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/10.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/11.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/12.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/13.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/14.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/15.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/16.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/17.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/18.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/19.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/20.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/21.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/22.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/23.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/24.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/25.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/26.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/27.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/28.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/29.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/30.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/31.cmfv
#EXTINF:6.000000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/32.cmfv
#EXTINF:1.560000,
https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/33.cmfv
#EXT-X-ENDLIST

""";

        private IStreamInfo? _streamInfo;

        [SetUp]
        public void Init()
        {
            var http = new NicoHttpStab(new StringContent(this._video), new StringContent(string.Empty));
            this._streamInfo = new StreamInfo(http, new M3U8Parser(), new ErrorHandlerStub());
            this._streamInfo.Initialize("https://www.nicovideo.jp", "https://www.nicovideo.jp", 1080, false,50000);
        }

        [Test]
        public async Task プレイリストを解析する()
        {
            IAttemptResult result = await this._streamInfo!.GetStreamInfo();

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(this._streamInfo.VideoKeyURL, Is.EqualTo(@"https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/keys/video-h264-360p-lowest.key"));
            Assert.That(this._streamInfo.AudioKeyURL, Is.EqualTo("https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/keys/video-h264-360p-lowest.key"));
            Assert.That(this._streamInfo.VideoIV, Is.EqualTo("0x701970738D8478326537C3AC7374F2E9"));
            Assert.That(this._streamInfo.AudioIV, Is.EqualTo("0x701970738D8478326537C3AC7374F2E9"));
            Assert.That(this._streamInfo.AudioSegmentURLs.Count(), Is.EqualTo(33));
            Assert.That(this._streamInfo.VideoSegmentURLs.Count(), Is.EqualTo(33));

        }

    }
}
