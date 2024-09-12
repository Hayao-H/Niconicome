using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile;
using Niconicome.Models.Domain.Niconico.Watch;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.Session
{
    public interface ISessionManager
    {
        /// <summary>
        /// セッションを作成する
        /// </summary>
        /// <returns></returns>
        string CreateSession();

        /// <summary>
        /// セッションを削除する
        /// </summary>
        /// <param name="sessionID"></param>
        void DeleteSession(string sessionID);

        /// <summary>
        /// セッションに情報を追加する
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="videoFileList"></param>
        /// <param name="audioFileList"></param>
        /// <param name="videoKey"></param>
        /// <param name="audioKey"></param>
        void EditSession(string sessionID, string folderPath, IEnumerable<ISegmentInfo> videoFileList, IEnumerable<ISegmentInfo> audioFileList, string videoKey, string audioKey, string videoMapFileName, string audioMapFileName, string videoPlaylist, string audioPlaylist, int resolution, string videoIV, string audioIV);

        /// <summary>
        /// セッションの存在を確認
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        bool Exists(string sessionID);

        /// <summary>
        /// セッションを取得する
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        ISession GetSession(string sessionID);


    }

    public interface ISession
    {
        string SessionID { get; }

        Dictionary<string, ISegmentInfo> Videos { get; }

        Dictionary<string, ISegmentInfo> Audios { get; }

        string VideoKey { get; }

        string AudioKey { get; }

        string VideoMapFileName { get; }

        string AudioMapFileName { get; }

        string AudioPlaylist { get; }

        string VideoPlaylist { get; }

        string FolderPath { get; }

        string VideoIV { get; }

        string AudioIV { get; }

        int Resolution { get; }
    }

    public record Session : ISession
    {
        public required string SessionID { get; init; }

        public Dictionary<string, ISegmentInfo> Videos { get; init; } = new();

        public Dictionary<string, ISegmentInfo> Audios { get; init; } = new();

        public string VideoKey { get; set; } = string.Empty;

        public string AudioKey { get; set; } = string.Empty;

        public string VideoMapFileName { get; set; } = string.Empty;

        public string AudioMapFileName { get; set; } = string.Empty;

        public string AudioPlaylist { get; set; } = string.Empty;

        public string VideoPlaylist { get; set; } = string.Empty;

        public string FolderPath { get; set; } = string.Empty;

        public string VideoIV { get; set; } = string.Empty;

        public string AudioIV { get; set; } = string.Empty;

        public int Resolution { get; set; }

    }

    internal class SessionManager : ISessionManager
    {
        public string CreateSession()
        {
            string sessionID = Guid.NewGuid().ToString();
            this.sessions.Add(sessionID, new Session() { SessionID = sessionID });
            return sessionID;
        }

        public void DeleteSession(string sessionID)
        {
            this.sessions.Remove(sessionID);
        }

        public void EditSession(string sessionID, string folderPath, IEnumerable<ISegmentInfo> videoFileList, IEnumerable<ISegmentInfo> audioFileList, string videoKey, string audioKey, string videoMapFileName, string audioMapFileName, string videoPlaylist, string audioPlaylist, int resolution, string videoIV, string audioIV)
        {

            var session = this.sessions[sessionID];
            session.FolderPath = folderPath;
            session.VideoKey = videoKey;
            session.AudioKey = audioKey;
            session.AudioMapFileName = audioMapFileName;
            session.VideoMapFileName = videoMapFileName;
            session.AudioPlaylist = audioPlaylist;
            session.VideoPlaylist = videoPlaylist;
            session.Resolution = resolution;
            session.VideoIV = videoIV;
            session.AudioIV = audioIV;

            foreach (var v in videoFileList)
            {
                session.Videos.Add(v.FileName, v);
            }

            foreach (var a in audioFileList)
            {
                session.Audios.Add(a.FileName, a);
            }
        }

        public bool Exists(string sessionID)
        {
            return this.sessions.ContainsKey(sessionID);
        }

        public ISession GetSession(string sessionID)
        {
            return this.sessions[sessionID];
        }

        private readonly Dictionary<string, Session> sessions = new();
    }
}
