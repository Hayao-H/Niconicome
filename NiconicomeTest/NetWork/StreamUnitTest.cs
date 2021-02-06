using NUnit.Framework;
using System.Collections.Generic;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using System.Linq;


namespace NiconicomeTest.NetWork
{
    [TestFixture]
    public class StreamHandlerTest
    {
        private StreamHandler? streamHandler;

        [SetUp]
        public void Setup()
        {
            this.streamHandler = new StreamHandler(new M3U8Handler());
        }

        [Test]
        public void マスタープレイリストをパースして最高画質を取得する()
        {
            var playlist = @"#EXTM3U
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
1/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x720,FRAME-RATE=29.970
2/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x1080,FRAME-RATE=29.970
3/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
4/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
";

            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsTrue(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual(1080, streamInfo?.GetTheBestStream().Resolution.Vertical);
        }

        [Test]
        public void マスタープレイリストをパースして最低画質を取得する()
        {
            var playlist = @"#EXTM3U
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
1/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x720,FRAME-RATE=29.970
2/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x1080,FRAME-RATE=29.970
3/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
4/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
";

            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsTrue(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual(360, streamInfo?.GetTheWorstStream().Resolution.Vertical);
        }

        [Test]
        public void マスタープレイリストをパースして720pを取得する()
        {
            var playlist = @"#EXTM3U
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
1/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x720,FRAME-RATE=29.970
2/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x1080,FRAME-RATE=29.970
3/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=609037,AVERAGE-BANDWIDTH=472417,RESOLUTION=480x360,FRAME-RATE=29.970
4/ts/playlist.m3u8?ht2_nicovideo=12345678.c93ahgmy7p_qki6qg_3t67bpyj2564w
";

            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsTrue(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual(720, streamInfo?.GetStream(720).Resolution.Vertical);
        }

        [Test]
        public void 動画ストリームをパースしてセグメントファイル数を取得()
        {
            var playlist = @"#EXTM3U
#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:6
#EXT-X-MEDIA-SEQUENCE:1
#EXT-X-PLAYLIST-TYPE:VOD

# EXTINF:6.0,
            1.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
2.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
3.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
4.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
5.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
6.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
7.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
8.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
9.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
10.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
11.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
12.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
13.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
14.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
15.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
16.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
17.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
18.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
19.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
20.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
21.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
22.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
23.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXT-X-ENDLIST

";

            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsFalse(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual(23, streamInfo?.StreamUrls.Count);
        }

        [Test]
        public void 動画ストリームをパースして最初のセグメントファイルを取得()
        {
            var playlist = @"#EXTM3U
#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:6
#EXT-X-MEDIA-SEQUENCE:1
#EXT-X-PLAYLIST-TYPE:VOD

# EXTINF:6.0,
            1.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
2.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
3.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
4.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
5.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
6.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
7.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
8.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
9.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
10.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
11.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
12.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
13.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
14.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
15.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
16.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
17.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
18.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
19.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
20.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
21.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
22.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
23.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXT-X-ENDLIST

";

            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsFalse(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual("1.ts", streamInfo?.StreamUrls[0].FileName);
        }

        [Test]
        public void 動画ストリームをパースして最後のセグメントファイルを取得()
        {
            var playlist = @"#EXTM3U
#EXTM3U
#EXT-X-VERSION:3
#EXT-X-TARGETDURATION:6
#EXT-X-MEDIA-SEQUENCE:1
#EXT-X-PLAYLIST-TYPE:VOD

# EXTINF:6.0,
            1.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
2.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
3.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
4.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
5.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
6.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
7.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
8.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
9.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
10.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
11.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
12.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
13.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
14.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
15.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
16.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
17.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
18.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
19.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
20.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
21.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
22.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXTINF:6.0,
23.ts ? ht2_nicovideo = 6 - hqUl3ui9H8_1609675163354.08uoxgh9m0_qmcxb3_1qrxuz2q84xww
#EXT-X-ENDLIST

";
            IStreamInfo? streamInfo = this.streamHandler?.GetStreamInfo(playlist, string.Empty);

            Assert.IsNotNull(streamInfo);
            Assert.IsFalse(streamInfo?.IsMasterPlaylist);
            Assert.AreEqual("23.ts", streamInfo?.StreamUrls.Last().FileName);
        }
    }

    [TestFixture]
    public class StreamsCollectionTest
    {
        private IStreamsCollection? streams;

        [SetUp]
        public void SetUp()
        {
            this.streams = new StreamsCollection();

            ///解像度一覧
            ///1080p：1920×1080（フルHD）
            ///720p：1280×720（HD）
            ///480p：854×480（SD）
            ///360p：640×360
            ///240p：426×240
            IEnumerable<IStreamInfo> streamInfos = new List<string>() { "1920x1080", "1280x720", "854x480", "640x360", "426x240" }.Select(r => new StreamInfo() { Resolution = new Resolution(r) });
            this.streams.AddRange(streamInfos);
        }

        [Test]
        public void 最高解像度のストリームを取得する()
        {
            var stream = this.streams!.GetTheBestStream();
            Assert.IsNotNull(stream.Resolution);
            Assert.AreEqual(1080, stream.Resolution!.Vertical);
        }

        [Test]
        public void 最低解像度のストリームを取得する()
        {
            var stream = this.streams!.GetTheWorstStream();
            Assert.IsNotNull(stream.Resolution);
            Assert.AreEqual(240, stream.Resolution!.Vertical);
        }

        [TestCase(2000, 1080)]
        [TestCase(1080, 1080)]
        [TestCase(720, 720)]
        [TestCase(480, 480)]
        [TestCase(360, 360)]
        [TestCase(240, 240)]
        [TestCase(100, 240)]
        public void 指定した解像度のストリームを取得する(int verticalResolution, int expected)
        {
            uint uVerticalResolution = (uint)verticalResolution;
            uint uExpected = (uint)expected;

            var stream = this.streams!.GetStream(uVerticalResolution);
            Assert.IsNotNull(stream.Resolution);
            Assert.AreEqual(uExpected, stream.Resolution!.Vertical);
        }
    }
}

