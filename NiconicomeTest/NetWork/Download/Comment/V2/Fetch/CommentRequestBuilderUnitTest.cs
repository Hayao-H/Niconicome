using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using Request = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Request;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using NiconicomeTest.Stabs.Models.Domain.Utils;
using NUnit.Framework;
using Windows.Security.Authentication.OnlineId;
using WinRT;

namespace NiconicomeTest.NetWork.Download.Comment.V2.Fetch
{
    public class CommentRequestBuilderUnitTest
    {
        #region field

        private readonly string _userID = "user";

        private readonly string _userKey = "2525.testuserkey";

        private readonly string _threadKey = "2525.testthreadkey";

        private string? _key;

        private IDmcInfo? _dmcUser;

        private IDmcInfo? _dmcOfficial;

        private CommentRequestBuilder? _builder;

        #endregion

        [SetUp]
        public void SetUp()
        {
            var dmcU = new DmcInfo();
            var dmcO = new DmcInfo();

            dmcU.UserId = this._userID;
            dmcU.Userkey = this._userKey;
            dmcO.UserId = this._userID;

            foreach (var i in Enumerable.Range(0, 2))
            {
                var thread = new Thread()
                {
                    ID = 1,
                    Fork = i,
                    IsLeafRequired = true,
                    IsThreadkeyRequired = true,
                    Threadkey = this._threadKey,
                    Is184Forced = true,
                };

                dmcO.CommentThreads.Add(thread);
            }

            foreach (var i in Enumerable.Range(0, 2))
            {
                var thread = new Thread()
                {
                    ID = 1,
                    Fork = i,
                    IsLeafRequired = true,
                    IsOwnerThread = i == 0,
                };

                dmcU.CommentThreads.Add(thread);
            }

            this._dmcUser = dmcU;
            this._dmcOfficial = dmcO;

            this._builder = new CommentRequestBuilder(new OfficialCommentHandlerStub(), new LoggerStub());
            this._key = this._builder.ResetState();
        }

        [Test]
        public void キー認証()
        {
            bool result = this._builder!.CheckKeyForTest(this._key!);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ユーザー動画のリクエストを構築する()
        {
            IAttemptResult<List<Request::RequestRoot>> result = await this._builder!.BuildRequestAsyncInternalForTest(this._dmcUser!, new CommentFetchOption(true, true, false, 0), this._key!);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            List<Request::RequestRoot> data = result.Data!;

            Assert.That(data.Count, Is.EqualTo(14));
            Assert.That(data[0].Ping, Is.Not.Null);
            Assert.That(data[1].Ping, Is.Not.Null);
            Assert.That(data[2].Thread, Is.Not.Null);
            Assert.That(data[3].Ping, Is.Not.Null);
            Assert.That(data[4].Ping, Is.Not.Null);
            Assert.That(data[5].ThreadLeaves, Is.Not.Null);
            Assert.That(data[6].Ping, Is.Not.Null);
            Assert.That(data[7].Ping, Is.Not.Null);
            Assert.That(data[8].Thread, Is.Not.Null);
            Assert.That(data[9].Ping, Is.Not.Null);
            Assert.That(data[10].Ping, Is.Not.Null);
            Assert.That(data[11].ThreadLeaves, Is.Not.Null);
            Assert.That(data[12].Ping, Is.Not.Null);
            Assert.That(data[13].Ping, Is.Not.Null);

            Assert.That(data[2].Thread!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[8].Thread!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[5].ThreadLeaves!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[11].ThreadLeaves!.ThreadNo, Is.EqualTo("1"));

            Assert.That(data[2].Thread!.Fork, Is.EqualTo(0));
            Assert.That(data[8].Thread!.Fork, Is.EqualTo(1));
            Assert.That(data[5].ThreadLeaves!.Fork, Is.EqualTo(0));
            Assert.That(data[11].ThreadLeaves!.Fork, Is.EqualTo(1));

            Assert.That(data[2].Thread!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[8].Thread!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[5].ThreadLeaves!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[11].ThreadLeaves!.UserID, Is.EqualTo(this._userID));

            Assert.That(data[2].Thread!.UserKey, Is.EqualTo(this._userKey));
            Assert.That(data[8].Thread!.UserKey, Is.EqualTo(this._userKey));
            Assert.That(data[5].ThreadLeaves!.UserKey, Is.EqualTo(this._userKey));
            Assert.That(data[11].ThreadLeaves!.UserKey, Is.EqualTo(this._userKey));

            Assert.That(data[2].Thread!.ThreadKey, Is.Null);
            Assert.That(data[8].Thread!.ThreadKey, Is.Null);
            Assert.That(data[5].ThreadLeaves!.ThreadKey, Is.Null);
            Assert.That(data[11].ThreadLeaves!.ThreadKey, Is.Null);

            Assert.That(data[2].Thread!.Force184, Is.Null);
            Assert.That(data[8].Thread!.Force184, Is.Null);
            Assert.That(data[5].ThreadLeaves!.Force184, Is.Null);
            Assert.That(data[11].ThreadLeaves!.Force184, Is.Null);

            Assert.That(data[2].Thread!.When, Is.Null);
            Assert.That(data[8].Thread!.When, Is.Null);
            Assert.That(data[5].ThreadLeaves!.When, Is.Null);
            Assert.That(data[11].ThreadLeaves!.When, Is.Null);

            Assert.That(data[2].Thread!.ResFrom, Is.EqualTo(-1000));
            Assert.That(data[8].Thread!.ResFrom, Is.Null);

            Assert.That(data[2].Thread!.Version, Is.EqualTo("20061206"));
            Assert.That(data[8].Thread!.Version, Is.EqualTo("20090904"));
        }

        [Test]
        public async Task 公式動画のリクエストを構築する()
        {
            IAttemptResult<List<Request::RequestRoot>> result = await this._builder!.BuildRequestAsyncInternalForTest(this._dmcOfficial!, new CommentFetchOption(true, true, false, 0), this._key!);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            List<Request::RequestRoot> data = result.Data!;

            Assert.That(data.Count, Is.EqualTo(14));
            Assert.That(data[0].Ping, Is.Not.Null);
            Assert.That(data[1].Ping, Is.Not.Null);
            Assert.That(data[2].Thread, Is.Not.Null);
            Assert.That(data[3].Ping, Is.Not.Null);
            Assert.That(data[4].Ping, Is.Not.Null);
            Assert.That(data[5].ThreadLeaves, Is.Not.Null);
            Assert.That(data[6].Ping, Is.Not.Null);
            Assert.That(data[7].Ping, Is.Not.Null);
            Assert.That(data[8].Thread, Is.Not.Null);
            Assert.That(data[9].Ping, Is.Not.Null);
            Assert.That(data[10].Ping, Is.Not.Null);
            Assert.That(data[11].ThreadLeaves, Is.Not.Null);
            Assert.That(data[12].Ping, Is.Not.Null);
            Assert.That(data[13].Ping, Is.Not.Null);

            Assert.That(data[2].Thread!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[8].Thread!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[5].ThreadLeaves!.ThreadNo, Is.EqualTo("1"));
            Assert.That(data[11].ThreadLeaves!.ThreadNo, Is.EqualTo("1"));

            Assert.That(data[2].Thread!.Fork, Is.EqualTo(0));
            Assert.That(data[8].Thread!.Fork, Is.EqualTo(1));
            Assert.That(data[5].ThreadLeaves!.Fork, Is.EqualTo(0));
            Assert.That(data[11].ThreadLeaves!.Fork, Is.EqualTo(1));

            Assert.That(data[2].Thread!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[8].Thread!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[5].ThreadLeaves!.UserID, Is.EqualTo(this._userID));
            Assert.That(data[11].ThreadLeaves!.UserID, Is.EqualTo(this._userID));

            Assert.That(data[2].Thread!.UserKey, Is.Null);
            Assert.That(data[8].Thread!.UserKey, Is.Null);
            Assert.That(data[5].ThreadLeaves!.UserKey, Is.Null);
            Assert.That(data[11].ThreadLeaves!.UserKey, Is.Null);

            Assert.That(data[2].Thread!.ThreadKey, Is.EqualTo(this._threadKey));
            Assert.That(data[8].Thread!.ThreadKey, Is.EqualTo(this._threadKey));
            Assert.That(data[5].ThreadLeaves!.ThreadKey, Is.EqualTo(this._threadKey));
            Assert.That(data[11].ThreadLeaves!.ThreadKey, Is.EqualTo(this._threadKey));

            Assert.That(data[2].Thread!.Force184, Is.EqualTo("1"));
            Assert.That(data[8].Thread!.Force184, Is.EqualTo("1"));
            Assert.That(data[5].ThreadLeaves!.Force184, Is.EqualTo("1"));
            Assert.That(data[11].ThreadLeaves!.Force184, Is.EqualTo("1"));

            Assert.That(data[2].Thread!.When, Is.Null);
            Assert.That(data[8].Thread!.When, Is.Null);
            Assert.That(data[5].ThreadLeaves!.When, Is.Null);
            Assert.That(data[11].ThreadLeaves!.When, Is.Null);

            Assert.That(data[2].Thread!.ResFrom, Is.Null);
            Assert.That(data[8].Thread!.ResFrom, Is.Null);

            Assert.That(data[2].Thread!.Version, Is.EqualTo("20090904"));
            Assert.That(data[8].Thread!.Version, Is.EqualTo("20090904"));
        }

        [Test]
        public async Task 過去ログのリクエストを構築する()
        {
            IAttemptResult<List<Request::RequestRoot>> result = await this._builder!.BuildRequestAsyncInternalForTest(this._dmcUser!, new CommentFetchOption(true, true, true, 1648176555633), this._key!);

            Assert.That(result.IsSucceeded, Is.True);
            Assert.That(result.Data, Is.Not.Null);

            List<Request::RequestRoot> data = result.Data!;

            Assert.That(data[2].Thread!.When, Is.EqualTo(1648176555633));
            Assert.That(data[8].Thread!.When, Is.EqualTo(1648176555633));
            Assert.That(data[5].ThreadLeaves!.When, Is.EqualTo(1648176555633));
            Assert.That(data[11].ThreadLeaves!.When, Is.EqualTo(1648176555633));

            Assert.That(data[2].Thread!.WayBackKey, Is.EqualTo(OfficialCommentHandlerStub.Key));
            Assert.That(data[8].Thread!.WayBackKey, Is.EqualTo(OfficialCommentHandlerStub.Key));
            Assert.That(data[5].ThreadLeaves!.WayBackKey, Is.EqualTo(OfficialCommentHandlerStub.Key));
            Assert.That(data[11].ThreadLeaves!.WayBackKey, Is.EqualTo(OfficialCommentHandlerStub.Key));

        }

    }
}
