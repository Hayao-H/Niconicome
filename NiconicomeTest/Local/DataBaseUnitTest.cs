using NUnit.Framework;
using Niconicome.Models.Domain.Local;
using STypes = Niconicome.Models.Domain.Local.Store.Types;
using System.Collections.Generic;


namespace NiconicomeTest.Local.DataBaseTest
{
    [TestFixture]
    public class DatabaseTest
    {
        public IDataBase? dataBase;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.dataBase = Static.DataBaseInstance;
        }

        [Test]
        public void 設定名testSettingのURL設定が存在することを確認する()
        {
            bool result = this.dataBase?.Exists<STypes::UrlSetting>(STypes::UrlSetting.TableName, setting => setting.SettingName == "testSetting") ?? true;
            Assert.IsFalse(result);
        }

        [Test]
        public void 設定名TestSettingのURL設定を作成する()
        {
            int id = this.dataBase?.Store(new STypes::UrlSetting() { SettingName = "testSetting2", UrlString = "https://www.google.com" }, STypes::UrlSetting.TableName).Data ?? -1;
            bool result = this.dataBase?.Exists<STypes::UrlSetting>(STypes::UrlSetting.TableName, setting => setting.SettingName == "testSetting2") ?? false;

            Assert.AreNotEqual(-1, id);
            Assert.IsTrue(result);
            this.dataBase?.Delete(STypes::UrlSetting.TableName, id);
        }

        [Test]
        public void テストプレイリストを保存する()
        {

            var playlist = new STypes::Playlist()
            {
                PlaylistName = "テスト"
            };

            int id = this.dataBase?.Store(playlist, STypes::Playlist.TableName).Data ?? -1;
            Assert.AreNotEqual(-1, id);
            Assert.IsTrue(this.dataBase?.Exists<STypes::Playlist>(STypes::Playlist.TableName, id));

            this.dataBase?.Delete(STypes::Playlist.TableName, id);
        }
    }

}

