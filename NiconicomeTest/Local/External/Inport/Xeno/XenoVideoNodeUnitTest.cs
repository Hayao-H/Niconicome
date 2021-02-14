using NUnit.Framework;
using Niconicome.Models.Domain.Local.External.Import.Xeno;

namespace NiconicomeTest.Local.External.Inport.Xeno
{
    [TestFixture]
    class XenoVideoNodeUnitTest
    {
        [TestCase(@"*			－	[nm3859127]【東方】明治十七年のネイティブ紅茶館【アレンジ】		G:\[app]niconico\NicomentXenoglossia\list\東方（アレンジ）\	6243	08-07-05	nm3859127	250748","nm3859127")]
        [TestCase(@"*			－	[sm18511453]【激戦アレンジ】人形裁判　-SECOND ATTACK-【東方妖々夢】		G:\[app]niconico\NicomentXenoglossia\list\東方（アレンジ）\	185	12-08-02	sm18511453	57484", "sm18511453")]
        [TestCase("hoge fuga",null)]
        public void 動画情報ファイルノードの解析をテストする(string content,string id)
        {
            var video = new XenoVideoNode(content);
            Assert.That(video.NiconicoId, Is.EqualTo(id));
        }
    }
}
