using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using DmcResponse = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Response;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IDmcDataHandler
    {
        DmcRequest::DmcPostData GetPostData(IDomainVideoInfo videoInfo);
        Task<IWatchSessionInfo> GetSessionInfoAsync(DmcRequest::DmcPostData dmcPostData);
        Task<IWatchSessionInfo> GetSessionInfoAsync(IDomainVideoInfo videoinfo);
    }

    /// <summary>
    /// DMCサーバーとの通信を管理する
    /// </summary>
    public class DmcDataHandler : IDmcDataHandler
    {
        public DmcDataHandler(INicoHttp http)
        {
            this.http = http;
        }

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        public DmcRequest::DmcPostData GetPostData(IDomainVideoInfo videoInfo)
        {
            var data = DmcRequest::DmcPostData.GetInstance();
            var sessionIfnfo = videoInfo.DmcInfo.SessionInfo;

            //nullチェック
            var nullList = new List<string>();
            var type = sessionIfnfo.GetType();
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                string name = prop.Name;
                object? value = prop.GetValue(sessionIfnfo);
                if (value is null)
                {
                    nullList.Add(name);
                }
            }

            if (nullList.Count > 0)
            {
                throw new InvalidOperationException($"DMCサーバーへのPOSTに必要なデータがnullです。(一覧: {string.Join(',', nullList)})");
            }

            data.Session.Recipe_id = sessionIfnfo.RecipeId;
            data.Session.Content_id = sessionIfnfo.ContentId;
            data.Session.Content_type = "movie";
            data.Session.Content_src_id_sets.Add(this.GetContentSrcIdSets(sessionIfnfo));
            data.Session.Timing_constraint = "unlimited";
            data.Session.Keep_method.Heartbeat.Lifetime = sessionIfnfo.HeartbeatLifetime;
            data.Session.Protocol.Name = "http";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Use_ssl = "yes";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Use_well_known_port = "yes";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Transfer_preset = sessionIfnfo.TransferPriset;
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Segment_duration = 6000;
            data.Session.Session_operation_auth.Session_operation_auth_by_signature.Signature = sessionIfnfo.Signature;
            data.Session.Session_operation_auth.Session_operation_auth_by_signature.Token = sessionIfnfo.Token;
            data.Session.Content_auth.Auth_type = sessionIfnfo.AuthType;
            data.Session.Content_auth.Service_id = "nicovideo";
            data.Session.Content_auth.Service_user_id = sessionIfnfo.ServiceUserId;
            data.Session.Content_auth.Content_key_timeout = sessionIfnfo.ContentKeyTimeout;
            data.Session.Client_info.Player_id = sessionIfnfo.PlayerId;
            data.Session.Priority = sessionIfnfo.Priority;


            return data;
        }

        /// <summary>
        /// セッション情報を取得する
        /// </summary>
        /// <param name="dmcPostData"></param>
        /// <returns></returns>
        public async Task<IWatchSessionInfo> GetSessionInfoAsync(DmcRequest::DmcPostData dmcPostData)
        {
            string json = JsonParser.Serialize(dmcPostData);

            var res = await this.http.PostAsync(new Uri("https://api.dmc.nico/api/sessions?_format=json"), new StringContent(json));
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"DMCサーバーへのPOSTに失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase})");
            }

            string responseString = await res.Content.ReadAsStringAsync();

            var deserialized = JsonParser.DeSerialize<DmcResponse::DmcResponseData>(responseString);

            if (deserialized.Data.Session.ContentUri.IsNullOrEmpty())
            {
                throw new HttpRequestException("DMCサーバーからの情報取得に失敗しました。(Data.Session.ContentUriがnullまたは空白です。)");
            }
            else if (deserialized.Data.Session.Id.IsNullOrEmpty())
            {
                throw new HttpRequestException("DMCサーバーからの情報取得に失敗しました。(Data.Idがnullまたは空白です。)");
            }

            return new WatchSessionInfo()
            {
                DmcResponseJsonData = JsonParser.Serialize(deserialized.Data),
                ContentUrl = deserialized.Data.Session.ContentUri,
                SessionId = deserialized.Data.Session.Id,
            };
        }

        /// <summary>
        /// セッション情報を取得する
        /// </summary>
        /// <param name="videoinfo"></param>
        /// <returns></returns>
        public async Task<IWatchSessionInfo> GetSessionInfoAsync(IDomainVideoInfo videoinfo)
        {
            var postData = this.GetPostData(videoinfo);
            return await this.GetSessionInfoAsync(postData);
        }

        #region private


        /// <summary>
        /// Content_Src_Id_Setsを構成する
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        private DmcRequest::Content_Src_Id_Sets GetContentSrcIdSets(ISessionInfo sessionInfo)
        {

            sessionInfo.Videos.Sort((a, b) =>
            {
                if (a.EndsWith("_low")) return -1;
                if (b.EndsWith("_low")) return 1;
                return string.Compare(a, b);
            });
            var videoSrc = sessionInfo.Videos.Select((value, index) => new { value, index }).ToList();
            string audio = sessionInfo.Audios[0];
            var sets = new DmcRequest::Content_Src_Id_Sets();

            foreach (var video in videoSrc)
            {
                var idsData = new DmcRequest::Content_Src_Ids();
                idsData.Src_id_to_mux.Audio_src_ids.Add(audio);
                int videosCount = videoSrc.Count - video.index;

                foreach (var i in Enumerable.Range(0, videosCount))
                {
                    idsData.Src_id_to_mux.Video_src_ids.Add(videoSrc[i].value);
                }

                //idsData.Src_id_to_mux.Video_src_ids = idsData.Src_id_to_mux.Video_src_ids.OrderByDescending(s => s).ToList();
                idsData.Src_id_to_mux.Video_src_ids.Sort((a, b) =>
                {
                    if (a.EndsWith("_low")) return 1;
                    if (b.EndsWith("_low")) return -1;
                    return string.Compare(b, a);
                });

                sets.Content_src_ids.Add(idsData);

            }

            return sets;
        }
        #endregion
    }

}
