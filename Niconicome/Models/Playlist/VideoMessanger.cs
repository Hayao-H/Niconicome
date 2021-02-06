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

        /// <summary>
        /// メッセージを書き込む
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="message"></param>
        public static void Write(string guid,string message)
        {
            if (!VideoMessanger.messages.ContainsKey(guid))
            {
                VideoMessanger.messages.Add(guid, message);
            }
            else
            {
                VideoMessanger.messages[guid] = message;
            }
        }

        /// <summary>
        /// メッセージを取得する
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetMessage(string guid)
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
}
