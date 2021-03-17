using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Request;
using Niconicome.Models.Domain.Niconico.Watch;
using Request = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Request;
using WathJson = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{
    public interface IOfficialVideoUtils
    {
        Task<ICommentAuthInfo> GetAuthInfoAsync(string threadId);
        Task<string> GetWaybackkeyAsync(string threadId);
    }

    public interface ICommentAuthInfo
    {
        string Force184 { get; }
        string ThreadKey { get; }
    }

    public interface ICommentRequestBuilder
    {
        Task<string> GetRequestDataAsync(IDmcInfo dmcInfo, ICommentOptions options);
    }

    public interface ICommentOptions
    {
        bool OwnerComment { get; }
        bool NoEasyComment { get; }
        long When { get; }
    }

    /// <summary>
    /// 認証情報等を取得
    /// </summary>
    public class OfficialVideoUtils : IOfficialVideoUtils
    {
        public OfficialVideoUtils(INicoHttp http)
        {
            this.http = http;
        }

        private readonly INicoHttp http;

        /// <summary>
        /// 認証情報をサーバーから取得する
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public async Task<ICommentAuthInfo> GetAuthInfoAsync(string threadId)
        {
            string data;
            try
            {
                data = await this.http.GetStringAsync(new Uri($"http://flapi.nicovideo.jp/api/getthreadkey?thread={threadId}"));
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"スレッドキー・force184の取得に失敗しました。(詳細: {e.Message})");
            }

            var deserialized = HttpUtility.ParseQueryString(data);

            var threadkey = deserialized["threadkey"];
            var force184 = deserialized["force_184"];

            if (threadkey is null || force184 is null) throw new HttpRequestException($"スレッドキー・force184の形式が不正です。(data: {data})");

            return new CommentAuthInfo(threadkey, force184);
        }

        /// <summary>
        /// Waybackkeyを取得する
        /// </summary>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public async Task<string> GetWaybackkeyAsync(string threadId)
        {
            string data;
            try
            {
                data = await this.http.GetStringAsync(new Uri($"https://flapi.nicovideo.jp/api/getwaybackkey?thread={threadId}"));
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"waybackkeyの取得に失敗しました。(詳細:{e.Message})");
            }

            var deserialized = HttpUtility.ParseQueryString(data);
            var waybackkey = deserialized["waybackkey"];

            if (waybackkey is null) throw new HttpRequestException($"waybcakkeyの形式が不正です。(data:{data})");

            return waybackkey;
        }
    }

    /// <summary>
    /// 認証情報
    /// </summary>
    public class CommentAuthInfo : ICommentAuthInfo
    {

        public CommentAuthInfo(string threadkey, string force184)
        {
            this.ThreadKey = threadkey;
            this.Force184 = force184;
        }

        public string ThreadKey { get; init; }

        public string Force184 { get; init; }
    }

    /// <summary>
    /// リクエストを生成する
    /// </summary>
    public class CommentRequestBuilder : ICommentRequestBuilder
    {

        public CommentRequestBuilder(IOfficialVideoUtils officialVideoUtils)
        {
            this.officialVideoUtils = officialVideoUtils;
        }

        private readonly IOfficialVideoUtils officialVideoUtils;

        private int commandIndex = 0;

        private int requestIndex = 0;

        private string? wayBackkey;

        /// <summary>
        /// リクエストメッセージを取得する
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<string> GetRequestDataAsync(IDmcInfo dmcInfo, ICommentOptions options)
        {
            var data = await this.GetRequestDataInternalAsync(dmcInfo, options);
            string stringData = JsonParser.Serialize(data);

            return stringData;
        }

        /// <summary>
        /// 内部処理
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task<List<Request::Comment>> GetRequestDataInternalAsync(IDmcInfo dmcInfo, ICommentOptions options)
        {
            var data = new List<Request::Comment>();
            var initialIndex = this.commandIndex;

            data.Add(this.GetPingContent(PingType.StartRequest, this.requestIndex));
            foreach (var thread in dmcInfo.CommentThreads)
            {
                //非activeスレッドはスキップ
                //if (!thread.IsActive) continue;

                //easyコメントをスキップ
                if (options.NoEasyComment && thread.Label == "easy") continue;

                //過去ログの場合はwaybackkeyを取得
                if (options.When != default)
                {
                    this.wayBackkey = await this.officialVideoUtils.GetWaybackkeyAsync(thread.Id.ToString());
                }

                //投コメ
                if (!options.OwnerComment && thread.IsOwnerThread) continue;

                data.Add(this.GetPingContent(PingType.StartContent, this.commandIndex));
                var itemThread = new Request::Comment() { Thread =this.GetThread(thread, dmcInfo, options) };
                data.Add(itemThread);
                data.Add(this.GetPingContent(PingType.EndContent, this.commandIndex));
                ++this.commandIndex;

                //leaf
                if (thread.IsLeafRequired)
                {
                    data.Add(this.GetPingContent(PingType.StartContent, this.commandIndex));
                    var itemLeaf = new Request::Comment() { ThreadLeaves = this.GetThreadLeaves(thread, dmcInfo, options, thread.Label == "easy") };
                    data.Add(itemLeaf);
                    data.Add(this.GetPingContent(PingType.EndContent, this.commandIndex));
                    ++this.commandIndex;
                    //leave_second_flag = true;
                }

            }
            data.Add(this.GetPingContent(PingType.EndRequest, this.requestIndex));
            ++this.requestIndex;

            this.commandIndex = initialIndex + data.Count;

            return data;

        }

        /// <summary>
        /// threadを構築
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="dmcInfo"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private Request::Thread GetThread(WathJson::Thread thread, IDmcInfo dmcInfo, ICommentOptions options)
        {
            var data = new Request::Thread
            {
                Thread_ = thread.Id.ToString(),
                Language = 0,
                UserId = dmcInfo.UserId,
                WithGlobal = 1,
                Scores = 1,
                Nicoru = 3,
                Fork = thread.Fork
            };

            if (thread.IsOwnerThread)
            {
                data.Version = "20061206";
                data.ResFrom = -1000;
            }
            else
            {
                data.Version = "20090904";
                data.ResFrom = null;
            }



            if (options.When != default)
            {
                data.When = options.When;
                data.Waybackkey = this.wayBackkey!;
            }
            else
            {
                data.When = null;
            }

            if (thread.IsThreadkeyRequired)
            {
                //if (this.authInfo is null)
                //{
                //    this.authInfo = await this.officialVideoUtils.GetAuthInfoAsync(data.Thread_);
                //}
                data.Force184 = "1";
                data.Threadkey = thread.Threadkey;
            }
            else if (options.When == default)
            {
                data.UserKey = dmcInfo.Userkey;
            }



            return data;
        }

        /// <summary>
        /// threadleavesを取得する
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="dmcInfo"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private Request::ThreadLeaves GetThreadLeaves(WathJson::Thread thread, IDmcInfo dmcInfo, ICommentOptions options, bool isEasy)
        {
            double divided = dmcInfo.Duration / 60d;
            double min = Math.Ceiling(divided);
            string humOrQuarter = isEasy ? "25" : options.When == default ? "100,1000" : "0,1000";
            string nicoru = isEasy || options.When == default ? ",nicoru:100" : "";

            var data = new ThreadLeaves
            {
                Thread = thread.Id.ToString(),
                Language = 0,
                UserId = dmcInfo.UserId,
                Content = $"0-{min}:{humOrQuarter}{nicoru}",
                Scores = 1,
                Nicoru = 3,
                Fork = thread.Fork
            };

            if (thread.IsThreadkeyRequired)
            {
                //2021(R3)/03/15の変更でthreadkeyは別途取得が不要になった
                //Force184は1でいいっぽい
                ///if (this.authInfo is null)
                ///{
                ///    this.authInfo = await this.officialVideoUtils.GetAuthInfoAsync(data.Thread);
                ///}
                data.Threadkey = thread.Threadkey;
                data.Force184 = "1";
            }
            else if (options.When == default)
            {
                data.UserKey = dmcInfo.Userkey;
            }

            if (options.When != default)
            {
                data.When = options.When;
                data.Waybackkey = this.wayBackkey;
            }
            else
            {
                data.When = null;
            }

            return data;

        }

        /// <summary>
        /// 挿入するPINGを取得する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Request::Comment GetPingContent(PingType type, int index)
        {
            string contet = type switch
            {
                PingType.StartRequest => $"rs: {index}",
                PingType.EndRequest => $"rf: {index}",
                PingType.StartContent => $"ps: {index}",
                PingType.EndContent => $"pf: {index}",
                _ => $"rs:{index}",
            };

            return new Request::Comment()
            {
                Ping = new Request::Ping()
                {
                    Content = contet
                }
            };
        }
    }

    /// <summary>
    /// オプション
    /// </summary>
    public class CommentOptions : ICommentOptions
    {
        public bool OwnerComment { get; set; }

        public bool NoEasyComment { get; set; }

        public long When { get; set; }
    }

    public enum PingType
    {
        StartRequest,
        EndRequest,
        StartContent,
        EndContent,
    }
}
