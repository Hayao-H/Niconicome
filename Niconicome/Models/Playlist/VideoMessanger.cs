using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Playlist
{
    /// <summary>
    /// メッセージ共有クラス
    /// </summary>
    public static class VideoMessanger
    {

        private static readonly Dictionary<string, string> messages = new();

        private static readonly object lockObj = new();

        public static event EventHandler<VideoMessageChangeEventArgs>? VideoMessageChange;

        /// <summary>
        /// メッセージを書き込む
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="message"></param>
        public static void Write(string guid, string message)
        {
            lock (VideoMessanger.lockObj)
            {
                string old = string.Empty;

                if (!VideoMessanger.messages.ContainsKey(guid))
                {
                    VideoMessanger.messages.Add(guid, message);
                }
                else
                {
                    old = VideoMessanger.messages[guid];
                    VideoMessanger.messages[guid] = message;
                }

                VideoMessanger.RaiseOnMessage(guid, old, message);
            }

        }

        /// <summary>
        /// メッセージを取得する
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetMessage(string guid)
        {
            lock (VideoMessanger.lockObj)
            {
                if (!VideoMessanger.messages.ContainsKey(guid))
                {
                    return String.Empty;
                }
                else
                {
                    return VideoMessanger.messages[guid];
                }
            }
        }

        /// <summary>
        /// イベント発火
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldm"></param>
        /// <param name="newm"></param>
        private static void RaiseOnMessage(string id, string oldm, string newm)
        {
            VideoMessanger.VideoMessageChange?.Invoke(null, new VideoMessageChangeEventArgs(id, oldm, newm));
        }
    }

    /// <summary>
    /// メッセージ更新イベント
    /// </summary>
    public class VideoMessageChangeEventArgs : EventArgs
    {
        public VideoMessageChangeEventArgs(string id, string oldm, string newm)
        {
            this.ID = id;
            this.OldValue = oldm;
            this.NewValue = newm;
        }

        public string ID { get; init; }

        public string OldValue { get; init; }

        public string NewValue { get; init; }
    }
}
