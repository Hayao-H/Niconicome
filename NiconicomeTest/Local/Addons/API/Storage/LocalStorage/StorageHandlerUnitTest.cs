﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage;
using NiconicomeTest.Stabs.Models.Domain.Local.Addon.API.Storage;
using NUnit.Framework;

namespace NiconicomeTest.Local.Addons.API.Storage.LocalStorage
{

    [TestFixture]
    class StorageHandlerUnitTest
    {
        private IStorageHandler? handler;

        private Dictionary<string, string>? data;

        [SetUp]
        public void SetUp()
        {
            this.data = new Dictionary<string, string>();

            this.data.Add("greet", "Hello World!");
            this.data.Add("1", "Niconicome");
            this.data.Add("2", "Test");

            var instance = new StorageHandler(new StorageHelperStab(this.data));
            instance.Initialize("", "");
            this.handler = instance;
        }

        [TestCase("greet", "Hello World!")]
        [TestCase("1", "Niconicome")]
        [TestCase("2", "Test")]
        public void データを取得する(string key, string expectedValue)
        {
            string? value = this.handler!.GetItem(key);
            Assert.That(value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void データを削除する()
        {
            this.handler!.RemoveItem("greet");
            Assert.That(this.data!.Count, Is.EqualTo(2));
        }

        [Test]
        public void 全てのアイテムを削除する()
        {
            this.handler!.Clear();
            Assert.That(this.data!.Count, Is.EqualTo(0));
        }

        [Test]
        public void アイテムをセットする()
        {
            this.handler!.SetItem("key", "value");
            Assert.That(this.data!["key"], Is.EqualTo("value"));
            Assert.That(this.data!.Count, Is.EqualTo(4));
        }
    }
}
