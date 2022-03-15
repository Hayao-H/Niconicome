using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Core = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;

namespace NiconicomeTest.NetWork.Download.Comment.V2.Comment
{
    internal class ChildCommentCollectionUnitTest
    {
        private Core::ChildCommentCollection? _collection;

        private List<Core::IComment>? _comments;

        private readonly string _thread = "1";

        private readonly int _fork = 0;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            this._comments = new List<Core.IComment>(Enumerable.Range(1, 10).Select(i => new Core::Comment()
            {
                No = i,
                Fork = this._fork,
                Thread = this._thread,
                Content = "Hello World",
            }));

        }

        [SetUp]
        public void Setup()
        {
            this._collection = new Core::ChildCommentCollection(10, 2, this._thread, this._fork);
        }

        [Test]
        public void コメントを追加する()
        {
            foreach (var comment in this._comments!)
            {
                this._collection!.Add(comment);
            }

            Assert.That(this._collection!.Count, Is.EqualTo(10));

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 1; j++)
                {
                    Assert.That(this._collection.CommentsForTest[i][j].No, Is.EqualTo(2 * i + j + 1));
                }
            }
        }

        [Test]
        public void コメントの欠けをチェックする()
        {
            //構造
            //[1,2][null,null][5,6][7,null][null,null]
            foreach (var no in new[] { 1, 2, 5, 6, 7 }) this._collection!.Add(this._comments![no - 1]);

            Assert.That(this._collection!.Count, Is.EqualTo(5));

            List<(Core::IComment, int)> unFilled = this._collection.GetUnFilledRange();

            Assert.That(unFilled.Count, Is.EqualTo(1));
            Assert.That(unFilled[0].Item1.No, Is.EqualTo(5));
            Assert.That(unFilled[0].Item2, Is.EqualTo(2));



        }
    }
}
