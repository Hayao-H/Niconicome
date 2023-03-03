﻿using System;
using System.Collections.Generic;
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

            if (Regex.IsMatch(path, @"/niconicome/video/\d+/(sm|nm|so)?\d+/video.mp4"))
            {
                return RequestType.Video;
            }

            return RequestType.None;
        }
    }
}
