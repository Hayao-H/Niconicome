using NUnit.Framework;
using Niconicome.Models.Domain.Niconico.Download.Comment;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NiconicomeTest.NetWork.Download.Comment
{
    [TestFixture]
    class CommentCollectionUnittest
    {
        private ICommentCollection? collection;

        [SetUp]
        public void SetUp()
        {
            this.collection = CommentCollection.GetInstance(0, 12345, 0, false);
            var com1 = new Response::Comment()
            {
                Chat = new Response::Chat()
                {
                    Content = "One",
                    No = 1,
                    Thread = "12345",
                }
            };
            var com2 = new Response::Comment()
            {
                Chat = new Response::Chat()
                {
                    Content = "One",
                    No = 2,
                    Thread = "12345",
                }
            };
            var com3 = new Response::Comment()
            {
                Chat = new Response::Chat()
                {
                    Content = "Two",
                    No = 3,
                    Thread = "12345",
                }
            };
            var com4 = new Response::Comment()
            {
                Chat = new Response::Chat()
                {
                    Content = "Three",
                    No = 4,
                    Thread = "12345",
                }
            };

            var com5 = new Response::Comment()
            {
                Chat = new Response::Chat()
                {
                    Content = "Four",
                    No = 5,
                    Thread = "12346",
                }
            };
            var thread = new Response::Comment()
            {
                Thread = new Response::Thread()
                {
                    ThreadThread = "12345",
                    LastRes = (int)new DateTimeOffset(DateTime.Now.Ticks, new TimeSpan(9, 0, 0)).ToUnixTimeSeconds()
                }
            };
            this.collection.Add(new List<Response::Comment>() { com1, com2, com3, com4, com5, thread });
        }

        [Test]
        public void 要素数を調べる()
        {
            Assert.That(this.collection!.Count, Is.EqualTo(5));
        }

        [Test]
        public void 要素をフィルターする()
        {
            this.collection!.Where(c => c.Chat?.Content == "One");
            Assert.That(this.collection!.Count, Is.EqualTo(2));
        }

        [Test]
        public void スレッド情報を取得する()
        {
            Assert.That(this.collection!.ThreadInfo?.ThreadThread, Is.EqualTo("12345"));
        }

        [Test]
        public void 削除する()
        {
            this.collection!.Clear();
            Assert.That(this.collection!.Count, Is.EqualTo(0));
        }

        [Test]
        public void 最初のコメントを取得する()
        {
            var comment = this.collection!.GetFirstComment();
            Assert.That(comment?.No, Is.EqualTo(1));
        }

        [Test]
        public void 全てのコメントを取得する()
        {
            var comments = this.collection!.GetAllComments();
            Assert.That(comments.Count(), Is.EqualTo(5));
        }
    }
}
