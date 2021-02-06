using System;
using System.IO;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Network;
using NiconicomeTest.Stabs.Models.Domain.Network;
using Utils = Niconicome.Models.Domain.Utils;
using NiconicomeTest.Stabs.Models.Domain.Niconico;
using NiconicomeTest.Stabs.Models.Domain.Niconico.NicoHttpStabs;
using System.Drawing.Imaging;

namespace NiconicomeTest.NetWork
{
    [TestFixture]
    class CacheTest
    {
        private ICacheHandler? cache;

        [SetUp]
        public void SetUp()
        {
            this.cache = new CacheHandler(new CacheStreamStab());
            if (this.cache.HasCache("sm9", CacheType.Thumbnail))
            {
                File.Delete(this.cache.GetCachePath("sm9", CacheType.Thumbnail));
            }
        }

        [TestCase("sm9","9")]
        [TestCase("sm123","3")]
        [TestCase("so32","12")]
        [TestCase("124","4")]
        [TestCase("sm34","14")]
        [TestCase("18", "18")]
        public void ディレクトリー名を確認する(string id,string result)
        {
            if (this.cache is null) throw new InvalidOperationException();

            Assert.AreEqual(result,this.cache.GetDirectoryName(id));
        }

        [Test]
        public void キャッシュファイルのパスを取得する()
        {
            if (this.cache is null) throw new InvalidOperationException();

            string path = this.cache.GetCachePath("sm9", CacheType.Thumbnail);

            Assert.AreEqual(@"cache\thumb\9\sm9.jpg",path);
        }

        [Test]
        public void キャッシュファイルの存在を確認する()
        {
            if (this.cache is null) throw new InvalidOperationException();

            bool path = this.cache.HasCache("sm9", CacheType.Thumbnail);

            Assert.IsFalse(path);
        }
    }



    class CacheStraemUnitTest
    {
        private ICacheStraem? stream;

        private ICacheHandler? cache;

        [SetUp]
        public void SetUp()
        {
            var ms = new MemoryStream();
            var bmp = Properties.Resources.Onmyoji_Thumb;
            bmp.Save(ms,ImageFormat.Jpeg);
            var arr = ms.GetBuffer();
            var http = new NicoHttpStab(new ByteArrayContent(arr), new ByteArrayContent(arr));
            this.cache = new CacheHandler(new CacheStreamStab());
            this.stream = new CacheStream(http);
        }

        [Test]
        public async Task キャッシュを保存する()
        {

            await this.stream!.GetAndWrite("https://nicovideo.cdn.nimg.jp/thumbnails/9/9", this.cache!.GetCachePath("sm9", CacheType.Thumbnail));

            Assert.IsTrue(cache.HasCache("sm9", CacheType.Thumbnail));
        }

        
    }
}
