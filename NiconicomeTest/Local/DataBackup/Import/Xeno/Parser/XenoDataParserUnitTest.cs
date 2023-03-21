using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Parser;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Type;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Properties;
using NiconicomeTest.Stabs.Models.Domain.Local.IO.V2;
using NiconicomeTest.Stabs.Models.Domain.Utils.Error;
using NUnit.Framework;

namespace NiconicomeTest.Local.DataBackup.Import.Xeno.Parser
{
    [TestFixture]
    public class XenoDataParserUnitTest
    {
        private IXenoDataParser? _parser;

        [SetUp]
        public void SetUp()
        {
            var fileIO = new NiconicomeFileIOStub(path => path switch
            {
                "path.txt" => Resources.Xeno_Video_File,
                _ => Resources.Xeno_Playlist_File,
            });
            this._parser = new XenoDataParser(fileIO, new ErrorHandlerStub());
        }

        [Test]
        public void データを解析する()
        {
            IAttemptResult<IEnumerable<IXenoPlaylist>> result = this._parser!.ParseData("");

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            List<IXenoPlaylist> data = result.Data!.ToList();

            Assert.That(data.Count, Is.EqualTo(13));

            IXenoPlaylist? bocchiTheRock = data.FirstOrDefault(p => p.Name == "ぼっち・ざ・ろっく！");
            IXenoPlaylist? chainsawMan = data.FirstOrDefault(p => p.Name == "チェンソーマン");

            Assert.That(bocchiTheRock, Is.Not.Null);
            Assert.That(bocchiTheRock!.IsChannel, Is.True);
            Assert.That(bocchiTheRock!.FolderPath, Is.EqualTo("list\\アニメ\\ぼっち・ざ・ろっく！"));

            Assert.That(chainsawMan, Is.Not.Null);
            Assert.That(chainsawMan!.IsChannel, Is.True);
            Assert.That(chainsawMan!.FolderPath, Is.EqualTo("list\\アニメ\\チェンソーマン"));

            IXenoPlaylist? root = data.FirstOrDefault(p => p.Name == "Root");
            IXenoPlaylist? a = data.FirstOrDefault(p => p.Name == "A");
            IXenoPlaylist? aa = data.FirstOrDefault(p => p.Name == "A-A");
            IXenoPlaylist? ab = data.FirstOrDefault(p => p.Name == "A-B");
            IXenoPlaylist? aba = data.FirstOrDefault(p => p.Name == "A-B-A");
            IXenoPlaylist? b = data.FirstOrDefault(p => p.Name == "B");

            Assert.That(root, Is.Not.Null);
            Assert.That(a, Is.Not.Null);
            Assert.That(aa, Is.Not.Null);
            Assert.That(ab, Is.Not.Null);
            Assert.That(aba, Is.Not.Null);
            Assert.That(b, Is.Not.Null);

            Assert.That(root!.Children.Count, Is.EqualTo(2));
            Assert.That(root.Children.Contains(a!.ID), Is.True);
            Assert.That(root.Children.Contains(b!.ID), Is.True);

            Assert.That(a.Children.Count, Is.EqualTo(2));
            Assert.That(a.Children.Contains(aa!.ID), Is.True);
            Assert.That(a.Children.Contains(ab!.ID), Is.True);
            Assert.That(ab.Children.Count, Is.EqualTo(1));
            Assert.That(ab.Children.Contains(aba!.ID), Is.True);

            Assert.That(aa.Videos.Count, Is.EqualTo(2));

            IXenoVideo? onmyoji = aa.Videos.FirstOrDefault(v => v.NiconicoID == "sm9");
            IXenoVideo? kyukurarin = aa.Videos.FirstOrDefault(v => v.NiconicoID == "sm39257413");

            Assert.That(onmyoji,Is.Not.Null);
            Assert.That(kyukurarin,Is.Not.Null);
            Assert.That(onmyoji!.Title, Is.EqualTo("新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師"));
            Assert.That(kyukurarin!.Title, Is.EqualTo("きゅうくらりん ／ いよわ feat.可不"));
        }
    }
}
