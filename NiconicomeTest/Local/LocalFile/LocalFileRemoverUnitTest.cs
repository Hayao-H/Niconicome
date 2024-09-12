using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Local.IO.V2;
using NiconicomeTest.Stabs.Models.Domain.Niconico.Download.General;
using NiconicomeTest.Stabs.Models.Domain.Utils.Error;
using NUnit.Framework;

namespace NiconicomeTest.Local.LocalFile
{
    [TestFixture]
    public class LocalFileRemoverUnitTest
    {
        private ILocalFileRemover? _remover;

        private List<string>? _files;

        private NiconicomeFileIOStub? _fileIO;

        [SetUp]
        public void Setup()
        {
            this._fileIO = new NiconicomeFileIOStub();

            this._files = new List<string>()
            {
                @"C:\Niconicome\Test\Playlist\[sm9]テストテストテスト1.mp4",
                @"C:\Niconicome\Test\Playlist\[sm9]テストテストテスト1.xml",
                @"C:\Niconicome\Test\Playlist\[sm9]テストテストテスト1.jpg",
                @"C:\Niconicome\Test\Playlist\[sm9]テストテストテスト.html",
                @"C:\Niconicome\Test\Playlist\[so1234567]テストテストテスト2.mp4",
                @"C:\Niconicome\Test\Playlist\[so1234567]テストテストテスト2.xml",
                @"C:\Niconicome\Test\Playlist\[so1234567]テストテストテスト2.jpg",
                @"C:\Niconicome\Test\Playlist\[so1234567]テストテストテスト2.html",
                @"C:\Niconicome\Test\Playlist\[1234567]テストテストテスト3.mp4",
                @"C:\Niconicome\Test\Playlist\[1234567]テストテストテスト3.xml",
                @"C:\Niconicome\Test\Playlist\[1234567]テストテストテスト3.jpg",
                @"C:\Niconicome\Test\Playlist\[1234567]テストテストテスト3.html",
            };

            this._remover = new LocalFileRemover(new NiconicomeDirectoryIOStub(this._files, new List<string>()), this._fileIO, new NiconicoUtils(new ReplaceHandlerStub()), new ErrorHandlerStub());
        }

        [TestCase("sm9")]
        [TestCase("so1234567")]
        [TestCase("1234567")]
        public async Task 実体ファイルを削除する(string niconicoID)
        {
            List<string> deleted = new();

            this._fileIO!.DeleteMethodCall += (_, e) => deleted.Add(e.Path);

            IAttemptResult result = await this._remover!.RemoveFileAsync("", niconicoID);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(deleted.Count, Is.EqualTo(4));
            foreach (var file in deleted)
            {
                Assert.That(this._files!.Contains(file), Is.True);
            }

        }

        [TestCase("sm9")]
        [TestCase("so1234567")]
        [TestCase("1234567")]
        public async Task リストに存在しない実体ファイルを削除する(string niconicoID)
        {
            List<string> deleted = new();

            this._fileIO!.DeleteMethodCall += (_, e) => deleted.Add(e.Path);

            IAttemptResult result = await this._remover!.RemoveFilesAsync("", new List<string>() { niconicoID });

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(deleted.Count, Is.EqualTo(8));
            foreach (var file in deleted)
            {
                Assert.That(this._files!.Contains(file), Is.True);
            }

        }
    }
}
