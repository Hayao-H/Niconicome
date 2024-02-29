using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS.HLS;
using NUnit.Framework;

namespace NiconicomeTest.NetWork.Niconico.Watch.V2.DMS.HLS
{
    public class M3U8ParserUnitTest
    {
        private const string _master = """
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

        private const string _video = """
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

        private IM3U8Parser? _parser;

        [SetUp]
        public void Init()
        {
            this._parser = new M3U8Parser();
        }

        [Test]
        public void マスタープレイリストを解析する()
        {
            var result = this._parser!.Parse(_master).ToList();

            Assert.That(result.Count, Is.EqualTo(6));

        }

        [Test]
        public void ビデオプレイリストを解析する()
        {
            var result = this._parser!.Parse(_video).ToList();

            Assert.That(result.Count, Is.EqualTo(34));
            Assert.That(result[0].Type, Is.EqualTo(M3U8NodeType.Key));
            Assert.That(result[1].Type, Is.EqualTo(M3U8NodeType.Segment));
            Assert.That(result[1].Value, Is.EqualTo("6.000000,"));
            Assert.That(result[1].URL, Is.EqualTo("https://asset.domand.nicovideo.jp/6556dde7ea7f88d27388965a/video/12345/video-h264-360p-lowest/01.cmfv"));
        }
    }
}
