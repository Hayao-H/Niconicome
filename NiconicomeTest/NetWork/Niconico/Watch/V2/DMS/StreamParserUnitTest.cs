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
    public class StreamParserUnitTest
    {
        private readonly string _master = """
#EXTM3U
#EXT-X-VERSION:6
#EXT-X-INDEPENDENT-SEGMENTS
#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID="audio-aac-64kbps",NAME="Main Audio",DEFAULT=YES,URI="https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/audio-aac-64kbps.m3u8"
#EXT-X-MEDIA:TYPE=AUDIO,GROUP-ID="audio-aac-192kbps",NAME="Main Audio",DEFAULT=YES,URI="https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/audio-aac-192kbps.m3u8"
#EXT-X-STREAM-INF:BANDWIDTH=543471,AVERAGE-BANDWIDTH=391069,CODECS="avc1.42c01e,mp4a.40.2",RESOLUTION=640x360,FRAME-RATE=25.000,AUDIO="audio-aac-64kbps"
https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/video-h264-360p-lowest.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=1069207,AVERAGE-BANDWIDTH=796984,CODECS="avc1.4d401e,mp4a.40.2",RESOLUTION=640x360,FRAME-RATE=25.000,AUDIO="audio-aac-192kbps"
https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/video-h264-360p.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=1755020,AVERAGE-BANDWIDTH=1153138,CODECS="avc1.4d4020,mp4a.40.2",RESOLUTION=854x480,FRAME-RATE=25.000,AUDIO="audio-aac-192kbps"
https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/video-h264-480p.m3u8
#EXT-X-STREAM-INF:BANDWIDTH=2735712,AVERAGE-BANDWIDTH=1653102,CODECS="avc1.4d4020,mp4a.40.2",RESOLUTION=1280x720,FRAME-RATE=25.000,AUDIO="audio-aac-192kbps"
https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/video-h264-720p.m3u8

""";

        private IStreamParser? _parser;

        [SetUp]
        public void Init()
        {
            var http = new NicoHttpStab(new StringContent(this._master),new StringContent(string.Empty));
            this._parser = new StreamParser(http, new ErrorHandlerStub(), new M3U8Parser());
        }

        [Test]
        public async Task マスタープレイリストを取得する()
        {
            IAttemptResult<IStreamCollection> result = await this._parser!.ParseAsync("https://www.nicovideo.jp");
            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            IStreamCollection streamCollection = result.Data!;
            Assert.That(streamCollection.Count, Is.EqualTo(4));

            Assert.That(streamCollection.GetStream(360).VerticalResolution, Is.EqualTo(360));
            Assert.That(streamCollection.GetStream(480).VerticalResolution, Is.EqualTo(480));
            Assert.That(streamCollection.GetStream(720).VerticalResolution, Is.EqualTo(720));
            Assert.That(streamCollection.GetStream(720).PlaylistURL, Is.EqualTo("https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/video-h264-720p.m3u8"));
            Assert.That(streamCollection.GetStream(720).AudioURL, Is.EqualTo("https://delivery.domand.nicovideo.jp/hls/6556dde7ea7f88d27388965a/playlists/media/audio-aac-192kbps.m3u8"));

        }
    }
}
