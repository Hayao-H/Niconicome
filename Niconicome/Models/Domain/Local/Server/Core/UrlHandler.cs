using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public interface IUrlHandler
    {
        /// <summary>
        /// リクエストの種別を判断する
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        RequestType GetReqyestType(Uri request);
    }

    public class UrlHandler : IUrlHandler
    {
        public RequestType GetReqyestType(Uri request)
        {
            string path = request.AbsolutePath;

            if (Regex.IsMatch(path, @"/niconicome/video/\d+/(sm|nm|so)?\d+/video\.mp4"))
            {
                return RequestType.Video;
            }

            if (path == @"/niconicome/hls/playlist.m3u8")
            {
                return RequestType.M3U8;
            }

            if (Regex.IsMatch(path, @"/niconicome/hls/video\d+\.ts"))
            {
                return RequestType.TS;
            }

            if (path == @"/niconicome/css/userChrome.css")
            {
                return RequestType.UserChrome;
            }

            if (Regex.IsMatch(path, @"/niconicome/watch/v1/.+"))
            {
                return RequestType.WatchAPI;
            }

            if (Regex.IsMatch(path, @"/niconicome/api/comment/v1/.+"))
            {
                return RequestType.CommentAPI;
            }

            if (Regex.IsMatch(path, @"/niconicome/api/regacyhls/v1/.+"))
            {
                return RequestType.RegacyHLSAPI;
            }

            if (Regex.IsMatch(path, @"/niconicome/resource/v1/.+") || path == "/favicon.ico")
            {
                return RequestType.ResourceAPI;
            }

            if (Regex.IsMatch(path, @"/niconicome/api/videoinfo/v1/.+"))
            {
                return RequestType.VideoInfoAPI;
            }

            if (Regex.IsMatch(path, @"/niconicome/api/ng/v1/.+"))
            {
                return RequestType.NG;
            }

            return RequestType.None;
        }
    }
}
