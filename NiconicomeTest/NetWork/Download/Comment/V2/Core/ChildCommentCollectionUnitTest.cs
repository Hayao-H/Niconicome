﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Browser;
using Niconicome.Models.Helper.Result;
using NUnit.Framework;
using CCore = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;

namespace NiconicomeTest.NetWork.Download.Comment.V2.Core
{
    internal class ChildCommentCollectionUnitTest
    {
        private CCore::ChildCommentCollection? _collection;

        private List<CCore::IComment>? _comments;

        private readonly string _thread = "1";

        private readonly string _fork = "";

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            this._comments = new List<CCore::IComment>(Enumerable.Range(1, 10).Select(i => new CCore::Comment()
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
            this._collection = new CCore::ChildCommentCollection(10, 2, this._thread, this._fork);
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
            //[1,2][null,null][null,null][7,null][null,null]
            foreach (var no in new[] { 1, 2, 7 }) this._collection!.Add(this._comments![no - 1]);

            Assert.That(this._collection!.Count, Is.EqualTo(3));

            IReadOnlyList<(CCore::IComment, int)> unFilled = this._collection.GetUnFilledRange();

            Assert.That(unFilled.Count, Is.EqualTo(1));
            Assert.That(unFilled[0].Item1.No, Is.EqualTo(7));
            Assert.That(unFilled[0].Item2, Is.EqualTo(4));

        }

        [Test]
        public void 最初のコメントを取得する()
        {
            //構造
            //[null,null][null,null][5,6][7,null][null,null]
            foreach (var no in new[] { 5, 6, 7 }) this._collection!.Add(this._comments![no - 1]);

            Assert.That(this._collection!.Count, Is.EqualTo(3));

            IAttemptResult<CCore::IComment> result = this._collection.GetFirstcomment();

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.No, Is.EqualTo(5));
        }

        [Test]
        public void 指定したコメントを取得する()
        {
            //構造
            //[1,2][null,null][null,null][7,null][null,null]
            foreach (var no in new[] { 1, 2, 7 }) this._collection!.Add(this._comments![no - 1]);


            Assert.That(this._collection!.Count, Is.EqualTo(3));

            IAttemptResult<CCore::IComment> result = this._collection!.Get(7);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data!.No, Is.EqualTo(7));
        }
    }
}
