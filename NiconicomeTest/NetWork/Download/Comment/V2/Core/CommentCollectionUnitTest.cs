using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CCore = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;

namespace NiconicomeTest.NetWork.Download.Comment.V2.Core
{
    internal class CommentCollectionUnitTest
    {
        private CCore::CommentCollection? _collection;

        private List<CCore::IComment>? _comments1;

        private List<CCore::IComment>? _comments2;

        private readonly string _thread = "1";

        private readonly int _fork = 0;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            this._comments1 = new List<CCore::IComment>(Enumerable.Range(1, 10).Select(i => new CCore::Comment()
            {
                No = i,
                Fork = 0,
                Thread = "1",
                Content = "Hello World",
            }));

            this._comments2 = new List<CCore::IComment>(Enumerable.Range(1, 10).Select(i => new CCore::Comment()
            {
                No = i,
                Fork = 0,
                Thread = "2",
                Content = "Hello World",
            }));

        }

        [SetUp]
        public void Setup()
        {
            this._collection = new CCore::CommentCollection(10, 2);
        }

        [Test]
        public void コメントを追加する()
        {
            foreach (var comment in this._comments1!)
            {
                this._collection!.Add(comment);
            }

            foreach (var comment in this._comments2!)
            {
                this._collection!.Add(comment);
            }

            Assert.That(this._collection!.Count, Is.EqualTo(20));
        }

        [Test]
        public void コメントの欠けをチェックする()
        {
            //構造
            //col1:[1,2][null,null][null,null][7,null][null,null]
            //col2:[1,2][null,null][5,6][7,null][null,null]
            foreach (var no in new[] { 1, 2, 7 }) this._collection!.Add(this._comments1![no - 1]);
            foreach (var no in new[] { 1, 2, 5, 6, 7 }) this._collection!.Add(this._comments2![no - 1]);

            Assert.That(this._collection!.Count, Is.EqualTo(8));

            IReadOnlyList<CCore::UnFilledRange> unFilled = this._collection.GetUnFilledRange();

            Assert.That(unFilled.Count, Is.EqualTo(2));

            CCore::UnFilledRange range1 = unFilled[0];

            Assert.That(range1.Thread, Is.EqualTo("1"));
            Assert.That(range1.Fork, Is.EqualTo(0));
            Assert.That(range1.Start.No, Is.EqualTo(7));
            Assert.That(range1.Count, Is.EqualTo(4));

            CCore::UnFilledRange range2 = unFilled[1];

            Assert.That(range2.Thread, Is.EqualTo("2"));
            Assert.That(range2.Fork, Is.EqualTo(0));
            Assert.That(range2.Start.No, Is.EqualTo(5));
            Assert.That(range2.Count, Is.EqualTo(2));

        }
    }
}
