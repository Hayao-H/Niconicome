using NUnit.Framework;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace NiconicomeTest.Local
{
    [TestFixture]
    class SettingUnitTest
    {
        private ISettingHandler? handler;

        private IDataBase? dataBase;

        [SetUp]
        public void SetUp()
        {
            this.handler = new SettingHandler(Static.DataBaseInstance);
            this.dataBase = Static.DataBaseInstance;
            this.dataBase.Clear(STypes::AppSettingString.TableName);
            this.dataBase.Clear(STypes::AppSettingBool.TableName);
        }

        [Test]
        public void 真偽値設定を保存する()
        {
            this.handler!.SaveBoolSetting("test", true);

            Assert.IsTrue(this.handler!.Exists("test", SettingType.boolSetting));
            Assert.IsTrue(this.handler!.GetBoolSetting("test").Data);
        }

        [Test]
        public void 文字列設定を保存する()
        {
            this.handler!.SaveStringSetting("test","hoge");

            Assert.IsTrue(this.handler!.Exists("test", SettingType.stringSetting));
            Assert.AreEqual("hoge",this.handler!.GetStringSetting("test").Data);
        }
    }
}
