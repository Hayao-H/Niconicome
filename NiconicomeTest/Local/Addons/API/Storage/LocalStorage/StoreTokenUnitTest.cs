using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.API.Storage.LocalStorage
{
    class StoreTokenUnitTest
    {
        private IStorageToken? _token;

        [OneTimeSetUp]
        public void SetUp()
        {
            var data = new Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage.StorageData();
            data.Data.Add("テスト1", "テスト");
            data.Data.Add("テスト2", "テスト");
            data.Data.Add("テスト3", "テスト");

            this._token = new StorageToken(data);
        }

        [Test]
        public void 書き込み可能サイズであるかどうかをチェック()
        {
            bool result = this._token!.CanWrite;
            Assert.That(result, Is.True);
        }

        [Test]
        public void データを取得()
        {
            string data = this._token!.Data;
            Assert.That(data.Length, Is.GreaterThan(0));
        }
    }
}
