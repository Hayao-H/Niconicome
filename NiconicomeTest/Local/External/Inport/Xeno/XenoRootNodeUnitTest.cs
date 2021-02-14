using NUnit.Framework;
using Niconicome.Models.Domain.Local.External.Inport.Xeno;

namespace NiconicomeTest.Local.External.Inport.Xeno
{
    [TestFixture]
    class XenoRootNodeUnitTest
    {
        [TestCase(@"2	0		最近再生した動画	<最近再生した動画の一覧>",2,"最近再生した動画", "<最近再生した動画の一覧>",null)]
        [TestCase(@"2	0	s=7	かくしごと	https://ch.nicovideo.jp/kakushigoto-anime	list\アニメ\かくしごと", 2, "かくしごと", "https://ch.nicovideo.jp/kakushigoto-anime", @"list\アニメ\かくしごと")]
        [TestCase(@"2	0		JAZZ	G:\[app]niconico\NicomentXenoglossia\down\JAZZ.txt	G:\[app]niconico\NicomentXenoglossia\down", 2, "JAZZ", @"G:\[app]niconico\NicomentXenoglossia\down\JAZZ.txt", @"G:\[app]niconico\NicomentXenoglossia\down")]
        public void ノード解析のテスト(string origin,int layer,string title,string listPath,string folderPath)
        {
            var node = new XenoRootNode(origin);
            Assert.That(node.Layer, Is.EqualTo(layer));
            Assert.That(node.Title, Is.EqualTo(title));
            Assert.That(node.ListPath, Is.EqualTo(listPath));
            Assert.That(node.FolderPath, Is.EqualTo(folderPath));

        }
    }
}
